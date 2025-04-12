using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BepInEx;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2;
using RoR2.UI;
using System.Linq;


namespace CleanestHud
{
    internal static class Main
    {
        internal static HUD MyHud = null;
        internal static bool IsHudFinishedLoading = false;
        internal static bool IsHudUserBlacklisted = false;
        internal static bool IsHudEditable
        {
            get
            {
                return IsHudFinishedLoading && !IsHudUserBlacklisted;
            }
        }
        internal static CharacterBody HudTargetBody
        {
            get
            {
                if (!MyHud)
                {
                    Log.Error("HUD did not exist when trying to get HudTargetBody!");
                }
                if (!MyHud.targetBodyObject)
                {
                    Log.Error("HUD did not have a valid targetBodyObject!");
                }
                return MyHud.targetBodyObject.GetComponent<CharacterBody>();
            }
        }
        internal static bool SizeBasedPositionsNeedCalculating;


        internal static bool IsGameModeSimulacrum
        {
            get
            {
                return Run.instance.gameModeIndex == GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
            }
        }
        internal static bool AreSimulacrumWavesRunning
        {
            get
            {
                InfiniteTowerRun infiniteTowerRun = Run.instance as InfiniteTowerRun;
                if (infiniteTowerRun.waveController != null)
                {
                    return true;
                }
                return false;
            }
        }
        internal static List<Color> LastKnownScoreboardBodyColors = [];

        internal static class OnHooks
        {
            #region Important hooks
            internal static void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, HUD hud)
            {
                orig(hud);
                Log.Debug("HUD_Awake");
                MyHud = hud;

                HudResources.ImportantHudTransforms.FindHudTransforms();

                // for SOME reason the hp bar's sub bars don't exist on AWAKE????
                // so we need to wait and keep checking until they do exist, THEN and ONLY THEN can we safely do all our hud changes
                Transform hpBarShrunkenRoot = MyHud.mainUIPanel.transform.Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("HealthbarRoot").Find("ShrunkenRoot");
                MyHud.StartCoroutine(WaitForHpBarToFinishLoading(hpBarShrunkenRoot));
            }
            private static IEnumerator WaitForHpBarToFinishLoading(Transform hpBarShrunkenRoot)
            {
                while (hpBarShrunkenRoot.childCount == 0)
                {
                    Log.Debug("HUD HP bar does not have any children to modify yet, waiting");
                    yield return null;
                }

                BeginLiveHudChanges();
            }
            private static void BeginLiveHudChanges()
            {
                // storing HudTargetBody to prevent doing extra GetComponent calls from getting HudTargetBody
                CharacterBody targetBody = HudTargetBody;

                IsHudFinishedLoading = true;
                SizeBasedPositionsNeedCalculating = true;
                // substring is to remove "(Clone)" from the end of the name
                if (ConfigOptions.BodyNameBlacklist_Array.Contains(targetBody.name.Substring(0, targetBody.name.Length - 7)))
                {
                    IsHudUserBlacklisted = true;
                    return;
                }

                HudChanges.HudStructure.EditHudStructure();
                HudChanges.HudStructure.RepositionHudElementsBasedOnWidth();
                HudChanges.HudDetails.EditHudDetails();
                // manually call OnCameraChange since it isn't called when first spawning in
                MyHud.StartCoroutine(DelayOnCameraChange(targetBody));
            }
            internal static void HUD_OnDestroy(On.RoR2.UI.HUD.orig_OnDestroy orig, HUD self)
            {
                orig(self);
                IsHudFinishedLoading = false;
                IsHudUserBlacklisted = false;
                HudChanges.HudColor.SurvivorColor = Color.clear;
            }
            internal static void CameraModeBase_OnTargetChanged(On.RoR2.CameraModes.CameraModeBase.orig_OnTargetChanged orig, RoR2.CameraModes.CameraModeBase self, CameraRigController cameraRigController, RoR2.CameraModes.CameraModeBase.OnTargetChangedArgs args)
            {
                orig(self, cameraRigController, args);
                if (!cameraRigController.targetBody)
                {
                    Log.Debug("cameraRigController.targetBody was invalid? returning");
                    return;
                }
                MyHud.StartCoroutine(DelayOnCameraChange(cameraRigController.targetBody));
            }
            private static IEnumerator DelayOnCameraChange(CharacterBody targetCharacterBody)
            {
                // color change usually works, but if the camera changes again in 2 frames the color isn't applied?????
                // and survivor hud elements don't always get changed unless we wait a frame too??????
                // why is this so FUCKING JANK
                yield return null;
                OnCameraChange(targetCharacterBody);
            }
            private static void OnCameraChange(CharacterBody targetCharacterBody)
            {
                Log.Debug("OnCameraChange");
                if (targetCharacterBody == null)
                {
                    Log.Error("targetCharacterBody WAS NULL IN OnCameraChange! NO HUD CHANGES WILL OCCUR!");
                    return;
                }
                Log.Debug($"cameraRigController.targetBody.baseNameToken is {targetCharacterBody.baseNameToken}");

                HudChanges.HudColor.SurvivorColor = Helpers.GetAdjustedColor(targetCharacterBody.bodyColor, HudChanges.HudColor.DefaultSurvivorColorMultiplier, HudChanges.HudColor.DefaultSurvivorColorMultiplier);
                if (!IsHudEditable)
                {
                    Log.Debug("Cannot do survivor-specific HUD edits, the HUD is not editable!");
                    return;
                }
                switch (targetCharacterBody.baseNameToken)
                {
                    case "VOIDSURVIVOR_BODY_NAME":
                        HudChanges.SurvivorSpecific.EditVoidFiendCorruptionUI();
                        break;
                    case "SEEKER_BODY_NAME":
                        HudChanges.SurvivorSpecific.RepositionSeekerLotusUI();
                        break;
                }
            }
            #endregion



