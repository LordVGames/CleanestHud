using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudChanges.HudColor;

namespace CleanestHud.HudChanges
{
    internal class SurvivorSpecific
    {
        internal static class VoidFiend
        {
            internal static void SetVoidFiendMeterAnimatorStatus()
            {
                try
                {
                    Transform voidFiendMeterUi = MyHudLocator.FindChild("CrosshairExtras").Find("VoidSurvivorCorruptionUISimplified(Clone)");
                    Animator voidFiendMeterAnimator = voidFiendMeterUi.GetComponent<Animator>();
                    voidFiendMeterAnimator.enabled = ConfigOptions.AllowVoidFiendMeterAnimating.Value;
                    // no clue if this'll fix the ui getting stuck mid-squish if the animator is disabled mid-squish.
                    voidFiendMeterAnimator.playbackTime = 0;

                }
                catch
                {
                    return;
                };
            }

            internal static IEnumerator DelayEditVoidFiendCorruptionUI()
            {
                yield return null;
                EditVoidFiendCorruptionUI();
            }
            internal static void EditVoidFiendCorruptionUI()
            {
                Transform voidFiendCorruptionUI = MyHudLocator.FindChild("CrosshairExtras").Find("VoidSurvivorCorruptionUISimplified(Clone)");

                voidFiendCorruptionUI.localPosition = new Vector3(1, 0.5f, 0f);
                voidFiendCorruptionUI.localScale = Vector3.one * 1.2f;

                Animator animator = voidFiendCorruptionUI.GetComponent<Animator>();
                animator.speed = 0.75f;

                Transform viendCorruptionFillRoot = voidFiendCorruptionUI.GetChild(0);
                viendCorruptionFillRoot.localPosition = Vector3.zero;

                Transform corruptionText = viendCorruptionFillRoot.Find("Text");
                corruptionText.localPosition = new Vector3(0f, -60f, 0f);

                Image corruptionTextImage = corruptionText.GetComponent<Image>();
                corruptionTextImage.enabled = false;
            }
        }

        internal static class Seeker
        {
            internal static void RepositionSeekerLotusUI()
            {
                Transform bottomCenterCluster = MyHudLocator.FindChild("BottomCenterCluster");
                Transform bottomCenterClusterScaler = bottomCenterCluster.GetChild(2);
                Transform utilityArea = bottomCenterClusterScaler.GetChild(1);
                Transform utilityAreaDisplayRoot = utilityArea.GetChild(0);
                // this is ran twice when spawning in as seeker but it errors out here the first time?????
                // it works the 2nd time though
                try
                {
                    Transform seekerLotusUi = utilityAreaDisplayRoot.Find("SeekerLotusUI(Clone)");
                    switch (ConfigOptions.SeekerLotusHudPosition.Value)
                    {
                        case ConfigOptions.SpecialConfig.SeekerLotusHudPosition.AboveHealthBar:
                            seekerLotusUi.localPosition = new Vector3(98, -85, 0);
                            break;
                        case ConfigOptions.SpecialConfig.SeekerLotusHudPosition.LeftOfSkills:
                            seekerLotusUi.localPosition = new Vector3(-82, -150, 0);
                            break;
                    }
                }
                catch
                {
                    return;
                }
            }

            internal static void RepositionSeekerMeditationUI()
            {
                try
                {
                    Transform seekerMeditateUi = MyHudLocator.FindChild("CrosshairExtras").Find("SeekerMeditateUI(Clone)");
                    switch (ConfigOptions.SeekerMeditateHudPosition.Value)
                    {
                        case ConfigOptions.SpecialConfig.SeekerMeditateHudPosition.AboveHealthBar:
                            seekerMeditateUi.localPosition = new Vector3(2.5f, 210, 0);
                            break;
                        case ConfigOptions.SpecialConfig.SeekerMeditateHudPosition.OverCrosshair:
                            seekerMeditateUi.localPosition = new Vector3(2.5f, 400, 0);
                            break;
                    }
                }
                catch
                {
                    Log.Debug("Couldn't reposition seeker meditation hud!");
                    return;
                }
            }
        }
    }
}