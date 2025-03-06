using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;

namespace CleanestHud.HudChanges
{
    internal class SurvivorSpecific
    {
        #region Void Fiend
        internal static void SetVoidFiendMeterAnimatorStatus()
        {
            Transform mainUIArea = Main.MyHud.mainUIPanel.transform;
            Transform crosshairCanvas = mainUIArea.GetChild(1);
            Transform crosshairExtras = crosshairCanvas.GetChild(0);
            try
            {
                Transform voidFiendMeterUi = crosshairExtras.Find("VoidSurvivorCorruptionUISimplified(Clone)");
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
            Transform crosshairCanvas = ImportantHudTransforms.MainUIArea.Find("CrosshairCanvas");
            Transform crosshairExtras = crosshairCanvas.GetChild(0);
            Transform voidFiendCorruptionUI = crosshairExtras.Find("VoidSurvivorCorruptionUISimplified(Clone)");

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
        #endregion

        #region Seeker
        internal static IEnumerator DelayRepositionSeekerLotusUI()
        {
            yield return null;
            RepositionSeekerLotusUI();
        }
        internal static void RepositionSeekerLotusUI()
        {
            // this might get fucked up when other mods get involved
            Transform mainUIArea = Main.MyHud.mainUIPanel.transform;
            Transform springCanvas = mainUIArea.GetChild(2);
            Transform bottomCenterCluster = springCanvas.GetChild(4);
            // this is ran twice when spawning in as seeker but it errors out here the first time?????
            // it works the 2nd time though
            Transform bottomCenterClusterScaler = bottomCenterCluster.GetChild(2);
            Transform utilityArea = bottomCenterClusterScaler.GetChild(1);
            Transform utilityAreaDisplayRoot = utilityArea.GetChild(0);
            try
            {
                Transform seekerLotusUi = utilityAreaDisplayRoot.Find("SeekerLotusUI(Clone)");
                switch (ConfigOptions.SeekerLotusHudPosition.Value)
                {
                    case ConfigOptions.SpecialConfig.SeekerLotusHudPosition.AboveHealthBar:
                        seekerLotusUi.localPosition = new Vector3(98, -75, 0);
                        break;
                    case ConfigOptions.SpecialConfig.SeekerLotusHudPosition.LeftOfHealthbar:
                        seekerLotusUi.localPosition = new Vector3(-115, -165, 0);
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
            Transform mainUIArea = Main.MyHud.mainUIPanel.transform;
            Transform crosshairCanvas = mainUIArea.GetChild(1);
            Transform crosshairExtras = crosshairCanvas.GetChild(0);
            try
            {
                Transform seekerMeditateUi = crosshairExtras.Find("SeekerMeditateUI(Clone)");
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
        #endregion
    }
}