using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudChanges.HudColor;
using MiscFixes.Modules;

namespace CleanestHud.ModSupport
{
    // do NOT rename this to MystMod because the actual myst mod is called that for once
    internal class Myst
    {
        private static bool? _modexists;
        internal static bool ModIsRunning
        {
            get
            {
                _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(MystMod.MystPlugin.MODUID);
                return (bool)_modexists;
            }
        }

        private static bool IsHudCameraTargetMyst
        {
            get
            {
                return HudCameraTargetBody != null && HudCameraTargetBody?.baseNameToken == "JAVANGLE_MYST_BODY_NAME";
            }
        }

        private static Transform mystScaler;
        private static Transform normalScaler;
        private static Transform equipmentSlotDisplayRoot;
        private static Transform skillsPanel1;
        private static Transform skillsPanel2;


        internal static void HUD_OnDestroy(On.RoR2.UI.HUD.orig_OnDestroy orig, HUD self)
        {
            orig(self);
            // storing some HUD elements in the class to save on some .Find or .GetChild calls
            mystScaler = null;
            normalScaler = null;
            equipmentSlotDisplayRoot = null;
            skillsPanel1 = null;
            skillsPanel2 = null;
        }


        internal static void OnSurvivorSpecificHudEditsFinished()
        {
            if (HudCameraTargetBody == null)
            {
                return;
            }
            Transform mystHud = MyHud.mainContainer.transform.Find("MystHuD(Clone)");
            if (mystHud == null)
            {
                return;
            }
            mystScaler = mystHud.GetChild(0).GetChild(0);
            normalScaler = MyHudLocator.FindChild("SkillDisplayRoot");
            if (IsHudCameraTargetMyst)
            {
                ReplaceNormalScalerWithMystScaler();
                EditEquipmentSlotStructure();
                EditMystScalerDetails();
            }
            else
            {
                ReplaceMystScalerWithNormalScaler();
                return;
            }
        }



        private static void ReplaceNormalScalerWithMystScaler()
        {
            // not really "replacing" it, moreso hiding the original and putting myst's on top of it
            mystScaler.gameObject.SetActive(true);
            for (int i = 0; i < normalScaler.childCount; i++)
            {
                Transform child = normalScaler.GetChild(i);
                if (child.name.Contains("Skill") || child.name == "EquipmentSlot")
                {
                    child.gameObject.SetActive(false);
                }
            }
            // manually calling the event's method to call what it calls
            ConfigOptions_OnShowSkillKeybindsChanged();
            Transform mystCluster = mystScaler.parent;
            mystCluster.rotation = Quaternion.identity;
        }

        private static void ReplaceMystScalerWithNormalScaler()
        {
            mystScaler.gameObject.SetActive(false);
            for (int i = 0; i < normalScaler.childCount; i++)
            {
                Transform child = normalScaler.GetChild(i);
                //Log.Warning(child.name);
                if (child.name.Contains("Skill") || child.name == "EquipmentSlot")
                {
                    child.gameObject.SetActive(true);
                }
            }
            ResetNormalSprintAndInventoryRemindersStatus();
        }

        private static void EditEquipmentSlotStructure()
        {
            equipmentSlotDisplayRoot = mystScaler.Find("EquipmentSlot").GetChild(0);
            equipmentSlotDisplayRoot.localScale = new Vector3(0.8f, 0.785f, 0.8f);
            equipmentSlotDisplayRoot.localPosition = new Vector3(-28, 17.25f, 0);

            Transform equipmentSlotBGPanel = equipmentSlotDisplayRoot.Find("BGPanel");
            equipmentSlotBGPanel.localPosition = Vector3.zero;

            Transform equipmentTextBackgroundPanel = equipmentSlotDisplayRoot.Find("EquipmentTextBackgroundPanel");
            equipmentTextBackgroundPanel.localPosition = new Vector3(0, -34.0633f, 0);
        }