            internal static void BaseConVar_AttemptSetString(On.RoR2.ConVar.BaseConVar.orig_AttemptSetString orig, RoR2.ConVar.BaseConVar self, string newValue)
            {
                orig(self, newValue);
                if (self.name == "resolution")
                {
                    SizeBasedPositionsNeedCalculating = true;
                    // try as we might, this doesn't really work
                    HudChanges.HudStructure.RepositionHudElementsBasedOnWidth();
                }
            }

            // removes the slight transparent background image from each ally on the list on the left
            internal static void AllyCardController_Awake(On.RoR2.UI.AllyCardController.orig_Awake orig, AllyCardController self)
            {
                orig(self);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                // icons go out of the background when there's not a lot of allies so they need to be changed a lil bit
                Transform portrait = self.transform.GetChild(0);
                MyHud.StartCoroutine(HudChanges.HudDetails.DelayEditAllyCardPortrait(portrait));

                if (!ConfigOptions.AllowAllyCardBackgrounds.Value)
                {
                    Image background = self.GetComponent<Image>();
                    background.enabled = false;
                }
            }

            internal static void AllyCardController_UpdateInfo(On.RoR2.UI.AllyCardController.orig_UpdateInfo orig, AllyCardController self)
            {
                orig(self);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                if (ConfigOptions.AllowAllyCardBackgrounds.Value)
                {
                    HudChanges.HudColor.ColorAllyCardControllerBackground(self);
                }
            }

            internal static void HealthBar_InitializeHealthBar(On.RoR2.UI.HealthBar.orig_InitializeHealthBar orig, HealthBar self)
            {
                // only edit the player's healthbar which has a special name
                if (self.name != "HealthbarRoot")
                {
                    orig(self);
                    return;
                }

                Log.Debug("HealthBar_InitializeHealthBar");
                // sots added a new way of showing the hp bar's text which is sadly lower quality than the original
                // luckily we can just disable the new way and it will fall back to the old higher quality way
                self.spriteAsNumberManager = null;
                // but it doesn't appear by default so we need to toggle the old one off and the new one on
                Transform managedSpriteHP = self.transform.Find("Managed_Sprite_HP");
                if (managedSpriteHP != null)
                {
                    managedSpriteHP.gameObject.SetActive(false);
                }
                else
                {
                    Log.Debug("Couldn't find managedSpriteHP! This is OK though since it isn't needed for the HUD.");
                }
                Transform slash = self.transform.Find("Slash");
                if (slash != null)
                {
                    slash.gameObject.SetActive(true);
                }
                else
                {
                    Log.Error("Couldn't find \"Slash\" on the HP bar! Another mod may have already disabled/removed it. This means the improved HP bar text will not be shown, and no text will be shown if \"managedSpriteHP\" was successfully set to inactive. Report this on github!");
                }

                orig(self);
            }

