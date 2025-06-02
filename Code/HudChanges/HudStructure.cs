using System;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
using static CleanestHud.Main;
using static CleanestHud.HudResources;

namespace CleanestHud.HudChanges
{
    public class HudStructure
    {
        public const float SkillSlotSpacing = 105;
        public static event Action OnHudStructureEditsFinished;



        internal static class AssetEdits
        {
            internal static void LiveEditHudElementPrefabs()
            {
                EditScoreboardStripAsset();
            }
            internal static void EditScoreboardStripAsset()
            {
                Transform scoreboardStrip = HudAssets.ScoreboardStrip.transform;

                Transform longBackground = scoreboardStrip.GetChild(0);
                Image longBackgroundImage = longBackground.GetComponent<Image>();
                longBackgroundImage.sprite = HudAssets.WhiteSprite;

                Transform totalTextContainer = longBackground.Find("TotalTextContainer");
                Transform moneyText = totalTextContainer.Find("MoneyText");
                HGTextMeshProUGUI moneyTextMesh = moneyText.GetComponent<HGTextMeshProUGUI>();
                moneyTextMesh.color = Color.white;

                Transform nameLabel = longBackground.GetChild(2);
                HGTextMeshProUGUI nameLabelText = nameLabel.GetComponent<HGTextMeshProUGUI>();
                nameLabelText.alignment = TextAlignmentOptions.Center;
                // i would reposition parts of the asset here but that never works ingame lmao
            }
        }



