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
using CleanestHud.HudChanges;
using static CleanestHud.HudResources;
using MiscFixes.Modules;


namespace CleanestHud
{
    public static class Main
    {
        public static HUD MyHud = null;
        public static ChildLocator MyHudLocator = null;
        public static bool IsHudFinishedLoading = false;
        public static bool IsHudUserBlacklisted = true;
        public static bool IsHudEditable
        {
            get
            {
                //Log.Warning($"IsHudFinishedLoading is {IsHudFinishedLoading}");
                //Log.Warning($"IsHudUserBlacklisted is {IsHudUserBlacklisted}");
                //Log.Warning($"MyHudLocator is {MyHudLocator}");
                return IsHudFinishedLoading && !IsHudUserBlacklisted && MyHudLocator != null;
            }
        }
        internal static bool IsColorChangeCoroutineWaiting = false;
        internal static List<Color> LastKnownScoreboardBodyColors = [];
        internal static CharacterBody HudCameraTargetBody
        {
            get
            {
                if (!MyHud)
                {
                    Log.Error("HUD did not exist when trying to get HudCameraTargetBody!");
                    return null;
                }
                if (!MyHud.targetBodyObject)
                {
                    Log.Error("HUD did not have a valid targetBodyObject!");
                    return null;
                }
                return MyHud.cameraRigController.targetBody;
            }
        }
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



        /// <summary>
        /// Happens after HUD structure edits, HUD detail edits, and survivor-specific HUD elements have all been edited, but not HUD coloring.
        /// </summary>
        /// <remarks>
        /// Can happen multiple times after the HUD is created.
        /// </remarks>
        public static event Action OnSurvivorSpecificHudEditsFinished;



        internal static class OnHooks
        {
            #region Important hooks
            internal static void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, HUD hud)
            {
                orig(hud);
                Log.Debug("HUD_Awake");
                MyHud = hud;
                MyHudLocator = MyHud.GetComponent<ChildLocator>();
                ImportantHudTransforms.FindImportantHudTransforms();

                // for SOME reason the hp bar's sub bars don't exist on AWAKE????
                // so we need to wait and keep checking until they do exist, THEN and ONLY THEN can we safely do all our hud changes
                Transform hpBarShrunkenRoot = ImportantHudTransforms.BarRoots.Find("HealthbarRoot").Find("ShrunkenRoot");
                MyHud?.StartCoroutine(WaitForHpBarToFinishLoading(hpBarShrunkenRoot));
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
                IsHudFinishedLoading = true;


                // storing HudCameraTargetBody to prevent doing extra GetComponent calls from getting HudCameraTargetBody
                CharacterBody targetBody = HudCameraTargetBody;


                // removing "(Clone)" from the end of the name
                string properBodyName = targetBody.name[..^7];
                if (ConfigOptions.BodyNameBlacklist_Array.Contains(properBodyName))
                {
                    Log.Info($"Character body {properBodyName} is blacklisted from the HUD! 99% of HUD changes will not occur.");
                    IsHudUserBlacklisted = true;
                    return;
                }
                else
                {
                    Log.Debug($"Character body {properBodyName} is NOT blacklisted from the HUD!");
                    IsHudUserBlacklisted = false;
                }

                // sometimes an extra sub bar gets perma enabled??? it makes the healthbar a very light green and i don't want that
                HudDetails.RemoveBadHealthSubBarFromPersonalHealthBar();
                // this can also happen to all allycards, but it won't be handled in AllyCardController_Awake for already existing allycards when loading in because IsHudUserBlacklisted isn't changed to false yet
                // so they have to manually be changed now
                MyHud?.StartCoroutine(HudDetails.DelayRemoveBadHealthSubBarFromAllAllyCards());


                CanvasGroup wholeHudCanvasGroup = MyHud.GetOrAddComponent<CanvasGroup>();
                wholeHudCanvasGroup.alpha = ConfigOptions.HudTransparency.Value;


                if (ConfigOptions.AllowHudStructureEdits.Value)
                {
                    HudStructure.EditHudStructure();
                    HudStructure.RepositionHudElementsBasedOnWidth();
                }
                if (ConfigOptions.AllowHudDetailsEdits.Value)
                {
                    HudDetails.EditHudDetails();
                }
                if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value && ConfigOptions.AllowHudColorEdits.Value)
                {
                    HudDetails.SetFakeInfiniteLastDifficultySegmentStatus();
                }
                if (ConfigOptions.AllowSurvivorSpecificEdits.Value)
                {
                    // stupid stupid stupid
                    EditSurvivorSpecificUI();
                }
            }
            internal static void HUD_OnDestroy(On.RoR2.UI.HUD.orig_OnDestroy orig, HUD self)
            {
                orig(self);
                IsHudFinishedLoading = false;
                IsHudUserBlacklisted = true;
                IsColorChangeCoroutineWaiting = false;
                HudColor.SurvivorColor = Color.clear;
            }
            internal static void CameraModeBase_OnTargetChanged(On.RoR2.CameraModes.CameraModeBase.orig_OnTargetChanged orig, RoR2.CameraModes.CameraModeBase self, CameraRigController cameraRigController, RoR2.CameraModes.CameraModeBase.OnTargetChangedArgs args)
            {
                orig(self, cameraRigController, args);
                if (!cameraRigController.targetBody)
                {
                    Log.Debug("cameraRigController.targetBody was invalid? returning");
                    return;
                }
                if (MyHud != null)
                {
                    MyHud?.StartCoroutine(DelayOnCameraChange(cameraRigController));
                }
                else
                {
                    Log.Debug("MyHud was null in CameraModeBase_OnTargetChanged somehow, not doing DelayOnCameraChange");
                }
            }
            
