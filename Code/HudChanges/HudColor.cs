using MiscFixes.Modules;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.Main;

namespace CleanestHud.HudChanges
{
    public class HudColor
    {
        public const float DefaultHudColorIntensity = 0.643f;
        public const float DefaultSurvivorColorMultiplier = 0.85f;


        public static Color SurvivorColor
        {
            get { return _survivorColor; }
            set
            {
                Log.Debug("SurvivorColor has changed!");
                Log.Debug($"New newColor will be {value}");
                Log.Debug($"_survivorColor is {_survivorColor}");
                if (MyHud && MyHud.skillIcons.Length > 0)
                {
                    if (MyHud.skillIcons[0].isReadyPanelObject.GetComponent<Image>().color == value)
                    {
                        Log.Debug("HUD is already using the new newColor, returning.");
                    }
                    else
                    {
                        Log.Debug("HUD is not using the new newColor yet!");
                        _survivorColor = value;
                        UpdateHudColor();
                    }
                }
                else
                {
                    Log.Debug("HUD has not finished setting up yet, not coloring");
                }
            }
        }
        private static Color _survivorColor;


        /// <summary>
        /// Happens after <see cref="UpdateHudColor"/> fully colors the HUD. Will not run if <see cref="SurvivorColor"/> is set to a new color but the HUD's current color is the same.
        /// </summary>
        /// <remarks>
        /// In multiplayer, this may successfully run twice during the coloring process, as a second coloring is attempted 0.15 seconds after the first to fix the game sometimes grabbing the wrong player's body color in the first coloring.
        /// </remarks>
        public static event Action OnHudColorUpdate;


        internal static IEnumerator SetSurvivorColorFromTargetBody(CharacterBody targetCharacterBody)
        {
            // game is a dumbass and tries to set the other player's color to YOUR hud AFTER the game already sets YOUR OWN color, but only sometimes!!!!!!!
            // so we're gonna set the color like normal then wait a tiny bit then get the color again
            // basically doesn't do anything in singleplayer and it's not really noticeable in multiplayer since everything is fading in while this happens
            SurvivorColor = Main.Helpers.GetAdjustedColor(targetCharacterBody.bodyColor, DefaultSurvivorColorMultiplier, DefaultSurvivorColorMultiplier);
            if (IsColorChangeCoroutineWaiting)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.15f);
            IsColorChangeCoroutineWaiting = true;
            if (targetCharacterBody == null)
            {
                Log.Error("targetCharacterBody WAS NULL IN SetSurvivorColorFromTargetBody! NO HUD COLOR CHANGES WILL OCCUR!");
                yield break;
            }
            Log.Debug($"targetCharacterBody.baseNameToken after dumbass delay is {targetCharacterBody.baseNameToken}");
            SurvivorColor = Main.Helpers.GetAdjustedColor(targetCharacterBody.bodyColor, DefaultSurvivorColorMultiplier, DefaultSurvivorColorMultiplier);
            IsColorChangeCoroutineWaiting = false;
        }


