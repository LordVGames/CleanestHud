using CleanestHud.HudChanges.NormalHud.ScoreboardPanel;
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
using static CleanestHud.HudResources.ImportantHudTransforms;
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
        public static event Action OnHudColorEditsBegun;


        /// <summary>
        /// Happens a frame AFTER OnHudColorEditsBegun is fired.
        /// </summary>
        public static event Action OnHudColorEditsFinished;


        internal static IEnumerator SetSurvivorColorFromTargetBody(CharacterBody targetCharacterBody)
        {
            // game is a dumbass and tries to set the other player's color to YOUR hud AFTER the game already sets YOUR OWN color, but only sometimes!!!!!!!
            // so we're gonna set the color like normal then wait a tiny bit then get the color again
            // basically doesn't do anything in singleplayer and it's not really noticeable in multiplayer since everything is fading in while this happens
            SurvivorColor = Helpers.GetAdjustedColor(targetCharacterBody.bodyColor, DefaultSurvivorColorMultiplier, DefaultSurvivorColorMultiplier);
            if (IsColorChangeCoroutineWaiting)
            {
                yield break;
            }
            IsColorChangeCoroutineWaiting = true;
            yield return new WaitForSeconds(0.15f);
            if (targetCharacterBody == null)
            {
                Log.Error("targetCharacterBody WAS NULL IN SetSurvivorColorFromTargetBody! NO HUD COLOR CHANGES WILL OCCUR!");
                yield break;
            }
            Log.Debug($"targetCharacterBody.baseNameToken after dumbass delay is {targetCharacterBody.baseNameToken}");
            SurvivorColor = Helpers.GetAdjustedColor(targetCharacterBody.bodyColor, DefaultSurvivorColorMultiplier, DefaultSurvivorColorMultiplier);
            IsColorChangeCoroutineWaiting = false;
        }


        internal static void AddEventSubscriptions()
        {
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            OnHudColorEditsBegun += NormalHud.CurrenciesArea.HudColorEdits;
            OnHudColorEditsBegun += NormalHud.InspectionPanel.HudColorEdits;
            OnHudColorEditsBegun += NormalHud.DifficultyHud.DifficultyBar.HudColorEdits;
            OnHudColorEditsBegun += NormalHud.HealthBarArea.ExpBar.HudColorEdits;
            OnHudColorEditsBegun += NormalHud.SkillsAndEquipsArea.EquipmentSlots.HudColorEdits;
            OnHudColorEditsBegun += NormalHud.SkillsAndEquipsArea.SkillSlots.HudColorEdits;
            OnHudColorEditsBegun += Simulacrum.DefaultWaveUI.HudColorEdits;
            OnHudColorEditsBegun += Simulacrum.WavePanel.HudColorEdits;
        }


        internal static void Run_onRunStartGlobal(Run obj)
        {
            // why doesn't the color reset when clicking the restart button from that one mod
            // just reset fuck
            SurvivorColor = Color.clear;
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
            OnHudColorEditsBegun?.Invoke();
            MyHud?.StartCoroutine(DelayInvokeOnHudColorUpdate());
        }
        private static IEnumerator DelayInvokeOnHudColorUpdate()
        {
            yield return null;
            OnHudColorEditsFinished?.Invoke();
        }



        internal static void ColorAllOfScoreboardStrip(ScoreboardStrip scoreboardStrip, Color newColor)
        {
            ColorScoreboardStrip(scoreboardStrip, newColor);
            ColorItemIconGlowImages(scoreboardStrip, newColor);
            ScoreboardStrips.EditScoreboardStripEquipmentSlotHighlight(scoreboardStrip);
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
            Color highlightColor = Helpers.GetAdjustedColor(scoreboardStrip.userBody.bodyColor, brightnessMultiplier: 3);
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