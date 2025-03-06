using System;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
using static CleanestHud.HudResources;

namespace CleanestHud.HudChanges
{
    internal class HudStructure
    {
        internal static class AssetEdits
        {
            internal static void EditHudElementPrefabs()
            {
                EditScoreboardStripAsset();
            }
            private static void EditScoreboardStripAsset()
            {
                Transform scoreboardStrip = HudAssets.ScoreboardStrip.transform;

                Transform longBackground = scoreboardStrip.Find("LongBackground");
                Image longBackgroundImage = longBackground.GetComponent<Image>();
                longBackgroundImage.sprite = HudAssets.WhiteSprite;

                Transform nameLabel = longBackground.Find("NameLabel");
                RectTransform nameLabelRect = nameLabel.GetComponent<RectTransform>();
                nameLabelRect.pivot = new Vector2(0.5f, 0.5f);
                nameLabelRect.localPosition = new Vector3(-320f, 0f, 0f);

                Transform totalTextContainer = longBackground.Find("TotalTextContainer");
                Transform moneyText = totalTextContainer.Find("MoneyText");
                HGTextMeshProUGUI moneyTextMesh = moneyText.GetComponent<HGTextMeshProUGUI>();
                moneyTextMesh.color = Color.white;
            }
        }


        internal static void EditHudStructure()
        {
            if (Main.IsGameModeSimulacrum)
            {
                // the wave panel along with a few other things are within the whole wave ui transform
                EditSimulacrumDefaultWaveUI();
                EditSimulacrumWavePanel();
            }
            else
            {
                EditTimerPanel();
                EditDifficultyBarSegments();
            }
            EditBarRoots();
            EditBarRootsElements();
            EditHpBar();
            EditSpectatorLabel();
            EditSkillSlots();
            EditEquipmentSlots();
            RepositionSkillScaler();
            EditCurrenciesSection();
            EditItemInventoryDisplay();
            EditBossHpBarAndText();
            EditNotificationArea();
            RepositionSprintAndInventoryReminders();

            CanvasGroup wholeHudCanvasGroup = Main.MyHud.GetComponent<CanvasGroup>() ?? Main.MyHud.gameObject.AddComponent<CanvasGroup>();
            wholeHudCanvasGroup.alpha = ConfigOptions.HudTransparency.Value;
        }
        private static void EditSimulacrumDefaultWaveUI()
        {
            Transform defaultWaveUI = ImportantHudTransforms.RunInfoHudPanel.Find("DefaultWaveUI");
            if (!defaultWaveUI)
            {
                Main.Helpers.LogMissingHudVariable("EditSimulacrumDefaultWaveUI", "defaultWaveUI", "HudStructure");
                return;
            }
            Transform defaultFillBarRoot = defaultWaveUI.Find("FillBarRoot");

            Components.SimulacrumBarEditor defaultFillBarRootBarPositioner = defaultFillBarRoot.gameObject.AddComponent<Components.SimulacrumBarEditor>();
            defaultFillBarRootBarPositioner.idealLocalPosition = new Vector3(-120f, -21.15f, 0f);
            defaultFillBarRootBarPositioner.idealLocalScale = new Vector3(0.725f, 1f, 1f); ;

            Transform defaultRemainingEnemies = defaultWaveUI.Find("RemainingEnemiesRoot");
            Transform defaultRemainingEnemiesTitle = defaultRemainingEnemies.Find("RemainingEnemiesTitle");
            TextMeshProUGUI defaultRemainingEnemiesTitleMesh = defaultRemainingEnemiesTitle.GetComponent<TextMeshProUGUI>();
            defaultRemainingEnemiesTitleMesh.color = Color.white;

            Transform defaultRemainingEnemiesCounter = defaultRemainingEnemies.Find("RemainingEnemiesCounter");
            TextMeshProUGUI defaultRemainingEnemiesCounterMesh = defaultRemainingEnemiesCounter.GetComponent<TextMeshProUGUI>();
            defaultRemainingEnemiesCounterMesh.color = Color.white;
        }
        private static void EditSimulacrumWavePanel()
        {
            Transform wavePanel = ImportantHudTransforms.RunInfoHudPanel.Find("WavePanel");
            if (!wavePanel)
            {
                Main.Helpers.LogMissingHudVariable("EditSimulacrumWavePanel", "wavePanel", "HudStructure");
                return;
            }

            Transform waveText = wavePanel.Find("WaveText");
            HGTextMeshProUGUI waveTextMesh = waveText.GetComponent<HGTextMeshProUGUI>();
            waveTextMesh.color = Color.white;
        }
        private static void EditTimerPanel()
        {
            Transform timerPanel = ImportantHudTransforms.RunInfoHudPanel.Find("TimerPanel");
            if (!timerPanel)
            {
                Main.Helpers.LogMissingHudVariable("EditTimerPanel", "timerPanel", "HudStructure");
                return;
            }

            Transform timerText = timerPanel.Find("TimerText");
            HGTextMeshProUGUI timerTextMesh = timerText.GetComponent<HGTextMeshProUGUI>();
            timerTextMesh.color = Color.white;
        }
        private static void EditBarRoots()
        {
            if (!ImportantHudTransforms.BarRoots)
            {
                Main.Helpers.LogMissingHudVariable("EditBarRoots", "BarRoots", "HudStructure");
                return;
            }
            if (!ImportantHudTransforms.BottomCenterCluster)
            {
                Main.Helpers.LogMissingHudVariable("EditBarRoots", "BottomCenterCluster");
                return;
            }

            ImportantHudTransforms.BarRoots.parent = ImportantHudTransforms.BottomCenterCluster;
            Destroy(ImportantHudTransforms.BarRoots.GetComponent<VerticalLayoutGroup>());
            RectTransform barRootsRect = ImportantHudTransforms.BarRoots.GetComponent<RectTransform>();
            barRootsRect.rotation = Quaternion.identity;
            barRootsRect.pivot = new Vector2(0.5f, 0.25f);
            barRootsRect.anchoredPosition = Vector2.zero;
            barRootsRect.sizeDelta = new Vector2(-400f, 100f);
        }
        // the elements under BarRoots is the level display, XP bar and buffs/debuffs display
        private static void EditBarRootsElements()
        {
            if (!ImportantHudTransforms.BarRoots)
            {
                Main.Helpers.LogMissingHudVariable("EditBarRootsElements", "BarRoots", "HudStructure");
                return;
            }

            #region Finding transforms
            Transform levelDisplayCluster = ImportantHudTransforms.BarRoots.Find("LevelDisplayCluster");
            Transform levelDisplayRoot = levelDisplayCluster.Find("LevelDisplayRoot");
            Transform buffDisplayRoot = levelDisplayCluster.Find("BuffDisplayRoot");
            Transform expBarRoot = levelDisplayCluster.Find("ExpBarRoot");
            #endregion

            RectTransform levelDisplayClusterRect = levelDisplayCluster.GetComponent<RectTransform>();
            levelDisplayClusterRect.localPosition = new Vector3(-300f, 45f, 0f);
            Destroy(buffDisplayRoot.GetComponent<HorizontalLayoutGroup>());
            buffDisplayRoot.parent = ImportantHudTransforms.BarRoots;

            RectTransform buffDisplayRootRect = buffDisplayRoot.GetComponent<RectTransform>();
            buffDisplayRootRect.localPosition = new Vector3(-25f, -45f, 0f);

            RectTransform levelDisplayRootRect = levelDisplayRoot.GetComponent<RectTransform>();
            levelDisplayRootRect.pivot = new Vector2(0.5f, 0.5f);
            levelDisplayRootRect.localPosition = new Vector3(308, -23.5f, 0);

            Image expBarRootImage = expBarRoot.GetComponent<Image>();
            expBarRootImage.sprite = HudAssets.WhiteSprite;
            expBarRootImage.enabled = false;

            RectTransform expBarRootRect = expBarRoot.GetComponent<RectTransform>();
            expBarRootRect.localPosition = new Vector3(510.3f, -12.6f, 0f);
            expBarRoot.localScale = new Vector3(1.244f, 0.8f, 1f);

            Transform shrunkenExpBarRoot = expBarRoot.Find("ShrunkenRoot");
            RectTransform shrunkenExpBarRootRect = shrunkenExpBarRoot.GetComponent<RectTransform>();
            shrunkenExpBarRootRect.localScale = new Vector3(1f, 1.6666666666f, 1f);
            shrunkenExpBarRootRect.localPosition = new Vector3(-338.6f, 15.25f);
        }
        private static void EditHpBar()
        {
            if (!ImportantHudTransforms.BarRoots)
            {
                Main.Helpers.LogMissingHudVariable("EditHpBar", "BarRoots", "HudStructure");
                return;
            }
            Transform healthbarRoot = ImportantHudTransforms.BarRoots.Find("HealthbarRoot");

            RectTransform healthbarRootRect = healthbarRoot.GetComponent<RectTransform>();
            healthbarRootRect.localPosition = new Vector3(-210f, 45f, 0f);

            Image healthbarRootImage = healthbarRoot.GetComponent<Image>();
            healthbarRootImage.sprite = HudAssets.WhiteSprite;

            Transform shrunkenRoot = healthbarRoot.Find("ShrunkenRoot");
            if (shrunkenRoot.childCount != 0)
            {
                Transform child1 = shrunkenRoot.GetChild(0);
                child1.gameObject.SetActive(false);
                Transform child2 = shrunkenRoot.GetChild(1);
                child2.gameObject.SetActive(false);
            }
            else
            {
                // i better see no one take this log out of context lmao
                Log.Error("HP BAR SHRUNKENROOT DID NOT HAVE CHILDREN WHEN IT WAS SUPPOSED TO!!!!!");
            }
        }
        private static void EditSpectatorLabel()
        {
            if (!ImportantHudTransforms.BottomRightCluster)
            {
                Main.Helpers.LogMissingHudVariable("EditSpectatorLabel", "BottomRightCluster", "HudStructure");
                return;
            }
            if (!ImportantHudTransforms.BottomCenterCluster)
            {
                Main.Helpers.LogMissingHudVariable("EditSpectatorLabel", "BottomCenterCluster");
                return;
            }

            Transform spectatorLabel = ImportantHudTransforms.BottomCenterCluster.Find("SpectatorLabel");
            RectTransform spectatorLabelRect = spectatorLabel.GetComponent<RectTransform>();
            spectatorLabelRect.anchoredPosition = new Vector2(0f, 150f);

            GraphicRaycaster bottomRightGraphicRaycaster = ImportantHudTransforms.BottomRightCluster.GetComponent<GraphicRaycaster>();
            GraphicRaycaster bottomCenterGraphicRaycaster = ImportantHudTransforms.BottomCenterCluster.gameObject.AddComponent<GraphicRaycaster>();
            bottomCenterGraphicRaycaster.blockingObjects = bottomRightGraphicRaycaster.blockingObjects;
            bottomCenterGraphicRaycaster.ignoreReversedGraphics = bottomRightGraphicRaycaster.ignoreReversedGraphics;
            bottomCenterGraphicRaycaster.useGUILayout = bottomRightGraphicRaycaster.useGUILayout;
        }
        private static void EditSkillSlots()
        {
            foreach (SkillIcon skillIcon in Main.MyHud.skillIcons)
            {
                Transform cooldownText = skillIcon.transform.Find("CooldownText");

                skillIcon.cooldownRemapPanel = null;

                GameObject cooldownPanel = skillIcon.transform.Find("CooldownPanel").gameObject;
                cooldownPanel.SetActive(false);

                RectTransform cooldownTextRect = cooldownText.GetComponent<RectTransform>();
                cooldownTextRect.localPosition = new Vector3(-18f, 19.5f, 0f);

                HGTextMeshProUGUI cooldownTextMesh = cooldownText.GetComponent<HGTextMeshProUGUI>();
                cooldownTextMesh.color = Color.white;

                Image iconPanel = skillIcon.iconImage;
                RectTransform iconPanelRect = iconPanel.GetComponent<RectTransform>();
                iconPanelRect.localScale = Vector3.one * 1.1f;

                RectTransform isReadyPanelRect = skillIcon.isReadyPanelObject.GetComponent<RectTransform>();
                isReadyPanelRect.localScale *= 1.1f;

                GameObject skillBackgroundPanel = skillIcon.transform.Find("SkillBackgroundPanel").gameObject;
                skillBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);

                Transform skillStockRoot = skillIcon.transform.GetChild(4);
                Transform skillStockRootText = skillStockRoot.Find("StockText");
                HGTextMeshProUGUI skillStockRootTextMesh = skillStockRootText.GetComponent<HGTextMeshProUGUI>();
                skillStockRootTextMesh.color = Color.white;
            }
        }
        private static void EditEquipmentSlots()
        {
            for (int i = 0; i < Main.MyHud.equipmentIcons.Length; i++)
            {
                Vector3 equipmentDisplayRootRectLocalPosition = Vector3.zero;
                float equipmentSlotScaleFactor = 1;
                switch (i)
                {
                    case 0:
                        equipmentDisplayRootRectLocalPosition = new Vector3(-20f, 17f, 0f);
                        equipmentSlotScaleFactor = 0.92f;
                        break;
                    case 1:
                        equipmentDisplayRootRectLocalPosition = new Vector3(-10f, -5f, 0f);
                        // smaller by default, we don't have to scale any more/less unless we want it to be even smaller
                        equipmentSlotScaleFactor = 1f;
                        break;
                }



                EquipmentIcon equipment = Main.MyHud.equipmentIcons[i];
                Transform equipmentDisplayRoot = equipment.displayRoot.transform;

                RectTransform equipmentDisplayRootRect = equipmentDisplayRoot.GetComponent<RectTransform>();
                equipmentDisplayRootRect.localPosition = equipmentDisplayRootRectLocalPosition;

                Transform equipmentCooldownText = equipmentDisplayRoot.Find("CooldownText");
                RectTransform equipmentCooldownTextRect = equipmentCooldownText.GetComponent<RectTransform>();
                equipmentCooldownTextRect.localPosition = new Vector3(0f, 1f, 0f);

                ScaleEquipmentSlot(equipmentDisplayRoot, equipmentSlotScaleFactor);

                // the equipment keybind text below all the skill keybind texts because ?????
                // so it needs to be re-aligned manually
                GameObject equipmentTextBackgroundPanel = equipmentDisplayRoot.Find("EquipmentTextBackgroundPanel").gameObject;
                RectTransform equipmentTextBackgroundPanelRect = equipmentTextBackgroundPanel.GetComponent<RectTransform>();
                equipmentTextBackgroundPanelRect.localPosition = new Vector3(0, -31, 0);
                equipmentTextBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
            }

            // ss2 doesn't copy our styled alt equipment slot for some reason
            // i think it's because ss2 copies the equipment slot immediately while we wait a lil bit before doing our changes
            // which means we need to modify ALL composite injector equipment slots
            if (ModSupport.Starstorm2.ModIsRunning)
            {
                Main.MyHud.StartCoroutine(ModSupport.Starstorm2.CompositeInjectorSupport.DelayEditInjectorSlots());
            }
        }
        // A tiny bit of scaling still happens even when the scaleFactor is just 1
        internal static void ScaleEquipmentSlot(Transform equipmentDisplayRoot, float scaleFactor)
        {
            RectTransform equipmentDisplayRootRect = equipmentDisplayRoot.GetComponent<RectTransform>();
            Transform equipmentBGPanel = equipmentDisplayRoot.Find("BGPanel");
            RectTransform equipmentBGPanelRect = equipmentBGPanel.GetComponent<RectTransform>();
            Transform equipmentIconPanel = equipmentDisplayRoot.Find("IconPanel");
            RectTransform equipmentIconPanelRect = equipmentIconPanel.GetComponent<RectTransform>();
            Transform equipmentIsReadyPanel = equipmentDisplayRoot.Find("IsReadyPanel");

            equipmentDisplayRootRect.localScale = (new Vector3(1f, 0.99f, 1f) * scaleFactor);
            equipmentBGPanelRect.localScale *= scaleFactor;
            equipmentIconPanelRect.localScale *= scaleFactor;
            equipmentIsReadyPanel.localScale *= scaleFactor;
        }
        internal static void RepositionSkillScaler()
        {
            ImportantHudTransforms.SkillsScaler.parent = ImportantHudTransforms.BottomCenterCluster;

            RectTransform scalerRect = ImportantHudTransforms.SkillsScaler.GetComponent<RectTransform>();
            scalerRect.rotation = Quaternion.identity;
            scalerRect.pivot = new Vector2(0.5f, 0f);
            scalerRect.sizeDelta = new Vector2(-639f, -234f);
            float skillsHudYPos = ConfigOptions.ShowSkillKeybinds.Value ? 125f : 98f;
            scalerRect.anchoredPosition = new Vector2(62f, skillsHudYPos);
        }
        private static void EditCurrenciesSection()
        {
            Transform upperLeftCluster = Main.MyHud.moneyText.transform.parent;
            Image upperLeftClusterImage = upperLeftCluster.GetComponent<Image>();
            upperLeftClusterImage.enabled = false;
            VerticalLayoutGroup upperLeftClusterVerticalLayoutGroup = upperLeftCluster.GetComponent<VerticalLayoutGroup>();
            upperLeftClusterVerticalLayoutGroup.spacing = 0;


            // Main.MyHud.moneyText.transform is also moneyRoot
            Transform valueText = Main.MyHud.moneyText.transform.Find("ValueText");
            HGTextMeshProUGUI valueTextMesh = valueText.GetComponent<HGTextMeshProUGUI>();
            valueTextMesh.color = Color.white;
            Transform dollarSign = Main.MyHud.moneyText.transform.Find("DollarSign");
            HGTextMeshProUGUI dollarSignMesh = dollarSign.GetComponent<HGTextMeshProUGUI>();
            dollarSignMesh.color = Color.white;


            Transform lunarCoinRoot = upperLeftCluster.Find("LunarCoinRoot");
            Transform lunarCoinValueText = lunarCoinRoot.Find("ValueText");
            HGTextMeshProUGUI lunarCoinValueTextMesh = lunarCoinValueText.GetComponent<HGTextMeshProUGUI>();
            lunarCoinValueTextMesh.color = Color.white;
            Transform lunarCoinSign = lunarCoinRoot.Find("LunarCoinSign");
            HGTextMeshProUGUI lunarCoinSignMesh = lunarCoinSign.GetComponent<HGTextMeshProUGUI>();
            lunarCoinSignMesh.color = Color.white;

            // void coins aren't used in vanilla, but wolfo's simulacrum mod makes use of them
            Transform voidCoinRoot = upperLeftCluster.Find("VoidCoinRoot");
            Transform voidCoinValueText = voidCoinRoot.Find("ValueText");
            HGTextMeshProUGUI voidCoinValueTextMesh = voidCoinValueText.GetComponent<HGTextMeshProUGUI>();
            voidCoinValueTextMesh.color = Color.white;
            Transform voidCoinSign = voidCoinRoot.Find("VoidCoinSign");
            HGTextMeshProUGUI voidCoinSignMesh = voidCoinSign.GetComponent<HGTextMeshProUGUI>();
            voidCoinSignMesh.color = Color.white;
        }
        private static void EditItemInventoryDisplay()
        {
            Transform itemInventoryDisplayRoot = Main.MyHud.itemInventoryDisplay.transform.parent;
            RectTransform itemInventoryDisplayRootRect = itemInventoryDisplayRoot.GetComponent<RectTransform>();
            itemInventoryDisplayRootRect.localPosition = new Vector3(-26f, -90f, 0f);
            itemInventoryDisplayRootRect.anchoredPosition = new Vector2(550f, -90f);
            itemInventoryDisplayRootRect.pivot = new Vector2(0.5f, 0.5f);
            itemInventoryDisplayRootRect.sizeDelta = new Vector2(1000f, 140f);
            // goal is to have 18 icons per row instead of 20
        }
        private static void EditBossHpBarAndText()
        {
            Destroy(ImportantHudTransforms.TopCenterCluster.GetComponent<VerticalLayoutGroup>());

            Transform bossHealthBarRoot = ImportantHudTransforms.TopCenterCluster.Find("BossHealthBarRoot");
            Transform bossHealthBarRootRect = bossHealthBarRoot.GetComponent<RectTransform>();
            bossHealthBarRootRect.localPosition = new Vector3(0f, -160, -3.8f);
            Destroy(bossHealthBarRoot.GetComponent<VerticalLayoutGroup>());

            Transform bossContainer = bossHealthBarRoot.Find("Container");
            Destroy(bossContainer.GetComponent<VerticalLayoutGroup>());

            Transform bossHealthBarContainer = bossContainer.Find("BossHealthBarContainer");
            RectTransform bossHealthBarContainerRect = bossHealthBarContainer.GetComponent<RectTransform>();
            bossHealthBarContainerRect.localPosition = new Vector3(0, 25, 0);

            Transform bossNameLabel = bossContainer.Find("BossNameLabel");
            RectTransform bossNameLabelRect = bossNameLabel.GetComponent<RectTransform>();
            bossNameLabelRect.localPosition = new Vector3(0f, 32.5f, 0f);

            Transform bossSubtitleLabel = bossContainer.Find("BossSubtitleLabel");
            RectTransform bossSubtitleLabelRect = bossSubtitleLabel.GetComponent<RectTransform>();
            bossSubtitleLabelRect.localPosition = new Vector3(0f, -40f, 0f);

            HGTextMeshProUGUI bossSubtitleLabelMesh = bossSubtitleLabel.GetComponent<HGTextMeshProUGUI>();
            bossSubtitleLabelMesh.color = Color.white;

            Transform bossBackgroundPanel = bossHealthBarContainer.Find("BackgroundPanel");
            RectTransform bossBackgroundPanelRect = bossBackgroundPanel.GetComponent<RectTransform>();
            bossBackgroundPanelRect.localPosition = new Vector3(0f, -42.5f, 0f);
            bossBackgroundPanelRect.localScale = new Vector3(1f, 1.5f, 1f);


            Transform delayFillPanel = bossBackgroundPanel.Find("DelayFillPanel");
            Image delayFillPanelImage = delayFillPanel.GetComponent<Image>();
            delayFillPanelImage.color = new Color32(138, 0, 0, 255); // red color for boss hp bar

            Transform shieldPanel = bossBackgroundPanel.Find("ShieldPanel");
            Transform healthText = bossBackgroundPanel.Find("HealthText");
            RectTransform healthTextRect = healthText.GetComponent<RectTransform>();
            healthTextRect.localPosition = new Vector3(0f, 11.5f, -5.85840205f);
            healthTextRect.localEulerAngles = Vector3.zero;
            healthTextRect.localScale = new Vector3(1.25f, 0.8325f, 1.25f);

            int shieldPanelSiblingIndex = shieldPanel.GetSiblingIndex();
            int healthTextSiblingIndex = healthText.GetSiblingIndex();
            healthText.SetSiblingIndex(shieldPanelSiblingIndex);
            shieldPanel.SetSiblingIndex(healthTextSiblingIndex);
        }
        private static void EditNotificationArea()
        {
            Transform notificationArea = ImportantHudTransforms.MainContainer.Find("NotificationArea");
            RectTransform notificationAreaRect = notificationArea.GetComponent<RectTransform>();
            notificationAreaRect.localEulerAngles = new Vector3(0f, 6f, 0f);
            // local position in game is boosted by +576.2813 X and +54 Y, Z is unaffected
            notificationAreaRect.localPosition = new Vector3(-26, -334, 0f);
            notificationAreaRect.anchorMin = new Vector2(0.8f, 0.05f);
            notificationAreaRect.anchorMax = new Vector2(0.8f, 0.05f);
        }
        internal static void RepositionSprintAndInventoryReminders()
        {
            Transform scaler = ImportantHudTransforms.BottomCenterCluster.GetChild(2);
            Transform sprintCluster = scaler.Find("SprintCluster");
            Transform inventoryCluster = scaler.Find("InventoryCluster");

            Vector3 sprintClusterPosition = new Vector3(267.5f, -114, 0);
            Vector3 inventoryClusterPosition = new Vector3(335, -114, 0);
            if (!ConfigOptions.ShowSkillKeybinds.Value)
            {
                sprintClusterPosition.y = -72;
                inventoryClusterPosition.y = -72;
            }
            sprintCluster.localPosition = sprintClusterPosition;
            inventoryCluster.localPosition = inventoryClusterPosition;
        }
        private static void EditDifficultyBarSegments()
        {
            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            Transform scrollView = difficultyBar.Find("Scroll View");
            Transform backdrop = scrollView.Find("Backdrop");
            Transform viewport = scrollView.Find("Viewport");
            Transform content = viewport.Find("Content");

            for (int i = 0; i < content.childCount; i++)
            {
                Transform difficultyBarSegment = content.GetChild(i);

                Image segmentImage = difficultyBarSegment.GetComponent<Image>();
                segmentImage.preserveAspect = true;

                RectTransform segmentRect = difficultyBarSegment.GetComponent<RectTransform>();
                segmentRect.localScale = new Vector3(1f, 1.1f, 1f);
            }
        }



