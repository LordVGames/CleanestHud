using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudChanges.HudEditorComponents;
using MiscFixes.Modules;

namespace CleanestHud.HudChanges
{
    public class HudDetails
    {
        public static event Action OnHudDetailEditsFinished;



        internal static class AssetEdits
        {
            internal static void EditHudElementPrefabDetails()
            {
                EditItemIcon();
                RemoveGameEndPanelDetails();
                RemoveScoreboardStripAssetDetails();
                RemoveMoonDetonationPanelDetails();
                RemoveChatBoxDetails();
                HudAssets.StatStripTemplate.DisableImageComponent();
            }
            private static void EditItemIcon()
            {
                Transform navFocusHighlightTransform = HudAssets.ItemIconPrefab.transform.GetChild(1);

                RawImage navFocusHighlightRawImage = navFocusHighlightTransform.GetComponent<RawImage>();
                navFocusHighlightRawImage.texture = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");

                HGButton button = HudAssets.ItemIconPrefab.GetComponent<HGButton>();
                // setting the button colors to white makes it not influence glowimage color with yellow (the default)
                // we can always just manually color it back to yellow later anyways
                button.m_Colors.highlightedColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.highlightedColor, Color.white);
                button.m_Colors.normalColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.normalColor, Color.white);
                button.m_Colors.pressedColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.pressedColor, Color.white);
                button.m_Colors.selectedColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.selectedColor, Color.white);
                // better higlight visibility for some character colors
                button.m_Colors.m_ColorMultiplier = 1.5f;
            }


            private static void RemoveGameEndPanelDetails()
            {
                Transform safeAreaJUICEDLMAO = HudAssets.GameEndReportPanel.transform.Find("SafeArea (JUICED)");
                Transform bodyArea = safeAreaJUICEDLMAO.Find("BodyArea");



                Transform headerArea = safeAreaJUICEDLMAO.Find("HeaderArea");

                HGTextMeshProUGUI deathFlavorTextMesh = headerArea.Find("DeathFlavorText").GetComponent<HGTextMeshProUGUI>();
                deathFlavorTextMesh.fontStyle = FontStyles.Normal;
                deathFlavorTextMesh.fontSizeMax = 30f;

                HGTextMeshProUGUI resultLabelMesh = headerArea.Find("ResultArea/ResultLabel").GetComponent<HGTextMeshProUGUI>();
                resultLabelMesh.material = HudAssets.FontMaterial;



                Transform statsAndChatArea = bodyArea.Find("StatsAndChatArea");
                Transform chatArea = statsAndChatArea.Find("ChatArea");
                Transform chatBox = chatArea.GetChild(0);
                Transform standardRect = chatBox.GetChild(2);
                Transform standardRectScrollView = standardRect.GetChild(0);
                Transform statsContainer = statsAndChatArea.Find("StatsContainer");
                Transform statsBody = statsContainer.Find("Stats Body");
                Transform statsBodyScrollview = statsBody.Find("ScrollView");
                Transform statsBodyScrollviewViewportContent = statsBodyScrollview.Find("Viewport/Content");
                Transform scrollbarVertical = statsBodyScrollview.Find("Scrollbar Vertical");

                chatArea.DisableImageComponent();
                // GetChild(0) is to get to "PermanentBg"
                chatBox.GetChild(0).DisableImageComponent();
                // GetChild(1) is to get to "StandardRectScrollViewBackground"
                standardRectScrollView.GetChild(1).gameObject.SetActive(false);
                // GetChild(2) is to get to "StandardRectScrollViewBorderImage"
                standardRectScrollView.GetChild(2).gameObject.SetActive(false);
                statsContainer.Find("BorderImage").gameObject.SetActive(false);
                statsContainer.Find("Stats And Player Nav/Stats Header").DisableImageComponent();
                statsBody.DisableImageComponent();
                statsBodyScrollviewViewportContent.Find("SelectedDifficultyStrip").DisableImageComponent();
                statsBodyScrollviewViewportContent.Find("EnabledArtifactsStrip").DisableImageComponent();
                scrollbarVertical.DisableImageComponent();
                scrollbarVertical.Find("Sliding Area/Handle").DisableImageComponent();

                RectTransform chatAreaRect = chatArea.GetComponent<RectTransform>();
                chatAreaRect.sizeDelta = new Vector2(817f, 200f);
                chatAreaRect.localPosition = new Vector3(441.5f, -311.666666666f, 0f);
                //chatAreaRect.localEulerAngles = new Vector3(0f, 357f, 0f); // TODO this fucks things up



                Transform infoArea = bodyArea.Find("RightArea/InfoArea");
                Transform infoBody = infoArea.Find("Info Body");
                Transform itemArea = infoBody.Find("ItemArea");
                Transform itemAreaScrollView = itemArea.Find("ScrollView");
                Transform unlockArea = infoBody.Find("UnlockArea");
                Transform unlockAreaScrollView = unlockArea.Find("ScrollView");

                infoArea.Find("BorderImage").DisableImageComponent();
                infoArea.Find("Info Header").DisableImageComponent();
                itemArea.Find("Item Header").DisableImageComponent();
                itemAreaScrollView.DisableImageComponent();
                itemAreaScrollView.Find("Scrollbar Vertical").DisableImageComponent();
                unlockArea.Find("Unlocked Header").DisableImageComponent();
                unlockAreaScrollView.DisableImageComponent();
                unlockAreaScrollView.Find("Scrollbar Vertical").DisableImageComponent();
            }


            private static void RemoveScoreboardStripAssetDetails()
            {
                RawImage scoreboardStripRawImageBackground = HudAssets.ScoreboardStrip.gameObject.GetComponent<RawImage>();
                scoreboardStripRawImageBackground.texture = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");
                


                Transform longBackground = HudAssets.ScoreboardStrip.transform.GetChild(0);

                longBackground.Find("ClassBackground").DisableImageComponent();
                // DO NOT remove the items background image here
                // if we do it will break the automatic scaling of the hud at ultrawide resolutions
                // the ScoreboardStripEditor component makes the background image invisible instead
                // same effect with better functionality
                longBackground.Find("EquipmentBackground").DisableImageComponent();

                Transform nameLabel = longBackground.Find("NameLabel");
                // im pretty sure its fine to disable the backgroud image here since it doesn't scale with resolution
                nameLabel.Find("NameFocusHighlight").DisableImageComponent();
            }


            private static void RemoveMoonDetonationPanelDetails()
            {
                RectTransform hudCountdownPanelRect = HudAssets.MoonDetonationPanel.GetComponent<RectTransform>();
                // panel's x position is pushed -576.4219 ingame
                hudCountdownPanelRect.localPosition = new Vector3(576.4219f, -200f, 0f);



                Transform container = HudAssets.MoonDetonationPanel.transform.Find("Juice").Find("Container");

                container.Find("Backdrop").DisableImageComponent();
                container.Find("Border").DisableImageComponent();

                Transform countdownTitleLabel = container.Find("CountdownTitleLabel");
                HGTextMeshProUGUI countdownTitleLabelMesh = countdownTitleLabel.GetComponent<HGTextMeshProUGUI>();
                countdownTitleLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
                countdownTitleLabelMesh.color = Color.red;

                Transform countdownLabel = container.Find("CountdownLabel");
                HGTextMeshProUGUI countdownLabelMesh = countdownLabel.GetComponent<HGTextMeshProUGUI>();
                countdownLabelMesh.fontSharedMaterial = HudAssets.FontMaterial;
                countdownLabelMesh.color = Color.red;
            }


            private static void RemoveChatBoxDetails()
            {
                HudAssets.ChatBox.DisableImageComponent();



                Transform permanentBG = HudAssets.ChatBox.transform.Find("PermanentBG");

                Image permanentBGImage = permanentBG.GetComponent<Image>();
                permanentBGImage.sprite = HudAssets.WhiteSprite;
                permanentBGImage.color = new Color32(0, 0, 0, 212);

                Transform inputField = permanentBG.Find("Input Field");
                Image inputFieldImage = inputField.GetComponent<Image>();
                inputFieldImage.sprite = HudAssets.WhiteSprite;
                inputFieldImage.color = new Color32(0, 0, 0, 100);

                RectTransform permanentBGRect = permanentBG.GetComponent<RectTransform>();
                permanentBGRect.localPosition = new Vector3(0f, 1f, 0f);
                permanentBGRect.sizeDelta = new Vector2(0f, 48f);



                Transform chatBoxScrollView = HudAssets.ChatBox.transform.Find("StandardRect/Scroll View");

                chatBoxScrollView.Find("Background").DisableImageComponent();
                chatBoxScrollView.Find("BorderImage").DisableImageComponent();

                Transform chatBoxScrollbarVertical = chatBoxScrollView.Find("Scrollbar Vertical");
                Scrollbar chatBoxScrollbarVerticalScrollbar = chatBoxScrollbarVertical.GetComponent<Scrollbar>();
                chatBoxScrollbarVerticalScrollbar.enabled = true;
            }
        }

        internal static void EditHudDetails()
        {
            Log.Debug("EditHudDetails");
            EditDifficultyHudDetails();
            if (IsGameModeSimulacrum)
            {
                EditSimulacrumDefaultWaveUI();
                EditSimulacrumWavePanel();
            }
            else
            {
                EditTimerPanel();
                DisableDifficultyBarWormgear();
            }
            HideItemInventoryOutline();
            RemoveContextNotificationDetails();
            EditBossBarDetails();
            DisableScoreboardStripContainerOutline();
            DisableInspectionPanelItemIconDetails();
            RemoveArtifactPanelBackground();
            EditSuppressedItemsStrip();
            SetInspectPanelMaxAlpha();
            SetInspectPanelFadeInStatus();
            ColorMapNameTextWhite();
            SetSprintAndInventoryKeybindsStatus();
            RemoveSprintAndInventoryReminderTextBackgrounds();
            RemoveSkillAndEquipmentReminderTextBackgrounds();
            SetSkillOutlinesStatus();
            OnHudDetailEditsFinished?.Invoke();
        }
        private static void EditDifficultyHudDetails()
        {
            if (!ImportantHudTransforms.RunInfoHudPanel)
            {
                Main.Helpers.LogMissingHudVariable("EditDifficultyHudElements", "RunInfoHudPanel", "HudDetails");
                return;
            }
            if (!ImportantHudTransforms.RightInfoBar)
            {
                Main.Helpers.LogMissingHudVariable("EditDifficultyHudElements", "RightInfoBar", "HudDetails");
                return;
            }



            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            Transform setDifficultyPanel = ImportantHudTransforms.RunInfoHudPanel.Find("SetDifficultyPanel");

            difficultyBar.DisableImageComponent();
            difficultyBar.Find("Marker, Backdrop").DisableImageComponent();
            difficultyBar.Find("Scroll View").DisableImageComponent();
            setDifficultyPanel.DisableImageComponent();

            RectTransform difficultyIconRect = setDifficultyPanel.Find("DifficultyIcon").GetComponent<RectTransform>();
            difficultyIconRect.localPosition = new Vector3(20f, 0f, -0.5f);

            ImportantHudTransforms.RunInfoHudPanel.Find("OutlineImage").gameObject.SetActive(false);

            Transform objectivePanel = ImportantHudTransforms.RightInfoBar.Find("ObjectivePanel");
            objectivePanel.DisableImageComponent();
            objectivePanel.Find("Label").gameObject.SetActive(false);
        }
        private static void EditSimulacrumDefaultWaveUI()
        {
            Transform defaultWaveUI = ImportantHudTransforms.RunInfoHudPanel.Find("DefaultWaveUI");
            if (!defaultWaveUI)
            {
                Main.Helpers.LogMissingHudVariable("EditSimulacrumDefaultWaveUI", "defaultWaveUI", "HudDetails");
                return;
            }

            Transform defaultFillBarRoot = defaultWaveUI.Find("FillBarRoot");

            Transform defaultFillBarBackdrop = defaultFillBarRoot.Find("Fillbar Backdrop");
            Image defaultFillBarBackdropImage = defaultFillBarBackdrop.GetComponent<Image>();
            defaultFillBarBackdropImage.enabled = false;

            Transform defaultFillBarBackdropInner = defaultFillBarRoot.Find("FillBar Backdrop Inner");
            Image defaultFillBarBackdropInnerImage = defaultFillBarBackdropInner.GetComponent<Image>();
            defaultFillBarBackdropInnerImage.enabled = false;
        }
        private static void EditSimulacrumWavePanel()
        {
            Transform wavePanel = ImportantHudTransforms.RunInfoHudPanel.Find("WavePanel");
            if (!wavePanel)
            {
                Main.Helpers.LogMissingHudVariable("EditSimulacrumWavePanel", "wavePanel", "HudDetails");
                return;
            }

            Image wavePanelImage = wavePanel.GetComponent<Image>();
            wavePanelImage.enabled = false;
        }
        private static void EditTimerPanel()
        {
            Transform timerPanel = ImportantHudTransforms.RunInfoHudPanel.Find("TimerPanel");
            if (!timerPanel)
            {
                Main.Helpers.LogMissingHudVariable("EditTimerPanel", "timerPanel", "HudDetails");
                return;
            }

            Image timerPanelImage = timerPanel.GetComponent<Image>();
            timerPanelImage.enabled = false;

            GameObject wormGear = timerPanel.Find("Wormgear").gameObject;
            wormGear.SetActive(false);
        }
        private static void DisableDifficultyBarWormgear()
        {
            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            GameObject wormGear = difficultyBar.Find("Wormgear").gameObject;
            wormGear.SetActive(false);
        }
        internal static void SetSprintAndInventoryKeybindsStatus()
        {
            Transform scaler = MyHudLocator.FindChild("SkillDisplayRoot");
            if (!scaler)
            {
                Main.Helpers.LogMissingHudVariable("SetSprintAndInventoryKeybindsStatus", "SkillDisplayRoot");
            }

            GameObject sprintCluster = scaler.Find("SprintCluster").gameObject;
            sprintCluster.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);

            GameObject inventoryCluster = scaler.Find("InventoryCluster").gameObject;
            inventoryCluster.SetActive(ConfigOptions.ShowSprintAndInventoryKeybinds.Value);
        }
        private static void HideItemInventoryOutline()
        {
            Image itemInventoryDisplayImage = MyHud.itemInventoryDisplay.GetComponent<Image>();
            itemInventoryDisplayImage.color = Color.clear;
        }
        private static void RemoveContextNotificationDetails()
        {
            Transform contextNotification = MyHudLocator.FindChild("RightCluster").Find("ContextNotification");
            Transform contextDisplay = contextNotification.Find("ContextDisplay");
            RawImage contextDisplayImage = contextDisplay.GetComponent<RawImage>();
            contextDisplayImage.enabled = false;

            Transform inspectDisplay = contextNotification.GetChild(2);
            RawImage inspectDisplayBackground = inspectDisplay.GetComponent<RawImage>();
            inspectDisplayBackground.enabled = false;
        }
        private static void EditBossBarDetails()
        {
            Transform bossHealthBarContainer = MyHudLocator.FindChild("BossHealthBar").parent.parent;
            Image bossHealthBarContainerImage = bossHealthBarContainer.GetComponent<Image>();
            bossHealthBarContainerImage.enabled = false;


            Transform bossBackgroundPanel = bossHealthBarContainer.Find("BackgroundPanel");
            Image bossBackgroundPanelImage = bossBackgroundPanel.GetComponent<Image>();
            // this is set to true on purpose to enable dark background for missing boss hp
            bossBackgroundPanelImage.enabled = true;
        }
        private static void DisableScoreboardStripContainerOutline()
        {
            Transform scoreboardPanel = MyHudLocator.FindChild("ScoreboardPanel");
            Transform container = Helpers.GetContainerFromScoreboardPanel(scoreboardPanel);
            Transform stripContainer = container.Find("StripContainer");

            Image stripContainerImage = stripContainer.GetComponent<Image>();
            stripContainerImage.enabled = false;
        }
        private static void DisableInspectionPanelItemIconDetails()
        {
            Transform scoreboardPanel = MyHudLocator.FindChild("ScoreboardPanel");
            Transform container = Helpers.GetContainerFromScoreboardPanel(scoreboardPanel);
            Transform inspectPanelArea = container.Find("InspectPanel/InspectPanelArea");
            Transform inspectionPanel = inspectPanelArea.Find("InspectionPanel");
            Transform horizontalBox = inspectionPanel.GetChild(0);

            Transform inspectIconContainer = horizontalBox.GetChild(0);
            Image inspectIconContainerImage = inspectIconContainer.GetComponent<Image>();
            inspectIconContainerImage.enabled = false;

            Transform inspectVisualBackground = inspectIconContainer.GetChild(0);
            Image inspectHudIconImage = inspectVisualBackground.GetComponent<Image>();
            inspectHudIconImage.enabled = false;
        }
        private static void RemoveArtifactPanelBackground()
        {
            Transform artifactPanel = ImportantHudTransforms.RightInfoBar.Find("ArtifactPanel");
            if (artifactPanel)
            {
                Image artifactPanelImage = artifactPanel.GetComponent<Image>();
                artifactPanelImage.enabled = false;
            }
        }
        private static void EditSuppressedItemsStrip()
        {
            Transform scoreboardPanel = MyHudLocator.FindChild("ScoreboardPanel");
            Transform container = Helpers.GetContainerFromScoreboardPanel(scoreboardPanel);
            Transform suppressedItems = container.GetChild(3);



            ItemInventoryDisplay itemInventoryDisplay = suppressedItems.GetComponent<ItemInventoryDisplay>();
            itemInventoryDisplay.itemIconPrefabWidth = 68;

            Image suppressedItemsBackground = suppressedItems.GetComponent<Image>();
            suppressedItemsBackground.sprite = HudAssets.WhiteSprite;

            Color newBackgroundColor = suppressedItemsBackground.color;
            newBackgroundColor.a = 0.1f;
            suppressedItemsBackground.color = newBackgroundColor;
        }
        private static void ColorMapNameTextWhite()
        {
            Transform mapNameCluster = MyHud.mainContainer.transform.Find("MapNameCluster");
            Transform subtext = mapNameCluster.Find("Subtext");
            HGTextMeshProUGUI subtextMesh = subtext.GetComponent<HGTextMeshProUGUI>();
            subtextMesh.color = Color.white;
        }
        private static void RemoveSprintAndInventoryReminderTextBackgrounds()
        {
            Transform scaler = MyHudLocator.FindChild("SkillDisplayRoot");

            Transform sprintCluster = scaler.Find("SprintCluster");
            Transform keyBackgroundPanel = sprintCluster.GetChild(1);
            Image keyBackgroundPanelImage = keyBackgroundPanel.GetComponent<Image>();
            keyBackgroundPanelImage.enabled = false;

            Transform inventoryCluster = scaler.Find("InventoryCluster");
            Transform skillBackgroundPanel = inventoryCluster.Find("SkillBackgroundPanel");
            Image skillBackgroundPanelBackgroundImage = skillBackgroundPanel.GetComponent<Image>();
            skillBackgroundPanelBackgroundImage.enabled = false;
        }
        private static void RemoveSkillAndEquipmentReminderTextBackgrounds()
        {
            foreach (SkillIcon skillIcon in MyHud.skillIcons)
            {
                Transform skillBackgroundPanel = skillIcon.transform.Find("SkillBackgroundPanel");
                Image skillBackgroundPanelImage = skillBackgroundPanel.GetComponent<Image>();
                skillBackgroundPanelImage.enabled = false;
            }
            Transform equipmentDisplayRoot = MyHud.equipmentIcons[0].transform.GetChild(1);
            Transform equipmentTextBackgroundPanel = equipmentDisplayRoot.GetChild(6);
            Image equipmentTextBackgroundPanelImage = equipmentTextBackgroundPanel.GetComponent<Image>();
            equipmentTextBackgroundPanelImage.enabled = false;
        }



        internal static void EditScoreboardStripEquipmentSlotHighlight(ScoreboardStrip scoreboardStrip)
        {
            Transform equipmentBackground = scoreboardStrip.equipmentIcon.transform;
            Transform navFocusHighlight = equipmentBackground.GetChild(1);

            RawImage equipmentIconNavFocusHighlightRawImage = navFocusHighlight.GetComponent<RawImage>();
            equipmentIconNavFocusHighlightRawImage.texture = ModAssets.AssetBundle.LoadAsset<Texture2D>("NewNavHighlight");

            HGButton button = equipmentBackground.GetComponent<HGButton>();
            button.m_Colors.highlightedColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.highlightedColor, Color.white);
            button.m_Colors.normalColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.normalColor, Color.white);
            button.m_Colors.pressedColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.pressedColor, Color.white);
            button.m_Colors.selectedColor = Main.Helpers.ChangeColorWhileKeepingAlpha(button.m_Colors.selectedColor, Color.white);
            button.m_Colors.m_ColorMultiplier = 1.5f;
        }


        internal static void SetSkillsAndEquipmentReminderTextStatus()
        {
            foreach (SkillIcon skillIcon in MyHud.skillIcons)
            {
                Transform skillTextBackgroundPanel = skillIcon.transform.GetChild(5);
                skillTextBackgroundPanel.gameObject.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
            }

            Transform equipment1DisplayRoot = MyHud.equipmentIcons[0].displayRoot.transform;
            GameObject equipment1TextBackgroundPanel = equipment1DisplayRoot.GetChild(6).gameObject;
            equipment1TextBackgroundPanel.SetActive(ConfigOptions.ShowSkillKeybinds.Value);
        }


        internal static IEnumerator DelayRemoveNotificationBackground()
        {
            // need to wait a frame or else only the first notification is changed???????
            yield return null;
            RemoveNotificationBackground();
        }
        private static void RemoveNotificationBackground()
        {
            Transform notificationArea = MyHudLocator.FindChild("NotificationArea");
            if (notificationArea.childCount == 0)
            {
                Log.Debug("Notification area had no children, returning");
                return;
            }
            Transform notificationPanel = notificationArea.GetChild(0);
            if (!notificationPanel)
            {
                Log.Debug("Couldn't find a notification panel to modify, returning");
                return;
            }

            GameObject notificationBackdrop = notificationPanel.GetChild(0).GetChild(0).gameObject;
            notificationBackdrop.SetActive(false);
        }

        internal static void SetInspectPanelMaxAlpha()
        {
            Transform inspectPanelArea = ImportantHudTransforms.InspectPanelArea;
            UIJuice uiJuice = inspectPanelArea.GetComponent<UIJuice>();

            uiJuice.originalAlpha = ConfigOptions.HudTransparency.Value;
            uiJuice.transitionEndAlpha = ConfigOptions.HudTransparency.Value;
            if (!ConfigOptions.AllowInspectPanelFadeIn.Value)
            {
                inspectPanelArea.GetComponent<CanvasGroup>().alpha = ConfigOptions.HudTransparency.Value;
            }
        }
        internal static void SetInspectPanelFadeInStatus()
        {
            Transform inspectPanelArea = ImportantHudTransforms.InspectPanelArea;
            UIJuice uiJuice = inspectPanelArea.GetComponent<UIJuice>();
            CanvasGroup inspectPanelAreaCanvasGroup = inspectPanelArea.GetComponent<CanvasGroup>();

            if (ConfigOptions.AllowInspectPanelFadeIn.Value)
            {
                uiJuice.canvasGroup = inspectPanelAreaCanvasGroup;
                uiJuice.transitionDuration = ConfigOptions.InspectPanelFadeInDuration.Value;
            }
            else
            {
                uiJuice.transitionDuration = 0;
                uiJuice.canvasGroup = null;
                inspectPanelAreaCanvasGroup.alpha = ConfigOptions.HudTransparency.Value;
            }
        }


        #region Infinite last difficulty segment
        internal static void SetFakeInfiniteLastDifficultySegmentStatus()
        {
            Log.Debug("Attempting SetFakeInfiniteLastDifficultySegmentStatus");
            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            DifficultyBarController difficultyBarController = difficultyBar.GetComponent<DifficultyBarController>();
            // can't see the changes below "IM COMING FOR YOU" so
            if (difficultyBarController.currentSegmentIndex < 7)
            {
                Log.Debug("Difficulty bar is not far enough to matter, not doing SetFakeInfiniteLastDifficultySegmentStatus");
                return;
            }


            Transform segmentTemplate = difficultyBar.GetChild(4);
            Transform scrollView = difficultyBar.GetChild(1);
            Transform backdrop = scrollView.GetChild(0);


            Log.Debug($"ConfigOptions.EnableConsistentDifficultyBarBrightness.Value is {ConfigOptions.EnableConsistentDifficultyBarBrightness.Value}");
            if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value)
            {
                SetupFakeInfiniteDifficultySegment(backdrop, segmentTemplate);
            }
            else
            {
                UndoFakeInfiniteDifficultySegment(backdrop);
            }
        }
        
        private static void SetupFakeInfiniteDifficultySegment(Transform backdrop, Transform segmentTemplate)
        {
            Image backdropImage = backdrop.GetComponent<Image>();

            Vector3 slightlyShorter = new (1, 0.9f, 1);
            backdrop.localScale = slightlyShorter;

            backdropImage.sprite = segmentTemplate.GetComponent<Image>().sprite;
            Color noMoreTransparency = HudColor.SurvivorColor;
            noMoreTransparency.a = 1;
            MyHud?.StartCoroutine(TempComponentBackgroundImage(backdropImage, noMoreTransparency));
        }
        private static IEnumerator TempComponentBackgroundImage(Image backdropImage, Color newColor)
        {
            // this is stupid
            DifficultyBarBackgroundTransparencyRemover transparencyRemover = backdropImage.GetOrAddComponent<DifficultyBarBackgroundTransparencyRemover>();
            transparencyRemover.survivorColorNoTransparency = newColor;
            transparencyRemover.enabled = true;
            yield return new WaitForSeconds(2f);
            transparencyRemover.enabled = false;
        }

        private static void UndoFakeInfiniteDifficultySegment(Transform backdrop)
        {
            Log.Debug("UndoFakeInfiniteDifficultySegment");
            Image backdropImage = backdrop.GetComponent<Image>();

            backdrop.localScale = Vector3.one;
            // i can't figure out loading the original texture and this is basically the same so it's good enough
            backdropImage.sprite = HudAssets.WhiteSprite;
            Color normalTransparency = backdropImage.color;
            normalTransparency.a = 0.055f;
            backdropImage.color = normalTransparency;
        }
        #endregion


        internal static void SetSimulacrumWaveBarAnimatorStatus()
        {
            if (!IsGameModeSimulacrum)
            {
                return;
            }
            Transform simulacrumWaveUIClone = ImportantHudTransforms.RunInfoHudPanel.Find("InfiniteTowerDefaultWaveUI(Clone)");
            if (!simulacrumWaveUIClone)
            {
                return;
            }

            InfiniteTowerWaveProgressBar progressBar = simulacrumWaveUIClone.GetComponent<InfiniteTowerWaveProgressBar>();
            Animator progressBarAnimator = simulacrumWaveUIClone.GetComponent<Animator>();
            if (ConfigOptions.AllowSimulacrumWaveBarAnimating.Value)
            {
                progressBarAnimator.enabled = true;
                progressBar.animator = progressBarAnimator;
            }
            else
            {
                progressBar.animator = null;
                progressBarAnimator.enabled = false;
            }
        }


        private static bool TryGetSimulacrumWavePopUpTransform(Transform crosshairExtras, out Transform simulacrumWavePopUp)
        {
            for (int i = 0; i < crosshairExtras.childCount; i++)
            {
                Transform childTransform = crosshairExtras.GetChild(i);
                if (childTransform.name != "InfiniteTowerNextWaveUI(Clone)" && childTransform.name.Contains("InfiniteTower"))
                {
                    Log.Debug($"childTransform.name is {childTransform.name}");
                    simulacrumWavePopUp = childTransform;
                    return true;
                }
            }
            simulacrumWavePopUp = null;
            return false;
        }
        internal static IEnumerator DelayRemoveSimulacrumWavePopUpPanelDetails()
        {
            // panel usually doesn't appear on the first frame
            yield return null;
            Transform crosshairExtras = MyHudLocator.FindChild("CrosshairExtras");
            Transform simulacrumWavePopUp;
            while (!TryGetSimulacrumWavePopUpTransform(crosshairExtras, out simulacrumWavePopUp))
            {
                Log.Info($"Couldn't find any wave pop up panel, waiting a lil bit");
                yield return null;
            }
            RemoveSimulacrumWavePopUpPanelDetails(simulacrumWavePopUp);
        }
        private static void RemoveSimulacrumWavePopUpPanelDetails(Transform simulacrumWavePopUp)
        {
            Transform simulacrumWaveUiOffset = simulacrumWavePopUp.GetChild(0);

            Image background = simulacrumWaveUiOffset.GetComponent<Image>();
            background.enabled = false;

            Transform outline = simulacrumWaveUiOffset.GetChild(2);
            outline.gameObject.SetActive(false);
        }


        internal static IEnumerator DelayRemoveTimeUntilNextWaveBackground()
        {
            Transform crosshairExtras = MyHudLocator.FindChild("CrosshairExtras");
            while (!crosshairExtras.Find("InfiniteTowerNextWaveUI(Clone)"))
            {
                Log.Debug("Couldn't find InfiniteTowerNextWaveUI(Clone), waiting a lil bit");
                yield return null;
            }
            // passing in the UI as a parameter to safe on doing a few more ".Find" calls
            // as if we haven't done enough already with the while loop & my FindWithPartialMatch extension
            RemoveTimeUntilNextWaveBackground(crosshairExtras.Find("InfiniteTowerNextWaveUI(Clone)"));
        }
        private static void RemoveTimeUntilNextWaveBackground(Transform timeUntilNextWaveUI)
        {
            Transform offset = timeUntilNextWaveUI.GetChild(0);
            Transform timeUntilNextWaveRoot = offset.GetChild(0);
            Transform backdrop1 = timeUntilNextWaveRoot.GetChild(0);

            Image backdrop1Image = backdrop1.GetComponent<Image>();
            backdrop1Image.enabled = false;
        }



        internal static void SetAllyCardBackgroundsStatus()
        {
            Transform allyCardContainer = MyHudLocator.FindChild("LeftCluster").Find("AllyCardContainer");
            for (int i = 0; i < allyCardContainer.childCount; i++)
            {
                Transform allyCard = allyCardContainer.GetChild(i);
                Image background = allyCard.GetComponent<Image>();
                background.enabled = ConfigOptions.AllowAllyCardBackgrounds.Value;
                // portrait edits get reset after enabling/disabling the background
                MyHud?.StartCoroutine(DelayEditAllyCardPortrait(allyCard.GetChild(0)));
            }
        }
        internal static IEnumerator DelayEditAllyCardPortrait(Transform portrait)
        {
            yield return null;
            EditAllyCardPortrait(portrait);
        }
        internal static void EditAllyCardPortrait(Transform portrait)
        {
            portrait.localPosition = new Vector3(-0.9f, -24, 1);
            portrait.localScale = new Vector3(1, 0.99f, 1);
        }



        internal static IEnumerator DelayRemoveBadHealthSubBarFromAllAllyCards()
        {
            yield return null;
            RemoveBadHealthSubBarFromAllAllyCards();
        }
        private static void RemoveBadHealthSubBarFromAllAllyCards()
        {
            if (!IsHudEditable)
            {
                return;
            }
            AllyCardManager allyCardManager = MyHudLocator.FindChild("LeftCluster").Find("AllyCardContainer").GetComponent<AllyCardManager>();
            if (allyCardManager.cardAllocator.elements.Count == 0)
            {
                return;
            }


            foreach (AllyCardController allyCard in allyCardManager.cardAllocator.elements)
            {
                // HealthBar > BackgroundPanel > HealthBarSubBar(Clone) #2
                Transform backgroundPanel = allyCard.healthBar.transform.GetChild(0);
                Transform badHealthBar = backgroundPanel.GetChild(2);
                if (badHealthBar == null)
                {
                    return;
                }
                Image badHealthBarImage = badHealthBar.GetComponent<Image>();
                badHealthBarImage.enabled = false;
            }
        }



        internal static void RemoveBadHealthSubBarFromPersonalHealthBar()
        {
            Image badHpBarImage = ImportantHudTransforms.BarRoots.Find("HealthbarRoot").GetChild(0).GetChild(1).GetComponent<Image>();
            if (badHpBarImage != null)
            {
                badHpBarImage.enabled = false;
            }
            else
            {
                Log.Info("Couldn't find bad HP bar image. There's a chance an HP bar may appear more light-green than usual.");
            }
        }



        internal static void SetScoreboardLabelsActiveOrNot(Transform scoreboardPanel)
        {
            Transform container = Helpers.GetContainerFromScoreboardPanel(scoreboardPanel);
            Transform headerGroup = container.GetChild(0);

            headerGroup?.gameObject.SetActive(ConfigOptions.AllowScoreboardLabels.Value);
        }

        internal static void SetSkillOutlinesStatus()
        {
            foreach (var skillIcon in MyHud.skillIcons)
            {
                Transform isReadyPanel = skillIcon.transform.GetChild(0);
                Image iconOutline = isReadyPanel.GetComponent<Image>();
                iconOutline.enabled = ConfigOptions.ShowSkillAndEquipmentOutlines.Value;
            }
        }



        internal static IEnumerator DelayRemoveMonstersItemsPanelDetails()
        {
            yield return null;
            RemoveMonstersItemsPanelDetails();
        }
        private static void RemoveMonstersItemsPanelDetails()
        {
            Transform enemyInfoPanel = ImportantHudTransforms.RunInfoHudPanel.Find("RightInfoBar/EnemyInfoPanel(Clone)");
            if (enemyInfoPanel == null)
            {
                return;
            }
            Transform innerFrame = enemyInfoPanel.transform.Find("InnerFrame");


            Image enemyInfoPanelImage = enemyInfoPanel.GetComponent<Image>();
            enemyInfoPanelImage.enabled = false;

            Image innerFrameImage = innerFrame.GetComponent<Image>();
            innerFrameImage.enabled = false;

            Image monsterBodyIconContainerImage = innerFrame.Find("MonsterBodiesContainer/MonsterBodyIconContainer").GetComponent<Image>();
            monsterBodyIconContainerImage.enabled = false;

            Image inventoryDisplayImage = innerFrame.Find("InventoryContainer/InventoryDisplay").GetComponent<Image>();
            inventoryDisplayImage.enabled = false;
        }
    }
}