        internal static void EditHudStructure()
        {
            Log.Debug("EditHudStructure");
            if (IsGameModeSimulacrum)
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
            EditHealthBar();
            EditLevelDisplayClusterAndElements();
            EditSpectatorLabel();
            EditSkillSlots();
            EditEquipmentSlots();
            EditCurrenciesSection();
            EditItemInventoryDisplay();
            EditBossHpBarAndText();
            EditNotificationArea();
            EditScoreboardPanel();
            EditSkillsScaler();
            RepositionSprintAndInventoryReminders();
            OnHudStructureEditsFinished?.Invoke();
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

            HudEditorComponents.SimulacrumBarEditor defaultFillBarRootBarPositioner = defaultFillBarRoot.gameObject.AddComponent<HudEditorComponents.SimulacrumBarEditor>();
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
        private static void EditDifficultyBarSegments()
        {
            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            Transform scrollView = difficultyBar.Find("Scroll View");
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
        private static void EditBarRoots()
        {
            if (!ImportantHudTransforms.BarRoots)
            {
                Main.Helpers.LogMissingHudVariable("EditBarRoots", "BarRoots", "HudStructure");
                return;
            }
            if (!MyHudLocator.FindChild("BottomCenterCluster"))
            {
                Main.Helpers.LogMissingHudVariable("EditBarRoots", "BottomCenterCluster");
                return;
            }

            ImportantHudTransforms.BarRoots.SetParent(MyHudLocator.FindChild("BottomCenterCluster"));
            Destroy(ImportantHudTransforms.BarRoots.GetComponent<VerticalLayoutGroup>());
            RectTransform barRootsRect = ImportantHudTransforms.BarRoots.GetComponent<RectTransform>();
            barRootsRect.rotation = Quaternion.identity;
            barRootsRect.pivot = new Vector2(0.5f, 0.25f);
            barRootsRect.anchoredPosition = Vector2.zero;
            barRootsRect.sizeDelta = new Vector2(-400f, 100f);
        }
        private static void EditLevelDisplayClusterAndElements()
        {
            if (!ImportantHudTransforms.BarRoots)
            {
                Main.Helpers.LogMissingHudVariable("EditBarRootsElements", "BarRoots", "HudStructure");
                return;
            }

            Transform levelDisplayCluster = ImportantHudTransforms.BarRoots.GetChild(0);
            RectTransform levelDisplayClusterRect = levelDisplayCluster.GetComponent<RectTransform>();
            levelDisplayClusterRect.localPosition = new Vector3(-300f, 45f, 0f);

            // calling other edit methods within this one to save on some GetChild calls
            // also the localPositions set by these are fine and are always cenetered so DO NOT mess with them
            EditBuffDisplay(levelDisplayCluster);
            EditLevelDisplayRoot(levelDisplayCluster);
            EditExpBar(levelDisplayCluster);
        }
        private static void EditLevelDisplayRoot(Transform levelDisplayCluster)
        {
            // levelDisplayCluster.GetChild(0)
            Transform levelDisplayRoot = levelDisplayCluster.Find("LevelDisplayRoot");

            RectTransform levelDisplayRootRect = levelDisplayRoot.GetComponent<RectTransform>();
            levelDisplayRootRect.pivot = new Vector2(0.5f, 0.5f);
            levelDisplayRootRect.localPosition = new Vector3(311, -27f, 0);
        }
        private static void EditBuffDisplay(Transform levelDisplayCluster)
        {
            Transform buffDisplayRoot = MyHudLocator.FindChild("BuffDisplayRoot");

            Destroy(buffDisplayRoot.GetComponent<HorizontalLayoutGroup>());
            buffDisplayRoot.SetParent(ImportantHudTransforms.BarRoots);

            RectTransform buffDisplayRootRect = buffDisplayRoot.GetComponent<RectTransform>();
            buffDisplayRootRect.localPosition = new Vector3(-25f, -45f, 0f);
        }
        // yes it's called "exp" not "xp" ingame
        private static void EditExpBar(Transform levelDisplayCluster)
        {
            Transform expBarRoot = levelDisplayCluster.Find("ExpBarRoot");

            Image expBarRootImage = expBarRoot.GetComponent<Image>();
            expBarRootImage.sprite = HudAssets.WhiteSprite;
            expBarRootImage.color = Color.clear;

            RectTransform expBarRootRect = expBarRoot.GetComponent<RectTransform>();
            expBarRootRect.pivot = new Vector2(0.5f, 0.5f);
            expBarRootRect.sizeDelta = new Vector2(0, expBarRootRect.sizeDelta.y);
            expBarRootRect.localScale = new Vector3(1.002f, 1, 1);

            Transform shrunkenExpBarRoot = expBarRoot.GetChild(0);
            RectTransform shrunkenExpBarRootRect = shrunkenExpBarRoot.GetComponent<RectTransform>();
            shrunkenExpBarRootRect.localScale = new Vector3(1f, 1.66f, 1f);
        }
        private static void EditHealthBar()
        {
            if (!ImportantHudTransforms.BarRoots)
            {
                Main.Helpers.LogMissingHudVariable("EditHealthBar", "BarRoots", "HudStructure");
                return;
            }
            Transform healthBarRoot = ImportantHudTransforms.BarRoots.GetChild(1);

            Image healthBarRootImage = healthBarRoot.GetComponent<Image>();
            healthBarRootImage.sprite = HudAssets.WhiteSprite;

            Transform shrunkenRoot = healthBarRoot.Find("ShrunkenRoot");
            if (shrunkenRoot.childCount != 0)
            {
                Transform child1 = shrunkenRoot.GetChild(0);
                child1.gameObject.SetActive(false);
                Transform child2 = shrunkenRoot.GetChild(1);
                child2.gameObject.SetActive(false);
            }
        }
        private static void EditSpectatorLabel()
        {
            Transform bottomRightCluster = MyHudLocator.FindChild("BottomRightCluster");
            Transform bottomCenterCluster = MyHudLocator.FindChild("BottomCenterCluster");
            if (bottomRightCluster == null)
            {
                Main.Helpers.LogMissingHudVariable("EditSpectatorLabel", "BottomRightCluster");
                return;
            }
            if (bottomCenterCluster == null)
            {
                Main.Helpers.LogMissingHudVariable("EditSpectatorLabel", "BottomCenterCluster");
                return;
            }

            Transform spectatorLabel = bottomCenterCluster.Find("SpectatorLabel");
            RectTransform spectatorLabelRect = spectatorLabel.GetComponent<RectTransform>();
            spectatorLabelRect.anchoredPosition = new Vector2(0f, 150f);

            GraphicRaycaster bottomRightGraphicRaycaster = bottomRightCluster.GetComponent<GraphicRaycaster>();
            GraphicRaycaster bottomCenterGraphicRaycaster = bottomCenterCluster.gameObject.AddComponent<GraphicRaycaster>();
            bottomCenterGraphicRaycaster.blockingObjects = bottomRightGraphicRaycaster.blockingObjects;
            bottomCenterGraphicRaycaster.ignoreReversedGraphics = bottomRightGraphicRaycaster.ignoreReversedGraphics;
            bottomCenterGraphicRaycaster.useGUILayout = bottomRightGraphicRaycaster.useGUILayout;
        }
        private static void EditSkillSlots()
        {
            // not doing normal for loop so i don't have "MyHud.skillIcons[i]" everywhere
            Vector3 newSkillPosition = Vector3.zero;
            int i = 0;
            Vector3 centerSkillIconLocalPosition = MyHud.skillIcons[2].transform.localPosition;
            foreach (SkillIcon skillIcon in MyHud.skillIcons)
            {
                newSkillPosition = skillIcon.transform.localPosition;
                switch(i)
                {
                    case 0:
                        newSkillPosition.x = (centerSkillIconLocalPosition.x - (SkillSlotSpacing * 2));
                        break;
                    case 1:
                        newSkillPosition.x = (centerSkillIconLocalPosition.x - SkillSlotSpacing);
                        break;
                    case 2:
                        break;
                    case 3:
                        newSkillPosition.x = centerSkillIconLocalPosition.x + SkillSlotSpacing;
                        break;
                }
                skillIcon.transform.localPosition = newSkillPosition;
                i++;


                skillIcon.cooldownRemapPanel = null;

                GameObject cooldownPanel = skillIcon.transform.Find("CooldownPanel").gameObject;
                cooldownPanel.SetActive(false);

                Transform cooldownText = skillIcon.transform.Find("CooldownText");

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

                Transform skillStockRoot = skillIcon.transform.Find($"Skill{i}StockRoot");
                Transform skillStockRootText = skillStockRoot.Find("StockText");
                HGTextMeshProUGUI skillStockRootTextMesh = skillStockRootText.GetComponent<HGTextMeshProUGUI>();
                skillStockRootTextMesh.color = Color.white;
            }
        }
        private static void EditEquipmentSlots()
        {
            for (int i = 0; i < MyHud.equipmentIcons.Length; i++)
            {
                Vector3 equipmentDisplayRootRectLocalPosition = Vector3.zero;
                float equipmentSlotScaleFactor = 1;
                switch (i)
                {
                    case 0:
                        equipmentDisplayRootRectLocalPosition = new Vector3(-20.75f, 16.95f, 0f);
                        equipmentSlotScaleFactor = 0.928f;
                        break;
                    case 1:
                        equipmentDisplayRootRectLocalPosition = new Vector3(-20, -5f, 0f);
                        // smaller by default, we don't have to scale any more/less unless we want it to be even smaller
                        equipmentSlotScaleFactor = 1f;
                        break;
                }



                EquipmentIcon equipment = MyHud.equipmentIcons[i];
                Transform equipmentDisplayRoot = equipment.displayRoot.transform;

                RectTransform equipmentDisplayRootRect = equipmentDisplayRoot.GetComponent<RectTransform>();
                equipmentDisplayRootRect.localPosition = equipmentDisplayRootRectLocalPosition;

                Transform equipmentCooldownText = equipmentDisplayRoot.Find("CooldownText");
                RectTransform equipmentCooldownTextRect = equipmentCooldownText.GetComponent<RectTransform>();
                equipmentCooldownTextRect.localPosition = new Vector3(0f, 1f, 0f);

                ScaleEquipmentSlot(equipmentDisplayRoot, equipmentSlotScaleFactor);

            }

            RectTransform firstSkillIconTextBackgroundPanelRect = MyHud.skillIcons[0].transform.Find("SkillBackgroundPanel").GetComponent<RectTransform>();
            GameObject equipmentTextBackgroundPanel = MyHud.equipmentIcons[0].displayRoot.transform.Find("EquipmentTextBackgroundPanel").gameObject;
            RectTransform equipmentTextBackgroundPanelRect = equipmentTextBackgroundPanel.GetComponent<RectTransform>();
            equipmentTextBackgroundPanelRect.position = new Vector3(
                equipmentTextBackgroundPanelRect.position.x,
                firstSkillIconTextBackgroundPanelRect.position.y,
                equipmentTextBackgroundPanelRect.position.z
            );
            equipmentTextBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);

            // ss2 doesn't copy our styled alt equipment slot for some reason
            // i think it's because ss2 copies the equipment slot immediately while we wait a lil bit before doing our changes
            // which means we need to modify ALL composite injector equipment slots
            if (ModSupport.Starstorm2.ModIsRunning)
            {
                MyHud?.StartCoroutine(ModSupport.Starstorm2.CompositeInjectorSupport.DelayEditInjectorSlots());
            }
        }
        // A tiny bit of scaling still happens even when the scaleFactor is just 1
        internal static void ScaleEquipmentSlot(Transform equipmentDisplayRoot, float scaleFactor)
        {
            float extraFactor = 0.0448f;
            RectTransform equipmentDisplayRootRect = equipmentDisplayRoot.GetComponent<RectTransform>();
            Transform equipmentBGPanel = equipmentDisplayRoot.Find("BGPanel");
            RectTransform equipmentBGPanelRect = equipmentBGPanel.GetComponent<RectTransform>();
            Transform equipmentIconPanel = equipmentDisplayRoot.Find("IconPanel");
            RectTransform equipmentIconPanelRect = equipmentIconPanel.GetComponent<RectTransform>();
            Transform equipmentIsReadyPanel = equipmentDisplayRoot.Find("IsReadyPanel");

            equipmentDisplayRootRect.localScale = (new Vector3(1f, 0.98f, 1f) * scaleFactor);
            equipmentBGPanelRect.localScale  = new Vector3(scaleFactor + extraFactor, scaleFactor + extraFactor, scaleFactor);
            // why is the bg panel moved slightly? now we gotta move it back
            equipmentBGPanel.localPosition = Vector3.zero;
            equipmentIconPanelRect.localScale *= scaleFactor;
            equipmentIsReadyPanel.localScale *= scaleFactor;
        }
        private static void EditSkillsScaler()
        {
            Transform scaler = MyHudLocator.FindChild("SkillDisplayRoot");
            scaler.SetParent(MyHudLocator.FindChild("BottomCenterCluster"));
            RectTransform scalerRect = scaler.GetComponent<RectTransform>();
            scalerRect.rotation = Quaternion.identity;
            scalerRect.pivot = new Vector2(0.0875f, 0);
            scalerRect.sizeDelta = new Vector2(scalerRect.sizeDelta.x, -234f);
            scalerRect.localPosition = SkillsScalerLocalPosition;
        }
        private static void EditCurrenciesSection()
        {
            Transform upperLeftCluster = MyHud.moneyText.transform.parent;

            Image upperLeftClusterImage = upperLeftCluster.GetComponent<Image>();
            upperLeftClusterImage.enabled = false;

            VerticalLayoutGroup upperLeftClusterVerticalLayoutGroup = upperLeftCluster.GetComponent<VerticalLayoutGroup>();
            upperLeftClusterVerticalLayoutGroup.spacing = 0;



            // MyHud.moneyText.transform is also moneyRoot
            Transform valueText = MyHud.moneyText.transform.Find("ValueText");
            HGTextMeshProUGUI valueTextMesh = valueText.GetComponent<HGTextMeshProUGUI>();
            valueTextMesh.color = Color.white;

            Transform dollarSign = MyHud.moneyText.transform.Find("DollarSign");
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
            Transform itemInventoryDisplayRoot = MyHud.itemInventoryDisplay.transform.parent;
            RectTransform itemInventoryDisplayRootRect = itemInventoryDisplayRoot.GetComponent<RectTransform>();
            // make it a lil smaller since it goes to the edges of the hud on ultrawide otherwise
            itemInventoryDisplayRootRect.sizeDelta = new Vector2(itemInventoryDisplayRootRect.sizeDelta.x * 0.75f, itemInventoryDisplayRootRect.sizeDelta.y);
        }
        private static void EditBossHpBarAndText()
        {
            // TopCenterCluster
            Destroy(MyHudLocator.FindChild(0).GetComponent<VerticalLayoutGroup>());

            Transform bossHealthBarRoot = MyHudLocator.FindChild(0).Find("BossHealthBarRoot");
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

            // make the colored bars completely cover the background
            Vector3 newFillPanelScale = new(1.02f, 1, 1);

            Transform delayFillPanel = bossBackgroundPanel.Find("DelayFillPanel");
            delayFillPanel.localScale = newFillPanelScale;
            Image delayFillPanelImage = delayFillPanel.GetComponent<Image>();
            delayFillPanelImage.color = new Color32(138, 0, 0, 255);

            Transform fillPanel = bossBackgroundPanel.GetChild(1);
            fillPanel.localScale = newFillPanelScale;

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
            Transform notificationArea = MyHudLocator.FindChild("NotificationArea");
            RectTransform notificationAreaRect = notificationArea.GetComponent<RectTransform>();
            notificationAreaRect.localEulerAngles = new Vector3(0f, 6f, 0f);
            // local position in game is boosted by +576.2813 X and +54 Y, Z is unaffected
            notificationAreaRect.localPosition = new Vector3(-26, -334, 0f);
            notificationAreaRect.anchorMin = new Vector2(0.8f, 0.05f);
            notificationAreaRect.anchorMax = new Vector2(0.8f, 0.05f);
        }
        private static void EditScoreboardPanel()
        {
            Transform scoreboardPanel = MyHudLocator.FindChild("ScoreboardPanel");
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
        }



        internal static void RepositionSprintAndInventoryReminders()
        {
            Transform scaler = MyHudLocator.FindChild("SkillDisplayRoot");
            Transform sprintCluster = scaler.Find("SprintCluster");
            RectTransform sprintClusterRect = sprintCluster.GetComponent<RectTransform>();
            Transform inventoryCluster = scaler.Find("InventoryCluster");
            RectTransform inventoryClusterRect = inventoryCluster.GetComponent<RectTransform>();
            Transform hpBarRoot = ImportantHudTransforms.BarRoots.GetChild(1);

            // these raise up when skill keybinds are enabled for some reason
            float localYPosition;
            if (ConfigOptions.ShowSkillKeybinds.Value)
            {
                localYPosition = -106;
            }
            else
            {
                localYPosition = -64;
            }

            sprintCluster.position = new Vector3(
                hpBarRoot.position.x,
                sprintCluster.position.y,
                sprintCluster.position.z
            );
            sprintCluster.localPosition = new Vector3(
                sprintCluster.localPosition.x - sprintClusterRect.rect.width * 2,
                localYPosition,
                0
            );

            inventoryCluster.position = new Vector3(
                hpBarRoot.position.x,
                inventoryCluster.position.y,
                inventoryCluster.position.z
            );
            inventoryCluster.localPosition = new Vector3(
                inventoryCluster.localPosition.x - inventoryClusterRect.rect.width * 4,
                localYPosition,
                0
            );
        }


        internal static void EditScoreboardStrip(ScoreboardStrip scoreboardStrip)
        {
            // more space between icons, similar spacing to inventory at the top of the screen
            scoreboardStrip.itemInventoryDisplay.itemIconPrefabWidth = 58;
            scoreboardStrip.itemInventoryDisplay.maxHeight = 54;
            AttachEditorToScoreboardStrip(scoreboardStrip.transform);
        }
        private static void AttachEditorToScoreboardStrip(Transform scoreboardStripTransform)
        {
            HudEditorComponents.ScoreboardStripEditor scoreboardStripEditor = scoreboardStripTransform.gameObject.AddComponent<HudEditorComponents.ScoreboardStripEditor>();

            // ClassBackground's local position is handled by the component
            scoreboardStripEditor.ClassBackgroundRect_LocalScale = Vector3.one * 1.075f;
            scoreboardStripEditor.ClassBackgroundRect_Pivot = new Vector2(0.5f, 0.5f);

            // the entirety of itemsbackground is handled by the component

            // EquipmentBackground's local position is also handled by the component
            scoreboardStripEditor.EquipmentBackgroundRect_LocalScale = Vector3.one * 1.1f;
            scoreboardStripEditor.EquipmentBackgroundRect_Pivot = new Vector2(0.5f, 0.5f);

            // name label positioning is well yknow
        }



        internal static void MoveSpectatorLabel()
        {
            if (!IsHudEditable)
            {
                return;
            }
            Transform spectatorLabel = MyHudLocator.FindChild("BottomCenterCluster").Find("SpectatorLabel");
            if (spectatorLabel == null)
            {
                return;
            }

            spectatorLabel.localPosition = new Vector3(0, 700, 0);
        }

        

        internal static void RepositionHudElementsBasedOnWidth()
        {
            if (!IsHudEditable)
            {
                return;
            }

            Transform itemInventoryDisplayRoot = MyHud.itemInventoryDisplay.transform.parent;
            RectTransform itemInventoryDisplayRootRect = itemInventoryDisplayRoot.GetComponent<RectTransform>();
            itemInventoryDisplayRootRect.localPosition = new Vector3(
                // 0 is always at the center of the whole screen, and the inventory's left edge is placed at 0
                // we remove half the size delta to move it over left by half it's size to get it perfectly centered no matter the game width
                0 - (itemInventoryDisplayRootRect.sizeDelta.x / 2),
                itemInventoryDisplayRootRect.localPosition.y,
                itemInventoryDisplayRootRect.localPosition.z
            );

            Transform healthbarRoot = ImportantHudTransforms.BarRoots.GetChild(1);
            RectTransform healthbarRootRect = healthbarRoot.GetComponent<RectTransform>();
            healthbarRootRect.localPosition = new Vector3(
                0 - (healthbarRootRect.sizeDelta.x / 2),
                45,
                healthbarRootRect.localPosition.z
            );



            Transform levelDisplayCluster = ImportantHudTransforms.BarRoots.GetChild(0);

            Transform expBarRoot = levelDisplayCluster.GetChild(1);
            RectTransform expBarRootRect = expBarRoot.GetComponent<RectTransform>();
            // gotta do this one in 2 parts
            expBarRootRect.position = new Vector3(
                0,
                expBarRootRect.position.y,
                expBarRootRect.position.z
            );
            expBarRootRect.localPosition = new Vector3(
                expBarRootRect.localPosition.x,
                -4.2f,
                0
            );

            RepositionSprintAndInventoryReminders();
        }



        internal static void RepositionSkillScaler()
        {
            RectTransform scalerRect = MyHudLocator.FindChild("SkillDisplayRoot").GetComponent<RectTransform>();
            scalerRect.localPosition = SkillsScalerLocalPosition;
        }
    }
}