        internal static void EditScoreboardPanel()
        {
            Transform scoreboardPanel = ImportantHudTransforms.SpringCanvas.Find("ScoreboardPanel");
            Transform container = Helpers.GetContainerFromScoreboardPanel(scoreboardPanel);
            Transform stripContainer = container.GetChild(1);

            RectTransform scoreboardPanelRect = scoreboardPanel.GetComponent<RectTransform>();
            scoreboardPanelRect.localPosition = new Vector3(0, -90, 0);

            RectTransform stripContainerRect = stripContainer.GetComponent<RectTransform>();
            stripContainerRect.sizeDelta = new Vector2(0f, 80f);

            VerticalLayoutGroup stripContainerVerticalLayoutGroup = stripContainer.GetComponent<VerticalLayoutGroup>();
            stripContainerVerticalLayoutGroup.childForceExpandHeight = true;
            stripContainerVerticalLayoutGroup.childForceExpandWidth = true;
            stripContainerVerticalLayoutGroup.childControlHeight = true;
            stripContainerVerticalLayoutGroup.childControlWidth = true;
            stripContainerVerticalLayoutGroup.childScaleHeight = true;
            stripContainerVerticalLayoutGroup.childScaleWidth = true;

            for (int i = 0; i < stripContainer.childCount; i++)
            {
                Transform scoreboardStripTransform = stripContainer.GetChild(i);
                if (scoreboardStripTransform.gameObject.GetComponent<Components.ScoreboardStripEditor>() == null)
                {
                    AttachEditorToScoreboardStrip(scoreboardStripTransform);
                }
            }
        }
        private static void AttachEditorToScoreboardStrip(Transform scoreboardStripTransform)
        {
            Components.ScoreboardStripEditor scoreboardStripEditor = scoreboardStripTransform.gameObject.AddComponent<Components.ScoreboardStripEditor>();

            scoreboardStripEditor.ClassBackgroundRectLocalPosition = new Vector3(-477f, -0.2f, 0f);
            scoreboardStripEditor.ClassBackgroundRectLocalScale = Vector3.one * 1.075f;
            scoreboardStripEditor.ClassBackgroundRectPivot = new Vector2(0.5f, 0.5f);

            scoreboardStripEditor.ItemsBackgroundRectLocalPosition = new Vector3(120f, 9.5f, 0f);
            scoreboardStripEditor.ItemsBackgroundRectPivot = new Vector2(0.5f, 0.5f);

            scoreboardStripEditor.EquipmentBackgroundRectLocalPosition = new Vector3(475f, 1f, 0f);
            scoreboardStripEditor.EquipmentBackgroundRectLocalScale = Vector3.one * 1.1f;
            scoreboardStripEditor.EquipmentBackgroundRectPivot = new Vector2(0.5f, 0.5f);
        }
    }
}