        private static void EditMystScalerDetails()
        {
            Vector3 newKeybindTextLocalScale = new(0.15f, 0.125f, 1);
            DisableNormalSprintAndInventoryReminders();

            Transform backgroundExtra = mystScaler.GetChild(1);
            backgroundExtra.gameObject.SetActive(false);


            skillsPanel1 = mystScaler.Find("Panel1 Default");
            for (int i = 0; i < skillsPanel1.childCount; i++)
            {
                Transform skillIcon = skillsPanel1.GetChild(i);
                Transform skillBackgroundPanel = skillIcon.Find("SkillBackgroundPanel");
                skillBackgroundPanel.localScale = newKeybindTextLocalScale;
                skillBackgroundPanel.DisableImageComponent();
            }
            skillsPanel2 = mystScaler.Find("Panel2 Extra");
            for (int i = 0; i < skillsPanel2.childCount; i++)
            {
                Transform skillIcon = skillsPanel2.GetChild(i);
                Transform skillBackgroundPanel = skillIcon.Find("SkillBackgroundPanel");
                skillBackgroundPanel.localScale = newKeybindTextLocalScale;
                skillBackgroundPanel.DisableImageComponent();
            }


            Transform equipmentTextBackgroundPanel = equipmentSlotDisplayRoot.Find("EquipmentTextBackgroundPanel");
            equipmentTextBackgroundPanel.localScale = new Vector3(0.2f, 0.15f, 1);// equipment key reminder needs to be even bigger
            equipmentTextBackgroundPanel.DisableImageComponent();


            Transform tooltipCluster = mystScaler.Find("TooltipCluster");
            Transform magiciteCluster = tooltipCluster.GetChild(2);
            Transform keyBackgroundPanel = magiciteCluster.GetChild(0);
            keyBackgroundPanel.localScale = new Vector3(0.225f, 0.225f, 1);
            keyBackgroundPanel.DisableImageComponent();


            // if he fixes the spelling it then it is what it is
            Transform resourceGauges = mystScaler.Find("Resourse Guages");

            Transform magicGauge = resourceGauges.GetChild(0);
            Transform magicTextBackground = magicGauge.GetChild(0);
            magicTextBackground.DisableImageComponent();
            Transform magicTextFrame = magicTextBackground.GetChild(0);
            magicTextFrame.gameObject.SetActive(false);


            Transform limitGauge = resourceGauges.GetChild(1);
            Transform limitTextBackground = limitGauge.GetChild(0);
            limitTextBackground.DisableImageComponent();
            Transform limitTextFrame = limitTextBackground.GetChild(0);
            limitTextFrame.gameObject.SetActive(false);
        }



        internal static void OnHudColorUpdate()
        {
            if (HudCameraTargetBody == null)
            {
                return;
            }
            Transform mystHud = MyHud.mainContainer.transform.Find("MystHuD(Clone)");
            if (mystHud == null)
            {
                return;
            }
            Transform mystScaler = mystHud.GetChild(0).GetChild(0);
            if (HudCameraTargetBody.baseNameToken != "JAVANGLE_MYST_BODY_NAME")
            {
                return;
            }

            ColorMystScaler();
        }
        private static void ColorMystScaler()


        {
            for (int i = 0; i < skillsPanel1.childCount; i++)
            {
                Transform skillIcon = skillsPanel1.GetChild(i);
                GameObject isReadyPanel = skillIcon.GetChild(0).gameObject;
                Image isReadyPanelImage = isReadyPanel.GetComponent<Image>();
                isReadyPanelImage.color = SurvivorColor;
            }


            for (int i = 0; i < skillsPanel2.childCount; i++)
            {
                Transform skillIcon = skillsPanel2.GetChild(i);
                GameObject isReadyPanel = skillIcon.Find("IsReadyPanel").gameObject;
                Image isReadyPanelImage = isReadyPanel.GetComponent<Image>();
                isReadyPanelImage.color = SurvivorColor;
            }

            Transform equipmentIsReadyPanel = equipmentSlotDisplayRoot.GetChild(0);
            Image equipmentIsReadyPanelImage = equipmentIsReadyPanel.GetComponent<Image>();
            equipmentIsReadyPanelImage.color = SurvivorColor;

            Transform equipmentBGPanel = equipmentSlotDisplayRoot.Find("BGPanel");
            Image equipmentBGPanelImage = equipmentBGPanel.GetComponent<Image>();
            equipmentBGPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);
        }

