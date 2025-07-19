using UnityEngine;
using BepInEx.Configuration;
using RoR2.UI;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using System;
using MiscFixes.Modules;

namespace CleanestHud
{
    public static class ConfigOptions
    {
        public static class SpecialConfig
        {
            public enum SeekerMeditateHudPosition
            {
                AboveHealthBar,
                OverCrosshair
            }
            public enum SeekerLotusHudPosition
            {
                AboveSkillsMiddle,
                LeftOfSkills
            }
        }



        public static ConfigEntry<bool> AllowHudStructureEdits;
        public static ConfigEntry<bool> AllowHudDetailsEdits;
        public static ConfigEntry<bool> AllowHudColorEdits;
        public static ConfigEntry<bool> AllowSurvivorSpecificEdits;

        public static ConfigEntry<float> HudTransparency;
        private static void HudTransparency_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            CanvasGroup wholeHudCanvasGroup = Main.MyHud.GetOrAddComponent<CanvasGroup>();
            wholeHudCanvasGroup.alpha = HudTransparency.Value;
            HudChanges.HudDetails.SetInspectPanelMaxAlpha();
        }


        public static ConfigEntry<bool> ShowSkillKeybinds;
        private static void ShowSkillKeybinds_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSkillsAndEquipmentReminderTextStatus();
            OnShowSkillKeybindsChanged?.Invoke();
            // the sprint and inventory reminders are moved up/down depending on if the skills reminders exist
            // so their y positions need to change with it to stay in the same spot
            HudChanges.HudStructure.RepositionSprintAndInventoryReminders();
            HudChanges.HudStructure.RepositionSkillScaler();
        }
        public static event Action OnShowSkillKeybindsChanged;


        public static ConfigEntry<bool> ShowSkillAndEquipmentOutlines;
        private static void ShowSkillAndEquipmentOutlines_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSkillOutlinesStatus();
            OnShowSkillAndEquipmentOutlinesChanged?.Invoke();
        }
        public static event Action OnShowSkillAndEquipmentOutlinesChanged;


        public static ConfigEntry<bool> ShowSprintAndInventoryKeybinds;
        private static void ShowSprintAndInventoryKeybinds_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSprintAndInventoryKeybindsStatus();
            OnShowSprintAndInventoryKeybindsChanged?.Invoke();
        }
        public static event Action OnShowSprintAndInventoryKeybindsChanged;


        public static ConfigEntry<bool> AllowInspectPanelFadeIn;
        private static void AllowInspectPanelFadeIn_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetInspectPanelFadeInStatus();
        }


        public static ConfigEntry<float> InspectPanelFadeInDuration;
        private static void InspectPanelFadeInDuration_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetInspectPanelFadeInStatus();
        }


        public static ConfigEntry<bool> AllowAllyCardBackgrounds;
        private static void AllowAllyCardBackgrounds_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            if (AllowAllyCardBackgrounds.Value)
            {
                HudChanges.HudColor.ColorAllAllyCardBackgrounds();
            }
            HudChanges.HudDetails.SetAllyCardBackgroundsStatus();
        }


        public static ConfigEntry<bool> AllowScoreboardLabels;
        private static void AllowScoreboardLabels_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            Transform scoreboardPanel = Main.MyHudLocator.FindChild("ScoreboardPanel");
            HudChanges.HudDetails.SetScoreboardLabelsActiveOrNot(scoreboardPanel);
        }


        public static ConfigEntry<bool> AllowScoreboardItemHighlightColoring;
        private static void AllowScoreboardItemHighlightColoring_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            ScoreboardController scoreboardController = Main.MyHudLocator.FindChild("ScoreboardPanel").GetComponent<ScoreboardController>();
            for (int i = 0; i < scoreboardController.stripAllocator.elements.Count; i++)
            {
                Color colorToUse;
                if (scoreboardController.stripAllocator.elements[i].userBody == null)
                {
                    Color longBackgroundColor = scoreboardController.stripAllocator.elements[i].transform.GetChild(0).GetComponent<Image>().color;
                    longBackgroundColor.a = 1;
                    colorToUse = longBackgroundColor;
                }
                else
                {
                    colorToUse = scoreboardController.stripAllocator.elements[i].userBody.bodyColor;
                }
                Main.MyHud?.StartCoroutine(HudChanges.HudColor.DelayColorItemIconHighlights(scoreboardController.stripAllocator.elements[i], colorToUse));
            }
        }


        public static ConfigEntry<bool> AllowAutoScoreboardHighlight;


        public static ConfigEntry<bool> EnableConsistentDifficultyBarBrightness;
        private static void EnableConsistentDifficultyBarBrightness_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetFakeInfiniteLastDifficultySegmentStatus();
            HudChanges.HudColor.ColorDifficultyBar();
        }


        public static ConfigEntry<bool> AllowSimulacrumWaveBarAnimating;
        private static void AllowSimulacrumWaveBarAnimating_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSimulacrumWaveBarAnimatorStatus();
        }




        public static ConfigEntry<bool> AllowVoidFiendMeterAnimating;
        private static void AllowVoidFiendMeterAnimating_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.SurvivorSpecific.VoidFiend.SetVoidFiendMeterAnimatorStatus();
        }


        public static ConfigEntry<SpecialConfig.SeekerMeditateHudPosition> SeekerMeditateHudPosition;
        private static void SeekerMeditateHudPosition_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            // this really isn't needed but i know someone would've noticed it eventually
            HudChanges.SurvivorSpecific.Seeker.RepositionSeekerMeditationUI();
        }


        public static ConfigEntry<SpecialConfig.SeekerLotusHudPosition> SeekerLotusHudPosition;
        private static void SeekerLotusUiPosition_SettingChanged(object sender, EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.SurvivorSpecific.Seeker.RepositionSeekerLotusUI();
        }


        public static ConfigEntry<string> BodyNameBlacklist_Config;
        private static void BodyNameBlacklist_Config_SettingChanged(object sender, EventArgs e)
        {
            BodyNameBlacklist_Array = BodyNameBlacklist_Config.Value.Split(',');
        }
        internal static string[] BodyNameBlacklist_Array;


        public static ConfigEntry<bool> AllowDebugLogging;




        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void BindConfigOptions(ConfigFile config)
        {
            AllowHudStructureEdits = config.BindOption<bool>(
                "HUD Settings",
                "Allow HUD structure edits",
                "Should the structure editing phase of the HUD loading be allowed to happen?\n\nNOTE: This option is REALLY not supported, it will likely cause problems! Especially with some already supported modded survivors! This will also only take effect next stage or the next time the HUD is created!",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowHudDetailsEdits = config.BindOption<bool>(
                "HUD Settings",
                "Allow HUD details edits",
                "Should the details editing phase of the HUD loading be allowed to happen?\n\nNOTE: This option is not very supported, it may not fully work! This will also only take effect next stage or the next time the HUD is created!",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowHudColorEdits = config.BindOption<bool>(
                "HUD Settings",
                "Allow HUD color edits",
                "Should the HUD be colored based on the survivor being played/spectated?\n\nNOTE: This option is not very supported, it may not fully work! This will also only take effect next stage or the next time the HUD is created!",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            HudTransparency = config.BindOptionSteppedSlider(
                "HUD Settings",
                "HUD Transparency",
                "How transparent should the entire HUD be?\n1 = 100% opaque (no transparency), 0.8 = 80% opaque (20% transparency)",
                0.8f,
                0.05f,
                0, 10,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            ShowSkillAndEquipmentOutlines = config.BindOption<bool>(
                "HUD Settings",
                "Show skill outlines",
                "Should skills have survivor-colored outlines?",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            ShowSkillKeybinds = config.BindOption<bool>(
                "HUD Settings",
                "Show skill & equipment keybinds",
                "Should the keybinds set for your 4 skills & equipment be displayed below your skills be shown?",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            ShowSprintAndInventoryKeybinds = config.BindOption<bool>(
                "HUD Settings",
                "Show sprint and inventory keybinds",
                "Should the keybinds set for sprinting and opening the inventory menu be shown?",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowInspectPanelFadeIn = config.BindOption<bool>(
                "HUD Settings",
                "Allow inspect panel fade-in",
                "Should the inspect panel do it's fade-in animation whenever an item is clicked?",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            InspectPanelFadeInDuration = config.BindOptionSteppedSlider(
                "HUD Settings",
                "Inspect panel fade-in duration",
                "Set a custom duration for the inspect panel's fade-in animation. Vanilla is 0.2",
                0.4f,
                0.05f,
                0, 5,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowAllyCardBackgrounds = config.BindOption<bool>(
                "HUD Settings",
                "Allow ally backgrounds",
                "Should the allies on the left side of the HUD have their backgrounds? If Allowd, the backgrounds will be properly colored.",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowScoreboardLabels = config.BindOption<bool>(
                "HUD Settings",
                "Allow inventories menu labels",
                "Should the player/items/equipment labels be visible on the inventories screen?",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowScoreboardItemHighlightColoring = config.BindOption<bool>(
                "HUD Settings",
                "Allow changing inventory item icon highlight colors",
                "Should the highlights for item icons in the TAB inventories menu be colored based on the survivor that has those items?",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowAutoScoreboardHighlight = config.BindOption<bool>(
                "HUD Settings",
                "Allow auto highlight when opening the inventory menu",
                "Should the automatic highlight for the first person in TAB inventories list be allowed?\n\nNOTE: Disabling this and hovering over something will cause movement to stop working!",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            EnableConsistentDifficultyBarBrightness = config.BindOption<bool>(
                "HUD Settings",
                "Enable consistent difficulty bar segment brightness",
                "Should the coloring for the difficulty bar stay at the same brightness instead of getting darker as the difficulty increases?",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowSimulacrumWaveBarAnimating = config.BindOption<bool>(
                "HUD Settings",
                "Allow simulacrum wave progress bar animations",
                "Should the progress bar on the simulacrum's wave UI be allowed to animate & squish around whenever enemies spawn/enemies are left?\nNOTE: Will cause the bar to become stuck squished after a second or 2 of an active wave.",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );



            AllowSurvivorSpecificEdits = config.BindOption<bool>(
                "HUD Settings - Survivor Specific",
                "Allow survivor-specific HUD edits",
                "Should the editing phase for survivor-specific HUD elements be allowed to happen? This should be turned off if HUD structure edits are turned off.\n\nNOTE: This option is REALLY not supported, it may cause problems! This will also only take effect next stage or the next time the HUD is created!",
                true,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            AllowVoidFiendMeterAnimating = config.BindOption<bool>(
                "HUD Settings - Survivor Specific",
                "Allow Void Fiend corruption meter animations",
                "Should Void Fiend's corruption meter be allowed to animate & squish around whenever the percentage changes?\nNOTE: Currently, when the animations disabled, the colors of the meter do not change based on your form.",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            SeekerMeditateHudPosition = config.BindOption<SpecialConfig.SeekerMeditateHudPosition>(
                "HUD Settings - Survivor Specific",
                "Seeker Meditate Minigame UI Position",
                "Choose the position for the UI of the minigame you do for Seeker's meditation.",
                SpecialConfig.SeekerMeditateHudPosition.OverCrosshair,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            SeekerLotusHudPosition = config.BindOption<SpecialConfig.SeekerLotusHudPosition>(
                "HUD Settings - Survivor Specific",
                "Seeker Lotus UI Position",
                "Choose the position for the lotus flower thing on the UI that shows your progress towards your 7th meditation.",
                SpecialConfig.SeekerLotusHudPosition.LeftOfSkills,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            BodyNameBlacklist_Config = config.BindOption<string>(
                "HUD Settings - Survivor Specific",
                "Survivor Blacklist",
                "If the HUD gets messed up when playing with certain survivors, add their BODY name (i.e. CommandoBody) here to stop the majority of the mod's hud changes when playing that survivor. Each body name needs separated by a comma and NO spaces.",
                "",
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );
            BodyNameBlacklist_Array = BodyNameBlacklist_Config.Value.Split(',');


            AllowDebugLogging = config.BindOption<bool>(
                "Other",
                "Allow debug logging",
                "Allow to do some extra debug logging that can help diagnose issues with the mod.",
                false,
                MiscFixes.Modules.Extensions.ConfigFlags.ClientSided
            );



            if (ModSupport.RiskOfOptionsMod.ModIsRunning)
            {
                HudTransparency.SettingChanged += HudTransparency_SettingChanged;
                ShowSkillAndEquipmentOutlines.SettingChanged += ShowSkillAndEquipmentOutlines_SettingChanged;
                ShowSkillKeybinds.SettingChanged += ShowSkillKeybinds_SettingChanged;
                ShowSprintAndInventoryKeybinds.SettingChanged += ShowSprintAndInventoryKeybinds_SettingChanged;
                AllowInspectPanelFadeIn.SettingChanged += AllowInspectPanelFadeIn_SettingChanged;
                InspectPanelFadeInDuration.SettingChanged += InspectPanelFadeInDuration_SettingChanged;
                AllowAllyCardBackgrounds.SettingChanged += AllowAllyCardBackgrounds_SettingChanged;
                AllowScoreboardItemHighlightColoring.SettingChanged += AllowScoreboardItemHighlightColoring_SettingChanged;
                EnableConsistentDifficultyBarBrightness.SettingChanged += EnableConsistentDifficultyBarBrightness_SettingChanged;
                AllowSimulacrumWaveBarAnimating.SettingChanged += AllowSimulacrumWaveBarAnimating_SettingChanged;

                AllowVoidFiendMeterAnimating.SettingChanged += AllowVoidFiendMeterAnimating_SettingChanged;
                SeekerMeditateHudPosition.SettingChanged += SeekerMeditateHudPosition_SettingChanged;
                SeekerLotusHudPosition.SettingChanged += SeekerLotusUiPosition_SettingChanged;
                BodyNameBlacklist_Config.SettingChanged += BodyNameBlacklist_Config_SettingChanged;

                ModSupport.RiskOfOptionsMod.SetupModCategoryInfo();
            }
            if (ModSupport.LookingGlassMod.ModIsRunning)
            {
                ModSupport.LookingGlassMod.HookStatsPanelConfig_SettingChanged();
            }
        }
    }
}