        public static void UpdateHudColor()
        {
            Log.Debug("UpdateHudColor");
            if (!IsHudEditable)
            {
                Log.Debug("Tried to run UpdateHudColor, but the HUD is not editable!");
                return;
            }
            if (!Helpers.TestLevelDisplayClusterAvailability())
            {
                return;
            }
            Log.Debug($"Now updating the HUD's color, SurvivorColor is {SurvivorColor}");



            Transform simulacrumWaveUI = ImportantHudTransforms.RunInfoHudPanel.Find("InfiniteTowerDefaultWaveUI(Clone)");
            if (IsGameModeSimulacrum && simulacrumWaveUI)
            {
                ColorSimulacrumWaveProgressBar(simulacrumWaveUI.Find("FillBarRoot"));
            }
            else
            {
                ColorDifficultyBar();
            }
            ColorXpBar(ImportantHudTransforms.BarRoots.Find("LevelDisplayCluster/ExpBarRoot"));
            ColorSkillAndEquipmentSlots();
            ColorCurrenciesPanel();
            ColorInspectionPanel(Helpers.GetContainerFromScoreboardPanel(MyHudLocator.FindChild("ScoreboardPanel")));
            MyHud?.StartCoroutine(DelayInvokeOnHudColorUpdate());
        }
        private static IEnumerator DelayInvokeOnHudColorUpdate()
        {
            yield return null;
            OnHudColorUpdate?.Invoke();
        }
        private static void ColorXpBar(Transform xpBarRoot)
        {
            Color tempSurvivorColor = SurvivorColor;
            tempSurvivorColor.a = 0.72f;
            Image fillPanelImage = xpBarRoot.Find("ShrunkenRoot/FillPanel").GetComponent<Image>();
            fillPanelImage.color = tempSurvivorColor;

        }
        private static void ColorSkillAndEquipmentSlots()
        {
            foreach (SkillIcon skillIcon in MyHud.skillIcons)
            {
                GameObject isReadyPanel = skillIcon.isReadyPanelObject;
                Image isReadyPanelImage = isReadyPanel.GetComponent<Image>();
                isReadyPanelImage.color = SurvivorColor;
            }
            foreach (EquipmentIcon equipmentIcon in MyHud.equipmentIcons)
            {
                Image equipmentIsReadyPanelImage = equipmentIcon.isReadyPanelObject.GetComponent<Image>();
                equipmentIsReadyPanelImage.color = SurvivorColor;

                Image equipmentBGPanelImage = equipmentIcon.displayRoot.transform.Find("BGPanel").GetComponent<Image>();
                equipmentBGPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);
            }
            if (ModSupport.Starstorm2.ModIsRunning)
            {
                MyHud?.StartCoroutine(ModSupport.Starstorm2.CompositeInjectorSupport.DelayColorInjectorSlots());
            }
        }
        private static void ColorCurrenciesPanel()
        {
            Transform upperLeftCluster = MyHudLocator.FindChild("UpperLeftCluster");
            Color colorToUse = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);



            MyHud.moneyText.transform.Find("BackgroundPanel").GetComponent<RawImage>().color = colorToUse;
            MyHud.moneyText.transform.Find("ValueText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
            MyHud.moneyText.transform.Find("DollarSign").GetComponent<HGTextMeshProUGUI>().color = Color.white;



            Transform lunarCoinRoot = upperLeftCluster.Find("LunarCoinRoot");

            lunarCoinRoot.Find("BackgroundPanel").GetComponent<RawImage>().color = colorToUse;
            lunarCoinRoot.Find("ValueText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
            lunarCoinRoot.Find("LunarCoinSign").GetComponent<HGTextMeshProUGUI>().color = Color.white;



            // void coins aren't used in vanilla, but wolfo's simulacrum mod makes use of them
            Transform voidCoinRoot = upperLeftCluster.Find("VoidCoinRoot");

            voidCoinRoot.Find("BackgroundPanel").GetComponent<RawImage>().color = colorToUse;
            voidCoinRoot.Find("ValueText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
            voidCoinRoot.Find("VoidCoinSign").GetComponent<HGTextMeshProUGUI>().color = Color.white;
        }
        private static void ColorInspectionPanel(Transform container)
        {
            Image inspectionPanelImage = container.Find("InspectPanel").GetChild(0).GetChild(0).GetComponent<Image>();
            inspectionPanelImage.sprite = HudAssets.WhiteSprite;
            inspectionPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, transparencyMultiplier: 0.15f);
        }
        internal static void ColorDifficultyBar()
        {
            Transform difficultyBar = ImportantHudTransforms.RunInfoHudPanel.Find("DifficultyBar");
            Transform backdrop = difficultyBar.Find("Scroll View/Backdrop");



            Color[] difficultyBarSegmentColors = [];
            if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value)
            {
                difficultyBarSegmentColors = [.. Enumerable.Repeat(SurvivorColor, 9)];
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
            // this changes the newColor flash when the next difficulty BackgroundImage is reached
            for (int i = 0; i < difficultyBarController.segmentDefs.Length; i++)
            {
                difficultyBarController.segmentDefs[i].color = difficultyBarSegmentColors[i];
            }
            // this actually changes the colors of the difficulty segments
            for (int i = 0; i < difficultyBarController.images.Length; i++)
            {
                HudEditorComponents.DifficultyScalingBarColorChanger coloredDifficultyBarImage = difficultyBarController.images[i].GetOrAddComponent<HudEditorComponents.DifficultyScalingBarColorChanger>();
                coloredDifficultyBarImage.newColor = difficultyBarSegmentColors[i];
            }
            // coloring the backdrop needs to happen as it fades in or else it gets overridden
            MyHud?.StartCoroutine(ColorBackdropImageOverFadeIn(backdrop));
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
            HudEditorComponents.SimulacrumBarColorChanger barImageColorChanger = fillBarImage.GetOrAddComponent<HudEditorComponents.SimulacrumBarColorChanger>();
            barImageColorChanger.newFillBarColor = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: 0.5f);
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
            Transform allyCardContainer = MyHudLocator.FindChild("LeftCluster").GetChild(0);
            for (int i = 0; i < allyCardContainer.childCount; i++)
            {
                Transform allyCard = allyCardContainer.GetChild(i);
                AllyCardController allyCardController = allyCard.GetComponent<AllyCardController>();
                ColorAllyCardControllerBackground(allyCardController);
            }
        }



        internal static void ColorAllOfScoreboardStrip(ScoreboardStrip scoreboardStrip, Color newColor)
        {
            ColorScoreboardStrip(scoreboardStrip, newColor);
            ColorItemIconGlowImages(scoreboardStrip, newColor);
            HudDetails.EditScoreboardStripEquipmentSlotHighlight(scoreboardStrip);
            ColorEquipmentSlotHighlight(scoreboardStrip, newColor);
        }
        private static void ColorScoreboardStrip(ScoreboardStrip scoreboardStrip, Color newColor)
        {
            Transform scoreboardStripTransform = scoreboardStrip.transform;
            Transform longBackground = scoreboardStripTransform.GetChild(0);
            Image longBackgroundImage = longBackground.GetComponent<Image>();
            newColor.a = 0.15f;
            if (longBackgroundImage.color == newColor)
            {
                Log.Debug("long background is already colored correctly, returning");
                return;
            }


            longBackgroundImage.color = newColor;
            Color highlightColor = Main.Helpers.GetAdjustedColor(scoreboardStrip.userBody.bodyColor, brightnessMultiplier: 3);
            RawImage scoreboardStripHighlightRawImage = scoreboardStripTransform.gameObject.GetComponent<RawImage>();
            scoreboardStripHighlightRawImage.color = highlightColor;
        }
        internal static IEnumerator DelayColorItemIconHighlights(ScoreboardStrip scoreboardStrip, Color newColor)
        {
            // scoreboard strips don't have their icons immediately, so we wait a frame
            yield return null;
            ColorItemIconGlowImages(scoreboardStrip, newColor);
        }
        internal static void ColorItemIconGlowImages(ScoreboardStrip scoreboardStrip, Color newColor)
        {
            if (!IsHudEditable)
            {
                return;
            }

            Color colorToUse;
            if (ConfigOptions.AllowScoreboardItemHighlightColoring.Value)
            {
                colorToUse = newColor;
            }
            else
            {
                // not exactly the original newColor after being applied but it's close enough
                colorToUse = new Color(1, 0.8353f, 0.1934f);
            }

            foreach (ItemIcon itemIcon in scoreboardStrip.itemInventoryDisplay.itemIcons)
            {
                itemIcon.glowImage.color = colorToUse;
            }
        }
        internal static void ColorEquipmentSlotHighlight(ScoreboardStrip scoreboardStrip, Color newColor)
        {
            Transform navFocusHighlight = scoreboardStrip.equipmentIcon.transform.GetChild(1);
            RawImage navFocusHighlightRawImage = navFocusHighlight.GetComponent<RawImage>();
            navFocusHighlightRawImage.color = newColor;
        }
        internal static void ColorSingleItemIconHighlight(ItemIcon itemIcon)
        {
            if (!IsHudEditable)
            {
                return;
            }
            if (itemIcon == null || itemIcon.glowImage == null)
            {
                Log.Debug("Item icon or it's image was null, not coloring it.");
                return;
            }
            // itemicon transform > itemsbackground > longbackground > scoreboardstrip > scoreboardstrip component
            // yes it looks stupid but i need to make this as efficient as possible to help performance at high item counts
            // a getcomponent hurts but i don't think i can do it otherwise
            // at least this only happens when a single icon is created/updated
            ScoreboardStrip scoreboardStrip = itemIcon.transform.parent.parent.parent.GetComponent<ScoreboardStrip>();
            if (scoreboardStrip == null)
            {
                Log.Debug("ScoreboardStrip was null??? returning");
                return;
            }

            Color colorToUse;
            if (scoreboardStrip.userBody == null)
            {
                Log.Debug("ScoreboardStrip's userBody was null, using existing color from longbackground");
                Color longBackgroundColor = scoreboardStrip.transform.GetChild(0).GetComponent<Image>().color;
                longBackgroundColor.a = 1; // restore original survivor body color
                colorToUse = longBackgroundColor;
            }
            else
            {
                colorToUse = scoreboardStrip.userBody.bodyColor;
            }
            itemIcon.glowImage.color = colorToUse;
        }
    }
}