        internal static void ConfigOptions_OnShowSprintAndInventoryKeybindsChanged()
        {
            if (mystScaler == null || !IsHudCameraTargetMyst)
            {
                return;
            }
            DisableNormalSprintAndInventoryReminders();
            Transform tooltipCluster = mystScaler.GetChild(0);

            tooltipCluster.Find("SprintCluster").gameObject.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
            tooltipCluster.Find("InventoryCluster").gameObject.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
        }

        private static void DisableNormalSprintAndInventoryReminders()
        {
            Transform scaler = MyHudLocator.FindChild("SkillDisplayRoot");
            // keep the normal reminders hidden since we're always "replacing" them
            scaler.Find("SprintCluster").gameObject.SetActive(false);
            scaler.Find("InventoryCluster").gameObject.SetActive(false);
        }

        private static void ResetNormalSprintAndInventoryRemindersStatus()
        {
            Transform scaler = MyHudLocator.FindChild("SkillDisplayRoot");
            // keep the normal reminders hidden since we're always "replacing" them
            scaler.Find("SprintCluster").gameObject.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
            scaler.Find("InventoryCluster").gameObject.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
        }

        internal static void ConfigOptions_OnShowSkillKeybindsChanged()
        {
            if (mystScaler == null)
            {
                return;
            }
            MoveMystScaler();
            SetSkillAndEquipmentKeybindsStatus();
            MoveMystTooltipClusters();
        }
        private static void MoveMystScaler()
        {
            // lazy
            mystScaler.localPosition = new Vector3(
                -817,
                108 + (ConfigOptions.ShowSkillKeybinds.Value ? 20 : 0),
                -12.5f
            );
        }

        private static void SetSkillAndEquipmentKeybindsStatus()
        {
            skillsPanel1 = mystScaler.Find("Panel1 Default");
            for (int i = 0; i < skillsPanel1.childCount; i++)
            {
                Transform skillIcon = skillsPanel1.GetChild(i);
                Transform skillBackgroundPanel = skillIcon.Find("SkillBackgroundPanel");
                skillBackgroundPanel.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
            }
            skillsPanel2 = mystScaler.Find("Panel2 Extra");
            for (int i = 0; i < skillsPanel2.childCount; i++)
            {
                Transform skillIcon = skillsPanel2.GetChild(i);
                Transform skillBackgroundPanel = skillIcon.Find("SkillBackgroundPanel");
                skillBackgroundPanel.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
            }
            mystScaler.Find("EquipmentSlot").GetChild(0).Find("EquipmentTextBackgroundPanel").gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
        }

        // gotta manually fuckin reposition the tooltips because this is handled separately from the normal one because myst is quirky and has a copy of the hud for itself!!!!!
        private static void MoveMystTooltipClusters()
        {
            if (mystScaler == null)
            {
                return;
            }
            Transform tooltipCluster = mystScaler.GetChild(0);
            Transform sprintCluster = tooltipCluster.Find("SprintCluster");
            int extraYIfKeybindsShowing = (ConfigOptions.ShowSkillKeybinds.Value ? 0 : 20);

            tooltipCluster.Find("InventoryCluster").localPosition = new Vector3(
                -156,
                -71 + extraYIfKeybindsShowing,
                0
            );
            sprintCluster.localPosition = new Vector3(
                -105.5f,
                -71.5f + extraYIfKeybindsShowing,
                0
            );
            // GetChild(0) is KeyBackgroundPanel
            sprintCluster.GetChild(0).localPosition = new Vector3(
                -4,
                -34,
                0
            );
            tooltipCluster.Find("MagiciteCluster").localPosition = new Vector3(
                -62,
                -69 + extraYIfKeybindsShowing,
                0
            );
        }
    }
}