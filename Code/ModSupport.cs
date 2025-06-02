using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
using HarmonyLib;
using RoR2;
using RoR2.UI;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using SS2;
using static CleanestHud.Main;
using CleanestHud.HudChanges;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace CleanestHud
{
    internal static class ModSupport
    {
        internal static class RiskOfOptionsMod
        {
            private static bool? _modexists;
            public static bool ModIsRunning
            {
                get
                {
                    _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(RiskOfOptions.PluginInfo.PLUGIN_GUID);
                    return (bool)_modexists;
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void AddOptions()
            {
                ModSettingsManager.SetModIcon(ModAssets.AssetBundle.LoadAsset<Sprite>("CleanestHudIcon.png"));
                ModSettingsManager.SetModDescription("Adds an artifact that disables proc chains and prevents most items from starting a proc chain.");


                ModSettingsManager.AddOption(
                    new StepSliderOption(
                        ConfigOptions.HudTransparency,
                        new StepSliderConfig{
                            min = 0,
                            max = 1f,
                            increment = 0.025f
                        }
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.ShowSkillAndEquipmentOutlines
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.ShowSkillKeybinds
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.ShowSprintAndInventoryKeybinds
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowInspectPanelFadeIn
                    )
                );
                ModSettingsManager.AddOption(
                    new StepSliderOption(
                        ConfigOptions.InspectPanelFadeInDuration,
                        new StepSliderConfig
                        {
                            min = 0.025f,
                            max = 10,
                            increment = 0.025f
                        }
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowAllyCardBackgrounds
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowScoreboardLabels
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowScoreboardItemHighlightColoring
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowAutoScoreboardHighlight
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowConsistentDifficultyBarColor
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowSimulacrumWaveBarAnimating
                    )
                );


                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowVoidFiendMeterAnimating
                    )
                );
                ModSettingsManager.AddOption(
                    new ChoiceOption(
                        ConfigOptions.SeekerMeditateHudPosition
                    )
                );
                ModSettingsManager.AddOption(
                    new ChoiceOption(
                        ConfigOptions.SeekerLotusHudPosition
                    )
                );
                ModSettingsManager.AddOption(
                    new StringInputFieldOption(
                        ConfigOptions.BodyNameBlacklist_Config
                    )
                );


                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.AllowDebugLogging
                    )
                );
            }
        }

        internal static class LookingGlassMod
        {
            private static bool? _modexists;
            internal static bool ModIsRunning
            {
                get
                {
                    _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID);
                    return (bool)_modexists;
                }
            }
            internal static bool ItemCountersConfigValue
            {
                [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
                get
                {
                    if (ModIsRunning)
                    {
                        return LookingGlass.ItemCounters.ItemCounter.itemCounters.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
            }



            internal static bool StatsPanelConfigValue
            {
                [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
                get
                {
                    if (ModIsRunning)
                    {
                        return LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay.Value;
                    }
                    return false;
                }
            }
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void HookStatsPanelConfig_SettingChanged()
            {
                if (LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay != null)
                {
                    LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay.SettingChanged += StatsPanelConfig_SettingChanged;
                }
            }
            internal static void StatsPanelConfig_SettingChanged(object sender, EventArgs e)
            {
                if (!IsHudEditable)
                {
                    return;
                }

                if (StatsPanelConfigValue)
                {
                    MyHud.StartCoroutine(DelayRemoveLookingGlassStatsPanelBackground());
                }
            }



            internal static IEnumerator DelayRemoveLookingGlassStatsPanelBackground()
            {
                yield return null;
                Transform upperRightCluster = MyHud.gameModeUiRoot.transform;
                Transform runInfoHudPanel = upperRightCluster.GetChild(0);
                Transform rightInfoBar = runInfoHudPanel.Find("RightInfoBar");
                Transform playerStats = null;

                while (!rightInfoBar.Find("PlayerStats"))
                {
                    Log.Debug("Couldn't find LookingGlass PlayerStats, waiting a lil bit");
                    yield return null;
                }
                playerStats = rightInfoBar.Find("PlayerStats");
                while (!playerStats.TryGetComponent<RoR2.UI.SkinControllers.PanelSkinController>(out _))
                {
                    Log.Debug("Couldn't find LookingGlass PlayerStats PanelSkinController, waiting a lil bit");
                    yield return null;
                }
                RemoveLookingGlassStatsPanelBackground(playerStats);
            }
            private static void RemoveLookingGlassStatsPanelBackground(Transform playerStats)
            {
                UnityEngine.Object.Destroy(playerStats.GetComponent<RoR2.UI.SkinControllers.PanelSkinController>());
                UnityEngine.Object.Destroy(playerStats.GetComponent<Image>());
            }



            internal static void OnHudDetailEditsFinished()
            {
                if (ModIsRunning && StatsPanelConfigValue)
                {
                    MyHud.StartCoroutine(DelayRemoveLookingGlassStatsPanelBackground());
                }
            }
        }

        internal static class Starstorm2
        {
            private static bool? _modexists;
            internal static bool ModIsRunning
            {
                get
                {
                    _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SS2Main.GUID);
                    return (bool)_modexists;
                }
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
            {
                if (!IsHudFinishedLoading)
                {
                    Log.Debug("onBodyInventoryChangedGlobal happened while the HUD was not done loading, returning");
                    return;
                }

                int currentInjectorItemCount = characterBody.inventory.GetItemCount(SS2Content.Items.CompositeInjector);
                if (currentInjectorItemCount != CompositeInjectorSupport.KnownInjectorSlotCount)
                {
                    CompositeInjectorSupport.KnownInjectorSlotCount = currentInjectorItemCount;
                    MyHud.StartCoroutine(CompositeInjectorSupport.DelayEditInjectorSlots());
                    MyHud.StartCoroutine(CompositeInjectorSupport.DelayColorInjectorSlots());
                }
            }

            internal static class CompositeInjectorSupport
            {
                internal static int KnownInjectorSlotCount = -1;

                internal static IEnumerator DelayEditInjectorSlots()
                {
                    yield return null;
                    EditInjectorSlots();
                }
                [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
                internal static void EditInjectorSlots()
                {
                    if (!ModIsRunning || !IsHudFinishedLoading)
                    {
                        return;
                    }
                    SS2.Items.CompositeInjector.IconHolder ss2EquipmentIconHolder;
                    bool compositeInjectorExists = MyHud.gameObject.TryGetComponent<SS2.Items.CompositeInjector.IconHolder>(out ss2EquipmentIconHolder);
                    if (!compositeInjectorExists)
                    {
                        return;
                    }

                    for (int i = 0; i < ss2EquipmentIconHolder.icons.Length; i++)
                    {
                        if (!ss2EquipmentIconHolder.icons[i].displayRoot.activeSelf)
                        {
                            return;
                        }
                        Transform injectorSlotDisplayRoot = ss2EquipmentIconHolder.icons[i].displayRoot.transform;

                        // base position is moved enough away from mul-t's other equipment slot
                        // the position is also manually set because the postition & localposition change at some point while loading for no reason
                        // even if i store the position after i've modified it it still gets randomly changed
                        //ss2EquipmentIconHolder.icons[i].transform.position = new Vector3(4.5f, -5.2f, 12.8f);

                        // set localposition of injector icon displayroot to the mul-t alt equipment icon displayroot
                        ss2EquipmentIconHolder.icons[i].transform.GetChild(0).position = MyHud.equipmentIcons[1].transform.GetChild(0).position;
                        RectTransform displayRootRect = ss2EquipmentIconHolder.icons[i].transform.GetChild(0).GetComponent<RectTransform>();
                        Vector3 localPositionChange = Vector3.zero;
                        localPositionChange.x = displayRootRect.rect.width * 3;
                        // divide by 10 as an int for the row number
                        localPositionChange.y -= 107 * (int)(i / 10);
                        if (i < 10)
                        {
                            // modulus 10 for the right x position regardless of row
                            localPositionChange.x += 100 * (i % 10);
                        }
                        else
                        {
                            // make the injector slots past the 10th start at the 5th slot's x coordinate
                            // this is to make it not overlap with the hp bar at giga ultrawide resolutions (i.e. 3840x1080)
                            localPositionChange.x += 100 * (((i - 11) % 10) + 5);
                        }
                        ss2EquipmentIconHolder.icons[i].transform.localPosition += localPositionChange;

                        // the isready panel is weirdly not aligned + injector slots never have cooldown text anyways so they're getting removed
                        Transform injectorSlotIsReadyPanel = injectorSlotDisplayRoot.GetChild(0);
                        injectorSlotIsReadyPanel.gameObject.SetActive(false);
                        // we can still use it to check if we've scaled the slot already or not
                        Vector3 alreadyScaled = new(1.1f, 1.1f, 1.1f);
                        if (injectorSlotIsReadyPanel.localScale != alreadyScaled)
                        {
                            HudStructure.ScaleEquipmentSlot(injectorSlotDisplayRoot, 1.1f);
                        }
                    }
                }

                internal static IEnumerator DelayColorInjectorSlots()
                {
                    yield return null;
                    ColorInjectorSlots();
                }
                [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
                internal static void ColorInjectorSlots()
                {
                    if (!ModIsRunning || !IsHudFinishedLoading)
                    {
                        return;
                    }
                    SS2.Items.CompositeInjector.IconHolder ss2EquipmentIconHolder;
                    bool compositeInjectorExists = MyHud.gameObject.TryGetComponent<SS2.Items.CompositeInjector.IconHolder>(out ss2EquipmentIconHolder);
                    if (!compositeInjectorExists)
                    {
                        return;
                    }

                    foreach (SS2.Items.CompositeInjector.EquipmentIconButEpic injectorSlot in ss2EquipmentIconHolder.icons)
                    {
                        if (!injectorSlot.displayRoot.activeSelf)
                        {
                            // doing return instead of continue since if we've hit an inactive slot that means the rest will also be inactive
                            return;
                        }

                        Transform injectorSlotDisplayRoot = injectorSlot.displayRoot.transform;

                        Transform injectorSlotIsReadyPanel = injectorSlotDisplayRoot.GetChild(0);
                        Image injectorSlotEquipmentIsReadyPanelImage = injectorSlotIsReadyPanel.GetComponent<Image>();
                        injectorSlotEquipmentIsReadyPanelImage.color = Main.Helpers.GetAdjustedColor(HudColor.SurvivorColor, colorIntensityMultiplier: HudColor.DefaultHudColorIntensity);

                        Transform injectorSlotBGPanel = injectorSlotDisplayRoot.Find("BGPanel");
                        Image injectorSlotBGPanelImage = injectorSlotBGPanel.GetComponent<Image>();
                        injectorSlotBGPanelImage.color = Main.Helpers.GetAdjustedColor(HudColor.SurvivorColor, colorIntensityMultiplier: HudColor.DefaultHudColorIntensity);
                    }
                }
            }
        }

        internal static class DriverMod
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
            internal static bool AllowDriverWeaponSlotCreation = false;
            internal static Transform DriverWeaponSlot
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



            internal static void OnSurvivorSpecificHudEditsFinished()
            {
                if (HudTargetBody?.baseNameToken != "ROB_DRIVER_BODY_NAME")
                {
                    TryDestroyDriverWeaponSlot();
                }
            }

            // once OnHudColorUpdate happens if the weapon slot is created it'll copy what we've edited and (almost) everything will be good
            // doing it in OnHudColorUpdate though since i want it to be a survivor specific thing that happens regardless if driver is blacklisted or not
            internal static void OnHudColorUpdate()
            {
                if (HudTargetBody == null || HudTargetBody.baseNameToken != "ROB_DRIVER_BODY_NAME")
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
                catch{}
                if (!IsHudUserBlacklisted)
                {
                    MyHud?.StartCoroutine(DelayEditWeaponSlotStructure());
                }
            }



            private static IEnumerator DelayEditWeaponSlotStructure()
            {
                yield return null;
                EditWeaponSlotStructure();
            }
            private static void EditWeaponSlotStructure()
            {
                if (!IsHudEditable)
                {
                    return;
                }
                Transform firstSkillSlotTextPanel = MyHud.skillIcons[0].transform.GetChild(5);
                Transform driverWeaponSlot = DriverWeaponSlot;
                if (driverWeaponSlot == null)
                {
                    Log.Debug("Driver weapon slot was null in EditWeaponSlotStructure");
                    return;
                }
                Transform displayRoot = DriverWeaponSlot.GetChild(1);
                Transform weaponSlotTextPanel = displayRoot.GetChild(6);
                Transform weaponChargeBar = displayRoot.GetChild(10);

                weaponSlotTextPanel.position = new Vector3(weaponSlotTextPanel.position.x, firstSkillSlotTextPanel.position.y, weaponSlotTextPanel.position.z);
                weaponChargeBar.localScale = new Vector3(0.44f, weaponChargeBar.localScale.y, weaponChargeBar.localScale.z);
                weaponChargeBar.localPosition = new Vector3(weaponChargeBar.localPosition.x, -0.5f, weaponChargeBar.localPosition.z);
            }

            internal static void TryDestroyDriverWeaponSlot()
            {
                Log.Debug("TryDestroyDriverWeaponSlot");
                try
                {
                    Destroy(MyHud.equipmentIcons[0].gameObject.transform.parent.Find("WeaponSlot").gameObject);
                }
                catch{};
            }
        }
    }
}