            // even though it's OnEnable, it's called every time a wave starts, not just the first wave of a map
            internal static void InfiniteTowerWaveProgressBar_OnEnable(On.RoR2.UI.InfiniteTowerWaveProgressBar.orig_OnEnable orig, InfiniteTowerWaveProgressBar simulacrumTowerWaveProgressBar)
            {
                orig(simulacrumTowerWaveProgressBar);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                // i would make a HudDetails method for this but it's literally one line
                simulacrumTowerWaveProgressBar.barImage.sprite = HudResources.HudAssets.WhiteSprite;
                HudChanges.HudColor.ColorSimulacrumWaveProgressBar(simulacrumTowerWaveProgressBar.barImage.transform.parent);
                HudChanges.HudDetails.SetSimulacrumWaveBarAnimatorStatus();
            }

            internal static void NotificationUIController_SetUpNotification(On.RoR2.UI.NotificationUIController.orig_SetUpNotification orig, NotificationUIController self, CharacterMasterNotificationQueue.NotificationInfo notificationInfo)
            {
                orig(self, notificationInfo);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                MyHud.StartCoroutine(HudChanges.HudDetails.DelayRemoveNotificationBackground());
            }

            internal static void ScoreboardController_Rebuild(On.RoR2.UI.ScoreboardController.orig_Rebuild orig, ScoreboardController scoreboardController)
            {
                if (IsHudUserBlacklisted)
                {
                    orig(scoreboardController);
                    return;
                }

                HudChanges.HudStructure.AssetEdits.EditScoreboardStripAsset();
                HudChanges.HudDetails.SetScoreboardLabelsActiveOrNot(scoreboardController.transform);
                orig(scoreboardController);
                MyHud.StartCoroutine(DelayScoreboardController_Rebuild(scoreboardController));
            }
            private static IEnumerator DelayScoreboardController_Rebuild(ScoreboardController scoreboardController)
            {
                // wait a frame first to make scoreboard strips added by other mods work
                yield return null;
                EditScoreboardStripsIfApplicable(scoreboardController);
            }
            private static void EditScoreboardStripsIfApplicable(ScoreboardController scoreboardController)
            {
                foreach (var scoreboardStrip in scoreboardController.stripAllocator.elements)
                {
                    Transform scoreboardStripTransform = scoreboardStrip.transform;
                    if (scoreboardStrip.userBody == null)
                    {
                        Log.Debug("scoreboard strip had a null body, returning");
                        return;
                    }

                    if (scoreboardStrip.itemInventoryDisplay.itemIconPrefabWidth != 58)
                    {
                        HudChanges.HudStructure.EditScoreboardStrip(scoreboardStrip);
                    }

                    // i don't like that i have to GetComponent on every scoreboard strip but i don't think i have a choice
                    Transform longBackground = scoreboardStripTransform.GetChild(0);
                    Image longBackgroundImage = longBackground.GetComponent<Image>();
                    if (!Helpers.AreColorsEqualIgnoringAlpha(longBackgroundImage.color, scoreboardStrip.userBody.bodyColor))
                    {
                        HudChanges.HudColor.ColorAllOfScoreboardStrip(scoreboardStrip);
                    }
                }
            }

            internal static void ScoreboardController_SelectFirstScoreboardStrip(On.RoR2.UI.ScoreboardController.orig_SelectFirstScoreboardStrip orig, ScoreboardController self)
            {
                if (IsHudUserBlacklisted)
                {
                    orig(self);
                }
                if (ConfigOptions.AllowAutoScoreboardHighlight.Value)
                {
                    orig(self);
                }
                return;
            }

            internal static void DifficultyBarController_OnCurrentSegmentIndexChanged(On.RoR2.UI.DifficultyBarController.orig_OnCurrentSegmentIndexChanged orig, DifficultyBarController self, int newSegmentIndex)
            {
                orig(self, newSegmentIndex);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                Log.Debug("DifficultyBarController_OnCurrentSegmentIndexChanged");
                Log.Debug($"newSegmentIndex is {newSegmentIndex}");
                Log.Debug($"ConfigOptions.AllowConsistentDifficultyBarColor.Value is {ConfigOptions.AllowConsistentDifficultyBarColor.Value}");

                if (newSegmentIndex > 6 && ConfigOptions.AllowConsistentDifficultyBarColor.Value)
                {
                    HudChanges.HudDetails.SetFakeInfiniteLastDifficultySegmentStatus();
                }
            }

