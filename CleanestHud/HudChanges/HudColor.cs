using RoR2;
using RoR2.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Object;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using System.Linq;
namespace CleanestHud.HudChanges
{
    internal class HudColor
    {
        /// <summary>
        /// Stores the current survivor's color.
        /// Setting this will automatically update the HUD's color, but only when it's a different color.
        /// </summary>
        /// 
        /// <remarks>
        /// Updated every time the camera's target changes. When setting this it does a GetComponent call since it gets <see cref="CurrentHudColor"/>.
        /// </remarks>
        public static Color SurvivorColor
        {
            get { return _survivorColor; }
            set
            {
                Log.Debug("SurvivorColor has changed!");
                Log.Debug($"New color will be {value}");
                Color currentColor = CurrentHudColor;
                Log.Debug($"currentColor is {currentColor}");
                if (value == currentColor)
                {
                    Log.Debug("Value is the same as current color. Returning.");
                    return;
                }
                else if (currentColor == Color.clear)
                {
                    Log.Debug("Current color is clear. Returning.");
                    return;
                }
                _survivorColor = value;
                UpdateHudColor();
            }
        }
        private static Color _survivorColor;
        public static Color CurrentHudColor
        {
            get
            {
                if (!IsHudFinishedLoading)
                {
                    Log.Error("Tried to get the HUD color, but the HUD has not finished loading!");
                    return Color.clear;
                }
                // this uses the pure survivor color and it's the quickest to get to in the hud's layout
                GameObject isReadyPanel1 = MyHud.skillIcons[0].isReadyPanelObject;
                Image isReadyPanelImage1 = isReadyPanel1.GetComponent<Image>();
                return isReadyPanelImage1.color;
            }
        }

        public const float DefaultHudColorIntensity = 0.643f;
        public const float DefaultSurvivorColorMultiplier = 0.85f;

        public static void UpdateHudColor()
        {
            Log.Debug("UpdateHudColor");
            if (!Helpers.TestLevelDisplayClusterAvailability())
            {
                return;
            }

            Transform simulacrumWaveUI = ImportantHudTransforms.RunInfoHudPanel.Find("InfiniteTowerDefaultWaveUI(Clone)");
            if (IsGameModeSimulacrum && simulacrumWaveUI)
            {
                ColorSimulacrumWaveProgressBar(simulacrumWaveUI.Find("FillBarRoot"));
            }
            else
            {
                ColorDifficultyBar();
            }
            Log.Debug($"SurvivorColor is {SurvivorColor}");
            ColorXpBar(ImportantHudTransforms.BarRoots.Find("LevelDisplayCluster").Find("ExpBarRoot"));
            ColorSkillAndEquipmentSlots();
            ColorCurrenciesPanel();
            ColorInspectionPanel(Helpers.GetContainerFromScoreboardPanel(ImportantHudTransforms.SpringCanvas.Find("ScoreboardPanel")));
            return;
        }
        private static void ColorXpBar(Transform xpBarRoot)
        {
            Transform shrunkenExpBarRoot = xpBarRoot.Find("ShrunkenRoot");
            Transform fillPanel = shrunkenExpBarRoot.Find("FillPanel");
            Image fillPanelImage = fillPanel.GetComponent<Image>();

            Color tempSurvivorColor = SurvivorColor;
            tempSurvivorColor.a = 0.72f;
            fillPanelImage.color = tempSurvivorColor;

        }
        private static void ColorSkillAndEquipmentSlots()
        {
            GameObject isReadyPanel1 = MyHud.skillIcons[0].isReadyPanelObject;
            GameObject isReadyPanel2 = MyHud.skillIcons[1].isReadyPanelObject;
            GameObject isReadyPanel3 = MyHud.skillIcons[2].isReadyPanelObject;
            GameObject isReadyPanel4 = MyHud.skillIcons[3].isReadyPanelObject;

            EquipmentIcon equipment1 = MyHud.equipmentIcons[0];
            Transform equipment1DisplayRoot = equipment1.displayRoot.transform;
            Transform equipmentIsReadyPanel1 = equipment1DisplayRoot.Find("IsReadyPanel");
            Transform equipment1BGPanel = equipment1DisplayRoot.Find("BGPanel");

            EquipmentIcon equipment2 = MyHud.equipmentIcons[1];
            Transform equipment2DisplayRoot = equipment2.displayRoot.transform;
            Transform equipmentIsReadyPanel2 = equipment2DisplayRoot.Find("IsReadyPanel");
            Transform equipment2BGPanel = equipment2DisplayRoot.Find("BGPanel");



            Image isReadyPanelImage1 = isReadyPanel1.GetComponent<Image>();
            isReadyPanelImage1.color = SurvivorColor;
            Image isReadyPanelImage2 = isReadyPanel2.GetComponent<Image>();
            isReadyPanelImage2.color = SurvivorColor;
            Image isReadyPanelImage3 = isReadyPanel3.GetComponent<Image>();
            isReadyPanelImage3.color = SurvivorColor;
            Image isReadyPanelImage4 = isReadyPanel4.GetComponent<Image>();
            isReadyPanelImage4.color = SurvivorColor;

            Image equipmentIsReadyPanel1Image = equipmentIsReadyPanel1.GetComponent<Image>();
            equipmentIsReadyPanel1Image.color = SurvivorColor;
            Image equipment1BGPanelImage = equipment1BGPanel.GetComponent<Image>();
            equipment1BGPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);

