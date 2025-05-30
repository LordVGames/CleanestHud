using UnityEngine;
using BepInEx.Configuration;
using RoR2.UI;
using System.Runtime.CompilerServices;
using static CleanestHud.HudResources;
using UnityEngine.UI;

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
                AboveHealthBar,
                LeftOfSkills
            }
        }



        public static ConfigEntry<float> HudTransparency;
        private static void HudTransparency_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            CanvasGroup wholeHudCanvasGroup = Main.MyHud.GetComponent<CanvasGroup>() ?? Main.MyHud.gameObject.AddComponent<CanvasGroup>();
            wholeHudCanvasGroup.alpha = HudTransparency.Value;
            HudChanges.HudDetails.SetInspectPanelMaxAlpha();
        }


        public static ConfigEntry<bool> ShowSkillKeybinds;
        private static void ShowSkillKeybinds_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSkillsAndEquipmentReminderTextStatus();
            HudChanges.HudStructure.RepositionSkillScaler();
            // the sprint and inventory reminders are moved up/down depending on if the skills reminders exist
            // so their y positions need to change with it to stay in the same spot
            HudChanges.HudStructure.RepositionSprintAndInventoryReminders();
        }


        public static ConfigEntry<bool> ShowSkillAndEquipmentOutlines;
        private static void ShowSkillAndEquipmentOutlines_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSkillOutlinesStatus();
        }


        public static ConfigEntry<bool> ShowSprintAndInventoryKeybinds;
        private static void ShowSprintAndInventoryKeybinds_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSprintAndInventoryKeybindsStatus();
        }


        public static ConfigEntry<bool> AllowInspectPanelFadeIn;
        private static void AllowInspectPanelFadeIn_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetInspectPanelFadeInStatus();
        }


        public static ConfigEntry<float> InspectPanelFadeInDuration;
        private static void InspectPanelFadeInDuration_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetInspectPanelFadeInStatus();
        }


        public static ConfigEntry<bool> AllowAllyCardBackgrounds;
        private static void AllowAllyCardBackgrounds_SettingChanged(object sender, System.EventArgs e)
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
        private static void AllowScoreboardLabels_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            Transform scoreboardPanel = ImportantHudTransforms.SpringCanvas.Find("ScoreboardPanel");
            HudChanges.HudDetails.SetScoreboardLabelsActiveOrNot(scoreboardPanel);
        }


        public static ConfigEntry<bool> AllowScoreboardItemHighlightColoring;
        private static void AllowScoreboardItemHighlightColoring_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            ScoreboardController scoreboardController = ImportantHudTransforms.SpringCanvas.Find("ScoreboardPanel").GetComponent<ScoreboardController>();
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
                Main.MyHud.StartCoroutine(HudChanges.HudColor.DelayColorItemIconHighlights(scoreboardController.stripAllocator.elements[i], colorToUse));
            }
        }


        public static ConfigEntry<bool> AllowAutoScoreboardHighlight;


        public static ConfigEntry<bool> AllowConsistentDifficultyBarColor;
        private static void AllowConsistentDifficultyBarColor_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetFakeInfiniteLastDifficultySegmentStatus();
            HudChanges.HudColor.ColorDifficultyBar();
        }


        public static ConfigEntry<bool> AllowSimulacrumWaveBarAnimating;
        private static void AllowSimulacrumWaveBarAnimating_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.HudDetails.SetSimulacrumWaveBarAnimatorStatus();
        }




        public static ConfigEntry<bool> AllowVoidFiendMeterAnimating;
        private static void AllowVoidFiendMeterAnimating_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.SurvivorSpecific.VoidFiend.SetVoidFiendMeterAnimatorStatus();
        }


        public static ConfigEntry<SpecialConfig.SeekerMeditateHudPosition> SeekerMeditateHudPosition;
        private static void SeekerMeditateHudPosition_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            // this really isn't needed but i know someone would've noticed it eventually
            HudChanges.SurvivorSpecific.Seeker.RepositionSeekerMeditationUI();
        }


        public static ConfigEntry<SpecialConfig.SeekerLotusHudPosition> SeekerLotusHudPosition;
        private static void SeekerLotusUiPosition_SettingChanged(object sender, System.EventArgs e)
        {
            if (!Main.IsHudEditable)
            {
                return;
            }

            HudChanges.SurvivorSpecific.Seeker.RepositionSeekerLotusUI();
        }


        public static ConfigEntry<string> BodyNameBlacklist_Config;
        private static void BodyNameBlacklist_Config_SettingChanged(object sender, System.EventArgs e)
        {
            BodyNameBlacklist_Array = BodyNameBlacklist_Config.Value.Split(',');
        }
        internal static string[] BodyNameBlacklist_Array;


        public static ConfigEntry<bool> AllowDebugLogging;


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void BindConfigOptions(ConfigFile config)
        {
            HudTransparency = config.Bind<float>(
                "HUD Settings",
                "HUD Transparency",
                0.8f,
                "How transparent should the entire HUD be?\n1 = 100% opaque (no transparency), 0.8 = 80% opaque (20% transparency)"
            );
            ShowSkillAndEquipmentOutlines = config.Bind<bool>(
                "HUD Settings",
                "Show skill outlines",
                true,
                "Should skills have survivor-colored outlines?"
            );
            ShowSkillKeybinds = config.Bind<bool>(
                "HUD Settings",
                "Show skill & equipment keybinds",
                false,
                "Should the keybinds set for your 4 skills & equipment be displayed below your skills be shown?"
            );
            ShowSprintAndInventoryKeybinds = config.Bind<bool>(
                "HUD Settings",
                "Show sprint and inventory keybinds",
                false,
                "Should the keybinds set for sprinting and opening the inventory menu be shown?"
            );
            AllowInspectPanelFadeIn = config.Bind<bool>(
                "HUD Settings",
                "Allow inspect panel fade-in",
                false,
                "Should the inspect panel do it's fade-in animation whenever an item is clicked?"
            );
            InspectPanelFadeInDuration = config.Bind<float>(
                "HUD Settings",
                "Inspect panel fade-in duration",
                0.4f,
                "Set a custom duration for the inspect panel's fade-in animation. Vanilla is 0.2"
            );
            AllowAllyCardBackgrounds = config.Bind<bool>(
                "HUD Settings",
                "Allow ally backgrounds",
                true,
                "Should the allies on the left side of the HUD have their backgrounds? If Allowd, the backgrounds will be properly colored."
            );
            AllowScoreboardLabels = config.Bind<bool>(
                "HUD Settings",
                "Allow inventories menu labels",
                false,
                "Should the player/items/equipment labels be visible on the inventories screen?"
            );
            AllowScoreboardItemHighlightColoring = config.Bind<bool>(
                "HUD Settings",
                "Allow changing inventory item icon highlight colors",
                true,
                "Should the highlights for item icons in the TAB inventories menu be colored based on the survivor that has those items?"
            );
            AllowAutoScoreboardHighlight = config.Bind<bool>(
                "HUD Settings",
                "Allow auto highlight when opening the inventory menu",
                false,
                "Should the automatic highlight for the first person in TAB inventories list be allowed?\n\nNOTE: Disabling this and hovering over something will cause movement to stop working!"
            );
            AllowConsistentDifficultyBarColor = config.Bind<bool>(
                "HUD Settings",
                "Allow consistent difficulty bar segment colors",
                true,
                "Should the coloring for the difficulty bar stay the same instead of getting darker as the difficulty increases?"
            );
            AllowSimulacrumWaveBarAnimating = config.Bind<bool>(
                "HUD Settings",
                "Allow simulacrum wave progress bar animations",
                false,
                "Should the progress bar on the simulacrum's wave UI be allowed to animate & squish around whenever enemies spawn/enemies are left?\nNOTE: Will cause the bar to become stuck squished after a second or 2 of an active wave."
            );

            AllowVoidFiendMeterAnimating = config.Bind<bool>(
                "HUD Settings - Survivor Specific",
                "Allow Void Fiend corruption meter animations",
                false,
                "Should Void Fiend's corruption meter be allowed to animate & squish around whenever the percentage changes?\nNOTE: Currently, when the animations disabled, the colors of the meter do not change based on your form."
            );
            SeekerMeditateHudPosition = config.Bind<SpecialConfig.SeekerMeditateHudPosition>(
                "HUD Settings - Survivor Specific",
                "Seeker Meditate Minigame UI Position",
                SpecialConfig.SeekerMeditateHudPosition.OverCrosshair,
                "Choose the position for the UI of the minigame you do for Seeker's meditation."
            );
            SeekerLotusHudPosition = config.Bind<SpecialConfig.SeekerLotusHudPosition>(
                "HUD Settings - Survivor Specific",
                "Seeker Lotus UI Position",
                SpecialConfig.SeekerLotusHudPosition.LeftOfSkills,
                "Choose the position for the lotus flower thing on the UI that shows your progress towards your 7th meditation."
            );
            BodyNameBlacklist_Config = config.Bind<string>(
                "HUD Settings - Survivor Specific",
                "Survivor Blacklist",
                "",
                "If the HUD gets messed up when playing with certain survivors, add their BODY name (i.e. CommandoBody) here to stop the mod's hud changes when playing that survivor. Each body name needs separated by a comma and NO spaces."
            );
            BodyNameBlacklist_Array = BodyNameBlacklist_Config.Value.Split(',');


            AllowDebugLogging = config.Bind<bool>(
                "Other",
                "Allow debug logging",
                false,
                "Allow to do some extra debug logging that can help diagnose issues with the mod."
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
                AllowConsistentDifficultyBarColor.SettingChanged += AllowConsistentDifficultyBarColor_SettingChanged;
                AllowSimulacrumWaveBarAnimating.SettingChanged += AllowSimulacrumWaveBarAnimating_SettingChanged;

                AllowVoidFiendMeterAnimating.SettingChanged += AllowVoidFiendMeterAnimating_SettingChanged;
                SeekerMeditateHudPosition.SettingChanged += SeekerMeditateHudPosition_SettingChanged;
                SeekerLotusHudPosition.SettingChanged += SeekerLotusUiPosition_SettingChanged;
                BodyNameBlacklist_Config.SettingChanged += BodyNameBlacklist_Config_SettingChanged;

                ModSupport.RiskOfOptionsMod.AddOptions();
            }
            if (ModSupport.LookingGlassMod.ModIsRunning)
            {
                ModSupport.LookingGlassMod.HookStatsPanelConfig_SettingChanged();
            }
        }
    }
}