            private static IEnumerator DelayOnCameraChange(CameraRigController cameraRigController)
            {
                // delay a frame to make sure everything that needs to be changed/setup has been (i.e. survivor specific ui)
                yield return null;
                Log.Debug("DelayOnCameraChange");
                if (cameraRigController.targetBody == null)
                {
                    Log.Error("targetCharacterBody WAS NULL IN DelayOnCameraChange! NO HUD CHANGES WILL OCCUR!");
                    yield break;
                }


                if (ConfigOptions.AllowHudStructureEdits.Value)
                {
                    HudStructure.MoveSpectatorLabel();
                }
                if (ConfigOptions.AllowSurvivorSpecificEdits.Value)
                {
                    EditSurvivorSpecificUI();
                }
                if (ConfigOptions.AllowHudColorEdits.Value)
                {
                    MyHud?.StartCoroutine(HudColor.SetSurvivorColorFromTargetBody(cameraRigController.targetBody));
                }
            }
            private static void EditSurvivorSpecificUI()
            {
                if (IsHudEditable)
                {
                    HudStructure.ResetSkillsScalerLocalPositionToDefault();
                    HudStructure.ResetInventoryClusterLocalPositionToDefault();
                    HudStructure.ResetSprintClusterLocalPositionToDefault();
                    // if a survivor specific edit is needed, it's called by this
                    OnSurvivorSpecificHudEditsFinished?.Invoke();
                    HudStructure.RepositionSkillScaler();
                    HudStructure.RepositionSprintAndInventoryReminders();
                }
                else
                {
                    Log.Debug("Cannot do survivor-specific HUD edits, the HUD is not editable!");
                }
            }
            #endregion


            internal static void BaseConVar_AttemptSetString(On.RoR2.ConVar.BaseConVar.orig_AttemptSetString orig, RoR2.ConVar.BaseConVar self, string newValue)
            {
                orig(self, newValue);
                if (self.name == "resolution")
                {
                    // try as we might, this doesn't really work
                    HudStructure.RepositionHudElementsBasedOnWidth();
                }
            }


