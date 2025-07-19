using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;

namespace CleanestHud.HudChanges.SurvivorSpecific
{
    internal class VoidFiend
    {
        private static bool IsHudCameraTargetViend
        {
            get
            {
                return HudCameraTargetBody?.baseNameToken == "VOIDSURVIVOR_BODY_NAME";
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



        internal static void SetupViendEdits()
        {
            SetVoidFiendMeterAnimatorStatus();
            MyHud?.StartCoroutine(DelayEditVoidFiendCorruptionUI());
        }

        internal static void SetVoidFiendMeterAnimatorStatus()
        {
            Transform viendMeterUi = MyHudLocator.FindChild("CrosshairExtras").Find("VoidSurvivorCorruptionUISimplified(Clone)");
            if (viendMeterUi == null)
            {
                return;
            }
            Animator viendMeterAnimator = viendMeterUi.GetComponent<Animator>();
            // y'never know
            if (viendMeterAnimator == null)
            {
                return;
            }


            viendMeterAnimator.enabled = ConfigOptions.AllowVoidFiendMeterAnimating.Value;
            // no clue if this'll fix the ui getting stuck mid-squish if the animator is disabled mid-squish.
            viendMeterAnimator.playbackTime = 0;
        }

        internal static IEnumerator DelayEditVoidFiendCorruptionUI()
        {
            yield return null;
            EditVoidFiendCorruptionUI();
        }
        internal static void EditVoidFiendCorruptionUI()
        {
            Transform viendCorruptionUI = MyHudLocator.FindChild("CrosshairExtras").Find("VoidSurvivorCorruptionUISimplified(Clone)");
            if (viendCorruptionUI == null)
            {
                return;
            }

            viendCorruptionUI.localPosition = new Vector3(1, 0.5f, 0f);
            viendCorruptionUI.localScale = Vector3.one * 1.2f;

            viendCorruptionUI.GetComponent<Animator>().speed = 0.75f;

            Transform viendCorruptionFillRoot = viendCorruptionUI.GetChild(0);
            viendCorruptionFillRoot.localPosition = Vector3.zero;
            Transform corruptionText = viendCorruptionFillRoot.Find("Text");
            corruptionText.localPosition = new Vector3(0f, -60f, 0f);
            corruptionText.DisableImageComponent();
        }
    }
}