            internal static void VoidSurvivorController_OnOverlayInstanceAdded(On.RoR2.VoidSurvivorController.orig_OnOverlayInstanceAdded orig, VoidSurvivorController self, RoR2.HudOverlay.OverlayController controller, GameObject instance)
            {
                // the animator does not exist until after orig so it HAS to be after it
                orig(self, controller, instance);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                self.overlayInstanceAnimator.enabled = ConfigOptions.AllowVoidFiendMeterAnimating.Value;
            }

            internal static void MeditationUI_SetupInputUIIcons(On.EntityStates.Seeker.MeditationUI.orig_SetupInputUIIcons orig, EntityStates.Seeker.MeditationUI self)
            {
                orig(self);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                HudChanges.SurvivorSpecific.RepositionSeekerMeditationUI();
            }
        }

        internal static class ILHooks
        {
            // hopoo forgot to put an extra space before the right cloud
            // then they never fixed it lol
            internal static void BossGroup_UpdateObservations(ILContext il)
            {
                ILCursor c = new(il);
                if (!c.TryGotoNext(MoveType.After,
                    x => x.MatchLdstr("<sprite name=\"CloudRight\" tint=1>")
                ))
                {
                    Log.Error("COULD NOT IL HOOK BossGroup_UpdateObservations");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }

                c.EmitDelegate<Func<string, string>>((cloudRightString) =>
                {
                    return cloudRightString.Insert(0, " ");
                });
            }



            // this was originally an On hook but there was always a half second where the buffs weren't realigned
            // doing this with an IL hook fixes that
            internal static void BuffDisplay_UpdateLayout(ILContext il)
            {
                ILCursor c = new(il);
                if (!c.TryGotoNext(MoveType.AfterLabel,
                    x => x.MatchCallvirt<BuffIcon>("get_rectTransform"),
                    x => x.MatchLdloc(0),
                    x => x.MatchCallvirt<RectTransform>("set_anchoredPosition")
                ))
                {
                    Log.Error("COULD NOT IL HOOK BuffDisplay_UpdateLayout");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }

                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Action<BuffDisplay>>((buffDisplay) =>
                {
                    // all healthbars have a buffDisplay, but the one for the player's hud has a special name
                    if (buffDisplay.name != "BuffDisplayRoot")
                    {
                        return;
                    }
                    if (buffDisplay.buffIconDisplayData.Count < 1)
                    {
                        return;
                    }

                    // the first buff always has +6 rotation on Y because ?????????? so it needs to be reset to 0
                    // also i swear this was working without needing to delay it but now i have to?????
                    if (buffDisplay.buffIconDisplayData[0].buffIconComponent != null)
                    {
                        MyHud.StartCoroutine(DelayFixFirstBuffRotation(buffDisplay.buffIconDisplayData[0].buffIconComponent.rectTransform));
                    }
                    if (IsHudEditable)
                    {
                        buffDisplay.rectTranform.localPosition = new Vector3(-25f * buffDisplay.buffIconDisplayData.Count, -45f, 0f);
                    }
                });
            }
            private static IEnumerator DelayFixFirstBuffRotation(RectTransform rectTransform)
            {
                yield return null;
                rectTransform.rotation = Quaternion.identity;
            }



            internal static void ItemIcon_SetItemIndex(ILContext il)
            {
                ILCursor c = new(il);



                // actually set glowImage to something (because gearbox never did!!!!!!!!!!!!)
                if (!c.TryGotoNext(MoveType.Before,
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<ItemIcon>("glowImage"),
                        x => x.MatchCall<UnityEngine.Object>("op_Implicit")
                    ))
                {
                    Log.Error("COULD NOT IL HOOK ItemIcon_SetItemIndex PART 1");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }
                try
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Action<ItemIcon>>((itemIcon) =>
                    {
                        if (itemIcon.image.name != "ItemIconScoreboard_InGame(Clone)")
                        {
                            return;
                        }

                        // doing this doesn't actually cause that much lag and only happens for whatever icons are added/updated
                        // it also makes future mass glowimage coloring super good on performance
                        itemIcon.glowImage = itemIcon.transform.GetChild(1).GetComponent<RawImage>();
                        HudChanges.HudColor.ColorSingleItemIconHighlight(itemIcon);
                    });
                }
                catch (Exception e)
                {
                    Log.Error($"COULD NOT EMIT INTO ItemIcon_SetItemIndex PART 1 DUE TO {e}");
                }



