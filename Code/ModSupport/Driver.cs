using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using HarmonyLib;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using CleanestHud.HudChanges;
using MiscFixes.Modules;

namespace CleanestHud.ModSupport
{
    internal class Driver
    {
        private static bool? _modexists;
        internal static bool ModIsRunning
        {
            get
            {
                _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(RobDriver.DriverPlugin.MODUID);
                return (bool)_modexists;
            }
        }
        private static bool IsHudCameraTargetDriver
        {
            get
            {
                return HudCameraTargetBody?.baseNameToken == "ROB_DRIVER_BODY_NAME";
            }
        }
        private static bool AllowDriverWeaponSlotCreation = false;
        private static Transform DriverWeaponSlot
        {
            get
            {
                if (!IsHudEditable)
                {
                    return null;
                }

                return MyHud.equipmentIcons[0].gameObject.transform.parent.Find("WeaponSlot");
            }
        }



        [HarmonyPatch]
        internal static class HarmonyPatches
        {
            [HarmonyPatch(typeof(RobDriver.Modules.Misc.DriverHooks), nameof(RobDriver.Modules.Misc.DriverHooks.NormalHudSetup))]
            [HarmonyILManipulator]
            internal static void TakeActionRegardingThatIndividual(ILContext il)
            {
                // relax i'll handle it
                ILCursor c = new(il);
                // I AM THE IL CURSOR
                ILLabel skipSprintAndInventoryReminders = c.DefineLabel();



                if (!c.TryGotoNext(MoveType.AfterLabel,
                    x => x.MatchLdloc(0),
                    x => x.MatchLdstr("SprintCluster")
                ))
                {
                    Log.Error($"COULD NOT IL HOOK {il.Method.Name} PART 1");
                    Log.Warning($"il is {il}");
                }
                c.EmitDelegate<Func<bool>>(() => { return IsHudUserBlacklisted; });
                c.Emit(OpCodes.Brfalse, skipSprintAndInventoryReminders);



                if (!c.TryGotoNext(MoveType.After,
                    x => x.MatchLdloc(0),
                    x => x.MatchLdstr("InventoryCluster")
                ))
                {
                    Log.Error($"COULD NOT IL HOOK {il.Method.Name} PART 2");
                    Log.Warning($"il is {il}");
                }
                c.Index += 4; // go after the line
                c.MarkLabel(skipSprintAndInventoryReminders);
            }


            [HarmonyPatch(typeof(RobDriver.Modules.Misc.DriverHooks), nameof(RobDriver.Modules.Misc.DriverHooks.NormalHudSetup))]
            [HarmonyPrefix]
            internal static bool ShouldAllowWeaponSlotCreation()
            {
                if (IsHudUserBlacklisted)
                {
                    return true;
                }
                else
                {
                    return AllowDriverWeaponSlotCreation;
                }
            }
        }



        internal static void HUD_OnDestroy(On.RoR2.UI.HUD.orig_OnDestroy orig, HUD self)
        {
            orig(self);
            AllowDriverWeaponSlotCreation = false;
        }

        internal static void OnSurvivorSpecificHudEditsFinished()
        {
            if (!IsHudCameraTargetDriver)
            {
                TryDestroyDriverWeaponSlot();
                HudStructure.ResetSkillsScalerLocalPositionToDefault();
                HudStructure.ResetSprintClusterLocalPositionToDefault();
            }
            else
            {
                HudStructure.SkillScalerLocalPosition = new Vector2(35, HudStructure.SkillScalerDefaultY);
                // -46
                HudStructure.SprintClusterLocalPosition = new Vector2(-439, -64);
                HudStructure.InventoryClusterLocalPosition = new Vector2(-511, -64);
            }
        }
        private static void TryDestroyDriverWeaponSlot()
        {
            try
            {
                Destroy(MyHud.equipmentIcons[0].gameObject.transform.parent.Find("WeaponSlot").gameObject);
            }
            catch { }
        }

        // once OnHudColorUpdate happens if the weapon slot is created it'll copy what we've edited and (almost) everything will be good
        // doing it here also makes it a survivor specific thing that happens regardless if driver is blacklisted or not
        internal static void OnHudColorUpdate()
        {
            if (!IsHudCameraTargetDriver)
            {
                return;
            }


            AllowDriverWeaponSlotCreation = true;
            // HACK: try/catch block is here to hide an NRE from Driver's NormalHudSetup until it gets fixed on DriverMod's end
            // it also allows EditWeaponSlotStructure to happen through what would be an NRE
            try
            {
                RobDriver.Modules.Misc.DriverHooks.NormalHudSetup(MyHud);
            }
            catch { }
            if (!IsHudUserBlacklisted && ConfigOptions.AllowHudStructureEdits.Value)
            {
                MyHud?.StartCoroutine(DelayEditWeaponSlotStructure());
            }
        }
        private static IEnumerator DelayEditWeaponSlotStructure()
        {
            yield return null;
            // covering for future driver support update
            try
            {
                EditWeaponSlotStructure();
            }
            catch { }
        }
        private static void EditWeaponSlotStructure()
        {
            if (!IsHudEditable)
            {
                return;
            }
            Transform driverWeaponSlot = DriverWeaponSlot;
            if (driverWeaponSlot == null)
            {
                //Log.Error("Driver weapon slot was null in EditWeaponSlotStructure");
                return;
            }
            Transform driverWeaponSlotDisplayRoot = driverWeaponSlot.GetChild(1);
            Transform weaponSlotTextPanel = driverWeaponSlotDisplayRoot.GetChild(6);
            Transform weaponChargeBar = driverWeaponSlotDisplayRoot.GetChild(10);
            Transform firstSkillSlotTextPanel = MyHud.skillIcons[0].transform.GetChild(5);



            // would copy the first skill's scale here but it makes it too small
            driverWeaponSlot.localScale = new Vector3(2.3f, 2.3f, 2.3f);
            driverWeaponSlot.localPosition = new Vector3(
                -269.2665f,
                driverWeaponSlot.localPosition.y,
                driverWeaponSlot.localPosition.z
            );

            weaponSlotTextPanel.position = new Vector3(
                weaponSlotTextPanel.position.x,
                firstSkillSlotTextPanel.position.y,
                weaponSlotTextPanel.position.z
            );
            SetWeaponTextStatus();
            weaponChargeBar.localScale = new Vector3(
                0.44f,
                weaponChargeBar.localScale.y,
                weaponChargeBar.localScale.z
            );
            weaponChargeBar.localPosition = new Vector3(
                weaponChargeBar.localPosition.x,
                -0.5f,
                weaponChargeBar.localPosition.z
            );
        }



        internal static void SetWeaponTextStatus()
        {
            if (!IsHudEditable)
            {
                return;
            }
            Transform driverWeaponSlot = DriverWeaponSlot;
            if (driverWeaponSlot == null)
            {
                Log.Error("Driver weapon slot was null in EditWeaponSlotStructure");
                return;
            }
            Transform driverWeaponSlotDisplayRoot = driverWeaponSlot.GetChild(1);
            Transform weaponSlotTextPanel = driverWeaponSlotDisplayRoot.GetChild(6);
            weaponSlotTextPanel.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
        }
    }
}