            internal static void AllyCardController_Awake(On.RoR2.UI.AllyCardController.orig_Awake orig, AllyCardController self)
            {
                orig(self);
                if (IsHudUserBlacklisted)
                {
                    return;
                }


                // icons go out of the background when there's not a lot of allies so they need to be changed a lil bit
                Transform portrait = self.transform.GetChild(0);
                MyHud?.StartCoroutine(HudDetails.DelayEditAllyCardPortrait(portrait));
                if (!ConfigOptions.AllowAllyCardBackgrounds.Value)
                {
                    self.DisableImageComponent();
                }


                // healthbar > BackgroundPanel
                Transform backgroundPanel = self.healthBar?.transform.GetChild(0);
                if (backgroundPanel != null)
                {
                    MyHud?.StartCoroutine(DelayRemoveBadHealthSubBar(backgroundPanel));
                }
            }
            private static IEnumerator DelayRemoveBadHealthSubBar(Transform backgroundPanel)
            {
                yield return null;
                List<Transform> healthBarSubBars = backgroundPanel.FindListOfPartialMatches("HealthBarSubBar");
                if (healthBarSubBars != null && healthBarSubBars.Count > 1)
                {
                    // the 2nd one is the bad super light-green hp bar that we wanna get rid of
                    healthBarSubBars[1]?.DisableImageComponent();
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
                    HudColor.ColorAllyCardControllerBackground(self);
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
                HudColor.ColorSimulacrumWaveProgressBar(simulacrumTowerWaveProgressBar.barImage.transform.parent);
                HudDetails.SetSimulacrumWaveBarAnimatorStatus();
            }


            internal static void NotificationUIController_SetUpNotification(On.RoR2.UI.NotificationUIController.orig_SetUpNotification orig, NotificationUIController self, CharacterMasterNotificationQueue.NotificationInfo notificationInfo)
            {
                orig(self, notificationInfo);
                if (IsHudUserBlacklisted)
                {
                    return;
                }

                MyHud?.StartCoroutine(HudDetails.DelayRemoveNotificationBackground());
            }


            internal static void ScoreboardController_Rebuild(On.RoR2.UI.ScoreboardController.orig_Rebuild orig, ScoreboardController scoreboardController)
            {
                if (IsHudUserBlacklisted)
                {
                    orig(scoreboardController);
                    return;
                }

                HudDetails.SetScoreboardLabelsActiveOrNot(scoreboardController.transform);
                orig(scoreboardController);
                MyHud?.StartCoroutine(DelayScoreboardController_Rebuild(scoreboardController));
            }
            private static IEnumerator DelayScoreboardController_Rebuild(ScoreboardController scoreboardController)
            {
                if (!IsHudEditable)
                {
                    yield break;
                }

                // wait a frame first to make scoreboard strips added by other mods work
                yield return null;
                EditScoreboardStripsIfApplicable(scoreboardController);
                if (IsGameModeSimulacrum)
                {
                    SetupSuppressedItemsStripEditor(scoreboardController);
                }
            }
            private static void EditScoreboardStripsIfApplicable(ScoreboardController scoreboardController)
            {
                foreach (var scoreboardStrip in scoreboardController.stripAllocator.elements)
                {
                    Transform scoreboardStripTransform = scoreboardStrip.transform;

                    if (scoreboardStrip.itemInventoryDisplay.itemIconPrefabWidth != 58)
                    {
                        HudStructure.EditScoreboardStrip(scoreboardStrip);
                    }

                    Transform longBackground = scoreboardStripTransform.GetChild(0);
                    Image longBackgroundImage = longBackground.GetComponent<Image>();
                    if (IsHudEditable && scoreboardStrip.userBody != null && !Helpers.AreColorsEqualIgnoringAlpha(longBackgroundImage.color, scoreboardStrip.userBody.bodyColor))
                    {
                        HudColor.ColorAllOfScoreboardStrip(scoreboardStrip, scoreboardStrip.userBody.bodyColor);
                    }
                }
            }
            private static void SetupSuppressedItemsStripEditor(ScoreboardController scoreboardController)
            {
                scoreboardController.transform.Find("Container/SuppressedItems")?.GetOrAddComponent<HudEditorComponents.SuppressedItemsStripEditor>();
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
                Log.Debug($"ConfigOptions.EnableConsistentDifficultyBarBrightness.Value is {ConfigOptions.EnableConsistentDifficultyBarBrightness.Value}");

                if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value)
                {
                    HudDetails.SetFakeInfiniteLastDifficultySegmentStatus();
                }
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
                if (!c.TryGotoNext(MoveType.After,
                    x => x.MatchCallvirt(out _),
                    x => x.MatchEndfinally(),
                    x => x.MatchLdarg(0)
                ))
                {
                    Log.Error("COULD NOT IL HOOK BuffDisplay_UpdateLayout");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }

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
                        MyHud?.StartCoroutine(DelayFixFirstBuffRotation(buffDisplay.buffIconDisplayData[0].buffIconComponent.rectTransform));
                    }
                    if (!IsHudUserBlacklisted)
                    {
                        buffDisplay.rectTranform.localPosition = new Vector3(-24 * buffDisplay.buffIconDisplayData.Count, -45, 0);
                    }
                });
                c.Emit(OpCodes.Ldarg_0);
            }
            private static IEnumerator DelayFixFirstBuffRotation(RectTransform rectTransform)
            {
                yield return null;
                if (IsHudFinishedLoading)
                {
                    rectTransform.rotation = Quaternion.identity;
                }
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
                        if (IsHudUserBlacklisted)
                        {
                            return;
                        }
                        if (itemIcon.image.name != "ItemIconScoreboard_InGame(Clone)")
                        {
                            return;
                        }


                        // doing this doesn't actually cause that much lag and only happens for whatever icons are added/updated
                        // it also makes future mass glowimage coloring better on performance
                        itemIcon.glowImage = itemIcon.transform.GetChild(1).GetComponent<RawImage>();
                        HudColor.ColorSingleItemIconHighlight(itemIcon);
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



            internal static void EscapeSequenceController_SetHudCountdownEnabled(ILContext il)
            {
                ILCursor c = new(il);



                if (!c.TryGotoNext(MoveType.After,
                        x => x.MatchLdstr("Prefabs/UI/HudModules/HudCountdownPanel"),
                        x => x.MatchCall(out _),
                        x => x.MatchLdloc(1),
                        x => x.MatchCall(out _),
                        x => x.MatchStloc(2)
                    ))
                {
                    Log.Error("COULD NOT IL HOOK EscapeSequenceController_SetHudCountdownEnabled");
                    Log.Warning($"cursor is {c}");
                    Log.Warning($"il is {il}");
                    return;
                }
                try
                {
                    c.EmitDelegate<Action>(() =>
                    {
                        if (!IsHudEditable)
                        {
                            return;
                        }

                        HudDetails.EditMoonDetonationPanel();
                    });
                }
                catch (Exception e)
                {
                    Log.Error($"COULD NOT EMIT INTO ItemIcon_SetItemIndex PART 1 DUE TO {e}");
                }
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

                MyHud?.StartCoroutine(HudDetails.DelayRemoveSimulacrumWavePopUpPanelDetails());
                MyHud?.StartCoroutine(HudDetails.DelayRemoveTimeUntilNextWaveBackground());
            }

            internal static void Run_onRunStartGlobal(Run obj)
            {
                // why doesn't the color reset when clicking the restart button from that one mod
                // just reset fuck
                HudColor.SurvivorColor = Color.clear;
            }

            // idk what's with the jetbrains stuff here but that's what it autocompleted with so whatever
            internal static void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
            {
                if (artifactDef.cachedName == "MonsterTeamGainsItems" && runArtifactManager.IsArtifactEnabled(artifactDef) && IsHudEditable)
                {
                    MyHud?.StartCoroutine(HudDetails.DelayRemoveMonstersItemsPanelDetails());
                }
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