                // turns out there's normally unused code to set said glowimage's color to the item's rarity color with 0.75 alpha
                // that's cool but we're doing our own coloring methods so we're gonna Br over it
                #region Removing vanilla glowimage coloring
                if (!c.TryGotoNext(MoveType.After,
                        x => x.MatchLdcR4(0.75f),
                        x => x.MatchNewobj<Color>(),
                        x => x.MatchCallvirt<Graphic>("set_color")
                    ))
                {
                    Log.Error("COULD NOT IL HOOK ItemIcon_SetItemIndex PART 2");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }
                ILLabel afterVanillaHighlightColoring = c.DefineLabel();
                try
                {
                    c.MarkLabel(afterVanillaHighlightColoring);
                    c.Index = 0;
                }
                catch (Exception e)
                {
                    Log.Error($"COULD NOT EMIT INTO ItemIcon_SetItemIndex PART 2 DUE TO {e}");
                }

                if (!c.TryGotoNext(MoveType.AfterLabel,
                        x => x.MatchLdarg(0),
                        x => x.MatchLdfld<ItemIcon>("glowImage"),
                        x => x.MatchCall<UnityEngine.Object>("op_Implicit")
                    ))
                {
                    Log.Error("COULD NOT IL HOOK ItemIcon_SetItemIndex PART 3");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }
                try
                {
                    c.Emit(OpCodes.Br, afterVanillaHighlightColoring);
                }
                catch (Exception e)
                {
                    Log.Error($"COULD NOT EMIT INTO ItemIcon_SetItemIndex PART 3 DUE TO {e}");
                }
                #endregion
            }
        }

        internal static class Events
        {
            internal static void InfiniteTowerRun_onWaveInitialized(InfiniteTowerWaveController obj)
            {
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                MyHud.StartCoroutine(HudChanges.HudDetails.DelayRemoveSimulacrumWavePopUpPanelDetails());
                MyHud.StartCoroutine(HudChanges.HudDetails.DelayRemoveTimeUntilNextWaveBackground());
            }

            internal static void Run_onRunStartGlobal(Run obj)
            {
                // why doesn't the color reset when clicking the restart button from that one mod
                // just reset fuck
                HudChanges.HudColor.SurvivorColor = Color.clear;
            }
        }

        internal static class Helpers
        {
            public static Color GetAdjustedColor(Color rgbColor, float saturationMultiplier = 1, float brightnessMultiplier = 1, float colorIntensityMultiplier = 1, float transparencyMultiplier = 1)
            {
                Color.RGBToHSV(rgbColor, out float hsvHue, out float hsvSaturation, out float hsvBrightness);
                Color adjustedColor = Color.HSVToRGB(hsvHue, hsvSaturation * saturationMultiplier, hsvBrightness * brightnessMultiplier, true);
                adjustedColor.r = rgbColor.r * colorIntensityMultiplier;
                adjustedColor.g = rgbColor.g * colorIntensityMultiplier;
                adjustedColor.b = rgbColor.b * colorIntensityMultiplier;
                adjustedColor.a = rgbColor.a * transparencyMultiplier;
                return adjustedColor;
            }

            internal static void LogMissingHudVariable(string methodName, string variableName, string containingClassName = null)
            {
                string errorMessage = $"Could not do {methodName}";
                if (containingClassName != null)
                {
                    errorMessage += $" in {containingClassName}";
                }
                errorMessage += $"! The {variableName} was null or did it not exist!";
                Log.Error(errorMessage);
            }

            internal static bool AreColorsEqualIgnoringAlpha(Color color1, Color color2)
            {
                return (color1.r == color2.r
                        && color1.g == color2.g
                        && color1.b == color2.b);
            }

            internal static Color ChangeColorWhileKeepingAlpha(Color originalColor, Color newColor)
            {
                return new Color(newColor.r, newColor.b, newColor.g, originalColor.a);
            }
        }
    }
}