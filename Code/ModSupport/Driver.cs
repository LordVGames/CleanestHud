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
using MonoDetour.HookGen;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.DetourTypes;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
namespace CleanestHud.ModSupport;


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
    private const float _weaponSlotPosX = -269.2665f;
    private const float _chargeBarScaleX = 0.44f;
    private const float _chargeBarPosY = -0.5f;




    [MonoDetourTargets(typeof(RobDriver.Modules.Misc.DriverHooks), GenerateControlFlowVariants = true)]
    internal static class Hooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            if (!ModIsRunning)
            {
                return;
            }


            Mdh.RobDriver.Modules.Misc.DriverHooks.NormalHudSetup.ILHook(CheckForSprintAndInventoryReminders);
            Mdh.RobDriver.Modules.Misc.DriverHooks.NormalHudSetup.ControlFlowPrefix(MaybeAllow);
            Mdh.RoR2.UI.HUD.OnDestroy.Postfix(OnHudDestroy);


            CleanestHud.HudChanges.HudColor.OnHudColorEditsFinished += ModSupport.Driver.OnHudColorUpdate;
            CleanestHud.HudChanges.SurvivorSpecific.SurvivorSpecific.OnSurvivorSpecificHudEditsFinished += Driver.OnSurvivorSpecificHudEditsFinished;
            ConfigOptions.OnShowSkillKeybindsChanged += SetWeaponTextStatus;
        }


        private static void OnHudDestroy(HUD self)
        {
            AllowDriverWeaponSlotCreation = false;
        }


        private static ReturnFlow MaybeAllow(ref HUD hud)
        {
            if (IsHudUserBlacklisted || AllowDriverWeaponSlotCreation)
            {
                return ReturnFlow.None;
            }
            return ReturnFlow.SkipOriginal;
        }


        private static void CheckForSprintAndInventoryReminders(ILManipulationInfo info)
        {
            ILWeaver w = new(info);
            Instruction startOfSkip = null!;
            Instruction endOfSkip = null!;


            w.MatchRelaxed(
                x => x.MatchLdloc(out _) && w.SetInstructionTo(ref startOfSkip, x),
                x => x.MatchLdstr("SprintCluster")
            ).ThrowIfFailure()
            .MatchRelaxed(
                x => x.MatchLdloc(out _),
                x => x.MatchLdstr("InventoryCluster") && w.SetInstructionTo(ref endOfSkip, x) && w.SetCurrentTo(x)
            ).ThrowIfFailure()
            .InsertBranchOverIfFalse(
                startOfSkip, endOfSkip,
                w.CreateDelegateCall(() =>
                {
                    return IsHudUserBlacklisted;
                })
            );
        }
    }


    internal static void OnSurvivorSpecificHudEditsFinished()
    {
        if (!IsHudCameraTargetDriver)
        {
            TryDestroyDriverWeaponSlot();
        }
        else
        {
            SkillsScaler.SkillScalerLocalPosition = new Vector2(35, SkillsScaler.SkillScalerDefaultY);
            SprintAndInventoryReminders.SprintClusterLocalPosition = new Vector2(-439, -64);
            SprintAndInventoryReminders.InventoryClusterLocalPosition = new Vector2(-511, -64);
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
            Log.Error("Driver weapon slot was null in EditWeaponSlotStructure");
            return;
        }
        Transform driverWeaponSlotDisplayRoot = driverWeaponSlot.GetChild(1);
        Transform weaponSlotTextPanel = driverWeaponSlotDisplayRoot.GetChild(6);
        Transform weaponChargeBar = driverWeaponSlotDisplayRoot.GetChild(10);
        Transform firstSkillSlotTextPanel = MyHud.skillIcons[0].transform.GetChild(5);



        // would copy the first skill's scale here but it makes it too small
        driverWeaponSlot.localScale = new Vector3(2.3f, 2.3f, 2.3f);
        driverWeaponSlot.localPosition = new Vector3(
            _weaponSlotPosX,
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
            _chargeBarScaleX,
            weaponChargeBar.localScale.y,
            weaponChargeBar.localScale.z
        );
        weaponChargeBar.localPosition = new Vector3(
            weaponChargeBar.localPosition.x,
            _chargeBarPosY,
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
        Transform weaponSlotTextPanel = driverWeaponSlotDisplayRoot.Find("EquipmentTextBackgroundPanel");
        weaponSlotTextPanel?.gameObject.SetActive(false);
    }
}