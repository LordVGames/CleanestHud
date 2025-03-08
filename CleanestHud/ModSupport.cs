using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using SS2;
using System.Collections;
using BepInEx.Configuration;

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
                    if (_modexists == null)
                    {
                        _modexists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(RiskOfOptions.PluginInfo.PLUGIN_GUID);
                    }
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
                        ConfigOptions.EnableAllyCardBackgrounds
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.EnableAutoScoreboardHighlight
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.EnableGradualDifficultyBarColor
                    )
                );
                ModSettingsManager.AddOption(
                    new CheckBoxOption(
                        ConfigOptions.EnableScoreboardItemHighlightColoring
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
                        ConfigOptions.EnableDebugLogging
                    )
                );
            }
        }

        internal static class Starstorm2
        {
            private static bool? _modexists;
            internal static bool ModIsRunning
            {
                get
                {
                    if (_modexists == null)
                    {
                        _modexists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SS2Main.GUID);
                    }
                    return (bool)_modexists;
                }
            }
            internal static int KnownInjectorSlotCount = -1;

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
            {
                if (!Main.IsHudFinishedLoading)
                {
                    Log.Debug("onBodyInventoryChangedGlobal happened while the HUD was not done loading, returning");
                    return;
                }

                int currentInjectorItemCount = characterBody.inventory.GetItemCount(SS2Content.Items.CompositeInjector);
                if (currentInjectorItemCount != KnownInjectorSlotCount)
                {
                    KnownInjectorSlotCount = currentInjectorItemCount;
                    Main.MyHud.StartCoroutine(CompositeInjectorSupport.DelayEditInjectorSlots());
                    Main.MyHud.StartCoroutine(CompositeInjectorSupport.DelayColorInjectorSlots());
                }
            }

            internal static class CompositeInjectorSupport
            {
                internal static IEnumerator DelayEditInjectorSlots()
                {
                    yield return null;
                    EditInjectorSlots();
                }
                [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
                internal static void EditInjectorSlots()
                {
                    if (!ModIsRunning || !Main.IsHudFinishedLoading)
                    {
                        return;
                    }
                    SS2.Items.CompositeInjector.IconHolder ss2EquipmentIconHolder;
                    bool compositeInjectorExists = Main.MyHud.gameObject.TryGetComponent<SS2.Items.CompositeInjector.IconHolder>(out ss2EquipmentIconHolder);
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
                        RectTransform injectorSlotDisplayRootRect = injectorSlotDisplayRoot.GetComponent<RectTransform>();
                        RectTransform injectorSlotRect = ss2EquipmentIconHolder.icons[i].GetComponent<RectTransform>();

                        // base position is moved enough away from mul-t's other equipment slot
                        // the position is also manually set because the postition & localposition change at some point while loading for no reason
                        // even if i store the position after i've modified it it still gets randomly changed
                        ss2EquipmentIconHolder.icons[i].transform.position = new Vector3(4.75f, -5.2f, 12.8f);
                        Vector3 localPositionChange = Vector3.zero;
                        // divide by 10 as an int for the row number and modulus 10 for the right x position regardless of row
                        localPositionChange.y -= 107 * (int)(i / 10);
                        localPositionChange.x += 100 * (i % 10);
                        ss2EquipmentIconHolder.icons[i].transform.localPosition += localPositionChange;

                        // the isready panel is weirdly not aligned + injector slots never have cooldown text anyways so they're getting removed
                        Transform injectorSlotIsReadyPanel = injectorSlotDisplayRoot.GetChild(0);
                        injectorSlotIsReadyPanel.gameObject.SetActive(false);
                        // we can still use it to check if we've scaled the slot already or not
                        Vector3 alreadyScaled = new Vector3(1.1f, 1.1f, 1.1f);
                        if (injectorSlotIsReadyPanel.localScale != alreadyScaled)
                        {
                            HudChanges.HudStructure.ScaleEquipmentSlot(injectorSlotDisplayRoot, 1.1f);
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
                    if (!ModIsRunning || !Main.IsHudFinishedLoading)
                    {
                        return;
                    }
                    SS2.Items.CompositeInjector.IconHolder ss2EquipmentIconHolder;
                    bool compositeInjectorExists = Main.MyHud.gameObject.TryGetComponent<SS2.Items.CompositeInjector.IconHolder>(out ss2EquipmentIconHolder);
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
                        injectorSlotEquipmentIsReadyPanelImage.color = Main.Helpers.GetAdjustedColor(HudChanges.HudColor.SurvivorColor, colorIntensityMultiplier: HudChanges.HudColor.DefaultHudColorIntensity);

                        Transform injectorSlotBGPanel = injectorSlotDisplayRoot.Find("BGPanel");
                        Image injectorSlotBGPanelImage = injectorSlotBGPanel.GetComponent<Image>();
                        injectorSlotBGPanelImage.color = Main.Helpers.GetAdjustedColor(HudChanges.HudColor.SurvivorColor, colorIntensityMultiplier: HudChanges.HudColor.DefaultHudColorIntensity);
                    }
                }
            }
        }

        internal static class LookingGlassMod
        {
            private static bool? _modexists;
            internal static bool ModIsRunning
            {
                get
                {
                    if (_modexists == null)
                    {
                        _modexists = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID);
                    }
                    return (bool)_modexists;
                }
            }

            internal static ConfigEntry<bool> StatsPanelEnabledConfig
            {
                get
                {
                    if (ModIsRunning)
                    {
                        return LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay;
                    }
                    return null;
                }
            }
            internal static void StatsPanelEnabledConfig_SettingChanged(object sender, EventArgs e)
            {
                if (!Main.IsHudFinishedLoading)
                {
                    return;
                }

                if (StatsPanelEnabledConfig.Value)
                {
                    Main.MyHud.StartCoroutine(DelayRemoveLookingGlassStatsPanelBackground());
                }
            }
            internal static IEnumerator DelayRemoveLookingGlassStatsPanelBackground()
            {
                yield return null;
                Transform upperRightCluster = Main.MyHud.gameModeUiRoot.transform;
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
        }
    }
}