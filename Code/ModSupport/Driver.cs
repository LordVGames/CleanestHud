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
using static CleanestHud.HudResources.ImportantHudTransforms;
using CleanestHud.HudChanges;
using MiscFixes.Modules;
using MonoDetour.HookGen;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.DetourTypes;
using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
using CleanestHud.HudChanges.SurvivorSpecific;
using RobDriver.Modules.Components;
using HG;
using RobDriver.Modules.Components.UI;
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
    internal static bool IsHudCameraTargetDriver
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

            return ExtraButtonPanels.Find("WeaponSlot");
        }
    }
    private const float _chargeBarScaleX = 0.44f;
    private const float _chargeBarPosY = -0.5f;
    private const float _skillScalerXAdjustment = 35;
    private static Vector3 _weaponSlotScale = new(2.3f, 2.3f, 2.3f);
    private static Vector3 _weaponSlotLocalPosNoAmmo = new(0, -28.5f);
    private static Vector3 _weaponSlotLocalPosWithAmmo = new(-7, _weaponSlotLocalPosNoAmmo.y);




    [MonoDetourTargets(typeof(RobDriver.Modules.Misc.DriverHooks), GenerateControlFlowVariants = true)]
    [MonoDetourTargets(typeof(DriverController))]
    [MonoDetourTargets(typeof(WeaponIcon))]
    internal static class Hooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            if (!ModIsRunning)
            {
                return;
            }


            Mdh.RobDriver.Modules.Misc.DriverHooks.NormalHudSetup.ControlFlowPrefix(MaybeAllow);
            Mdh.RobDriver.Modules.Misc.DriverHooks.NormalHudSetup.Postfix(AfterHudSetup);
            Mdh.RobDriver.Modules.Components.DriverController.OnWeaponChanged.Postfix(WeaponChanged);
            Mdh.RobDriver.Modules.Components.UI.WeaponIcon.SetDisplay.Postfix(Thing);
            Mdh.RoR2.UI.HUD.OnDestroy.Postfix(OnHudDestroy);


            HudColor.OnHudColorEditsFinished += ModSupport.Driver.OnHudColorEditsFinished;
            SurvivorSpecific.OnSurvivorSpecificHudChangesFinished += Driver.OnSurvivorSpecificHudEditsFinished;
            ConfigOptions.ShowSkillKeybinds.SettingChanged += SetWeaponTextStatus;
        }


        private static void Thing(WeaponIcon self)
        {
            if (self.iDrive.maxWeaponTimer > 0)
            {
                // undo move it does when the ammo bar appears
                self.displayRoot.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -5);
            }
        }


        private static void WeaponChanged(DriverController self, ref DriverWeaponDef newWeapon)
        {
            MyHud?.StartCoroutine(DelayEditWeaponSlotStructure());
        }


        private static void AfterHudSetup(ref HUD hud)
        {
            MyHud?.StartCoroutine(DelayEditWeaponSlotStructure());
        }
        private static IEnumerator DelayEditWeaponSlotStructure()
        {
            yield return null;
            EditWeaponSlotStructure();
            yield break;
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
            driverWeaponSlot.localScale = _weaponSlotScale;
            var positionSetter = driverWeaponSlot.EnsureComponent<HudEditorComponents.TransformLocalPositionSetter>();
            positionSetter.SetNewLocalPositionForSetterDelayed(_weaponSlotLocalPosNoAmmo);


            weaponSlotTextPanel.position = new Vector3(
                weaponSlotTextPanel.position.x,
                firstSkillSlotTextPanel.position.y,
                weaponSlotTextPanel.position.z
            );
            ActuallySetWeaponTextStatus();
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
    }


    internal static void OnSurvivorSpecificHudEditsFinished()
    {
        if (!IsHudCameraTargetDriver)
        {
            try
            {
                Destroy(DriverWeaponSlot?.gameObject);
            }
            catch { }
        }
        else
        {
            SkillsScaler.SkillScalerLocalPosition = new Vector2(SkillsScaler.SkillScalerDefaultX + _skillScalerXAdjustment, SkillsScaler.SkillScalerDefaultY);
        }
    }


    // once OnHudColorEditsBegun finishes if the weapon slot is created it'll copy what we've edited and (almost) everything will be good
    internal static void OnHudColorEditsFinished()
    {
        if (!IsHudCameraTargetDriver)
        {
            return;
        }


        AllowDriverWeaponSlotCreation = true;
        RobDriver.Modules.Misc.DriverHooks.NormalHudSetup(MyHud);
    }


    internal static void SetWeaponTextStatus(object sender, EventArgs e)
    {
        ActuallySetWeaponTextStatus();
    }
    private static void ActuallySetWeaponTextStatus()
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
        Transform weaponSlotText = weaponSlotTextPanel.Find("EquipmentKeyText");
        if (!weaponSlotText.TryGetComponent<HGTextMeshProUGUI>(out var textComponent))
        {
            return;
        }
        textComponent.text = "Weapon";
        weaponSlotTextPanel?.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
        weaponSlotText.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
    }
}