            Image equipmentIsReadyPanel2Image = equipmentIsReadyPanel2.GetComponent<Image>();
            equipmentIsReadyPanel2Image.color = SurvivorColor;
            Image equipment2BGPanelImage = equipment2BGPanel.GetComponent<Image>();
            equipment2BGPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);

            if (ModSupport.Starstorm2.ModIsRunning)
            {
                MyHud.StartCoroutine(ModSupport.Starstorm2.CompositeInjectorSupport.DelayColorInjectorSlots());
            }
        }
        private static void ColorCurrenciesPanel()
        {
            Transform moneyRoot = MyHud.moneyText.transform;
            Transform moneyBackgroundPanel = moneyRoot.Find("BackgroundPanel");
            Transform upperLeftCluster = moneyRoot.parent;
            Transform lunarCoinRoot = upperLeftCluster.Find("LunarCoinRoot");
            Transform lunarCoinBackgroundPanel = lunarCoinRoot.Find("BackgroundPanel");
            // void coins aren't used in vanilla, but wolfo's simulacrum mod makes use of them
            Transform voidCoinRoot = upperLeftCluster.Find("VoidCoinRoot");
            Transform voidCoinBackgroundPanel = voidCoinRoot.Find("BackgroundPanel");

            Color colorToUse = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);
            RawImage moneyBackgroundPanelImage = moneyBackgroundPanel.GetComponent<RawImage>();
            moneyBackgroundPanelImage.color = colorToUse;
            RawImage lunarCoinBackgroundPanelImage = lunarCoinBackgroundPanel.GetComponent<RawImage>();
            lunarCoinBackgroundPanelImage.color = colorToUse;
            RawImage voidCoinBackgroundPanelImage = voidCoinBackgroundPanel.GetComponent<RawImage>();
            voidCoinBackgroundPanelImage.color = colorToUse;
        }
        private static void ColorInspectionPanel(Transform container)
        {
            Transform inspectionPanel = container.Find("InspectPanel").GetChild(0).GetChild(0);
            Image inspectionPanelImage = inspectionPanel.GetComponent<Image>();
            inspectionPanelImage.color = new Color(SurvivorColor.r, SurvivorColor.g, SurvivorColor.b, 0.3f);
        }
        internal static void ColorDifficultyBar()
        {
            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            Transform scrollView = difficultyBar.Find("Scroll View");
            Transform backdrop = scrollView.Find("Backdrop");
            Transform viewport = scrollView.Find("Viewport");
            Transform content = viewport.Find("Content");

            Color[] difficultyBarSegmentColors = [];
            if (ConfigOptions.EnableConsistentDifficultyBarColor.Value)
            {
                difficultyBarSegmentColors = Enumerable.Repeat(SurvivorColor, 9).ToArray();
            }
            else
            {
                // darker colors as difficulty increases
                difficultyBarSegmentColors = [
                    SurvivorColor,
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (8f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (7f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (6f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (5f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (4f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (3f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (2f / 9f)),
                    Main.Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (1f / 9f))
                ];
            }

            DifficultyBarController difficultyBarController = difficultyBar.GetComponent<DifficultyBarController>();
            // this changes the color flash when the next difficulty backgroundImage is reached
            for (int i = 0; i < difficultyBarController.segmentDefs.Length; i++)
            {
                difficultyBarController.segmentDefs[i].color = difficultyBarSegmentColors[i];
            }
            // this actually changes the colors of the difficulty segments
            for (int i = 0; i < difficultyBarController.images.Length; i++)
            {
                Components.DifficultyScalingBarColorChanger coloredDifficultyBarImage = difficultyBarController.images[i].gameObject.GetComponent<Components.DifficultyScalingBarColorChanger>() ?? difficultyBarController.images[i].gameObject.AddComponent<Components.DifficultyScalingBarColorChanger>();
                coloredDifficultyBarImage.newColor = difficultyBarSegmentColors[i];
            }
            // coloring the backdrop needs to happen as it fades in or else it gets overridden
            MyHud.StartCoroutine(ColorBackdropImageOverFadeIn(backdrop));
        }
        private static IEnumerator ColorBackdropImageOverFadeIn(Transform backdrop)
        {
            Image backdropImage = backdrop.GetComponent<Image>();
            while (!Main.Helpers.AreColorsEqualIgnoringAlpha(backdropImage.color, SurvivorColor))
            {
                Color tempSurvivorColor = SurvivorColor;
                tempSurvivorColor.a = 0.055f;
                backdropImage.color = tempSurvivorColor;
                yield return null;
            }
        }



        internal static void ColorSimulacrumWaveProgressBar(Transform fillBarRoot)
        {
            if (!AreSimulacrumWavesRunning)
            {
                return;
            }

            Transform animated = fillBarRoot.GetChild(2);
            Image animatedImage = animated.GetComponent<Image>();
            animatedImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: 0.5f);

            Transform fillBar = fillBarRoot.GetChild(3);
            Image fillBarImage = fillBar.GetComponent<Image>();
            Components.SimulacrumBarColorChanger barImageColorChanger = fillBarImage.GetComponent<Components.SimulacrumBarColorChanger>() ?? fillBarImage.gameObject.AddComponent<Components.SimulacrumBarColorChanger>();
            barImageColorChanger.newFillBarColor = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: 0.5f);
        }



        internal static IEnumerator DelayColorItemIconHighlights(ScoreboardStrip scoreboardStrip)
        {
            // scoreboard strips don't have their icons immediately, so we wait a frame
            yield return null;
            ColorItemIconHighlights(scoreboardStrip);
        }
        private static void ColorItemIconHighlights(ScoreboardStrip scoreboardStrip)
        {
            Color colorToUse;
            if (ConfigOptions.EnableScoreboardItemHighlightColoring.Value)
            {
                colorToUse = scoreboardStrip.userBody.bodyColor;
            }
            else
            {
                // not exactly the original color after being applied but it's close enough
                colorToUse = new Color(1, 0.8353f, 0.1934f);
            }

            foreach (ItemIcon itemIcon in scoreboardStrip.itemInventoryDisplay.itemIcons)
            {
                itemIcon.glowImage.color = colorToUse;
            }
            ColorEquipmentSlotHighlight(scoreboardStrip);
        }
        private static void ColorEquipmentSlotHighlight(ScoreboardStrip scoreboardStrip)
        {
            Transform navFocusHighlight = scoreboardStrip.equipmentIcon.transform.GetChild(1);
            RawImage navFocusHighlightRawImage = navFocusHighlight.GetComponent<RawImage>();
            navFocusHighlightRawImage.color = scoreboardStrip.userBody.bodyColor;
        }


        internal static void ColorScoreboardStrip(ScoreboardStrip scoreboardStrip)
        {
            Transform scoreboardStripTransform = scoreboardStrip.transform;
            if (scoreboardStrip.userBody == null)
            {
                return;
            }

            Transform longBackground = scoreboardStripTransform.GetChild(0);
            Image longBackgroundImage = longBackground.GetComponent<Image>();
            Color normalPlayerColor = scoreboardStrip.userBody.bodyColor;
            normalPlayerColor.a = 0.15f;
            if (longBackgroundImage.color == normalPlayerColor)
            {
                return;
            }
            longBackgroundImage.color = normalPlayerColor;

            Color highlightColor = Main.Helpers.GetAdjustedColor(scoreboardStrip.userBody.bodyColor, brightnessMultiplier: 3);
            RawImage scoreboardStripHighlightRawImage = scoreboardStripTransform.gameObject.GetComponent<RawImage>();
            scoreboardStripHighlightRawImage.color = highlightColor;
        }



        internal static void ColorAllyCardControllerBackground(AllyCardController allyCardController)
        {
            if (!allyCardController || !allyCardController.cachedSourceCharacterBody)
            {
                return;
            }

            Image background = allyCardController.GetComponent<Image>();
            Color colorToUse = allyCardController.cachedSourceCharacterBody.bodyColor;
            colorToUse.a = 0.15f;
            background.sprite = HudAssets.WhiteSprite;
            background.color = colorToUse;
        }
        internal static void ColorAllAllyCardBackgrounds()
        {
            Transform leftCluster = ImportantHudTransforms.SpringCanvas.Find("LeftCluster");
            Transform allyCardContainer = leftCluster.GetChild(0);
            for (int i = 0; i < allyCardContainer.childCount; i++)
            {
                Transform allyCard = allyCardContainer.GetChild(i);
                AllyCardController allyCardController = allyCard.GetComponent<AllyCardController>();
                ColorAllyCardControllerBackground(allyCardController);
            }
        }
    }
}