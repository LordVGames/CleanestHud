using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using MonoDetour.HookGen;
using MonoDetour;
using RoR2.HudOverlay;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class VoidFiend
{
    private static Transform _viendCorruptionUI;
    private static Animator _viendCorruptionUIAnimator;
    private static bool IsHudEditable
    {
        get
        {
            return Main.IsHudEditable
            && ConfigOptions.AllowSurvivorSpecificEdits.Value
            && HudCameraTargetBody != null
            && HudCameraTargetBody.bodyIndex == DLC1Content.BodyPrefabs.VoidSurvivorBody.bodyIndex;
        }
    }


    internal static void SetupViendEdits()
    {
        if (!IsHudEditable)
        {
            return;
        }
        _viendCorruptionUI = CrosshairExtras.Find("VoidSurvivorCorruptionUISimplified(Clone)");
        if (_viendCorruptionUI == null)
        {
            return;
        }
        _viendCorruptionUIAnimator = _viendCorruptionUI.GetComponent<Animator>();
        // y'never know
        if (_viendCorruptionUIAnimator == null)
        {
            return;
        }


        SetVoidFiendMeterAnimatorStatus();
        MyHud?.StartCoroutine(DelayEditVoidFiendCorruptionUI());
    }
    internal static void SetVoidFiendMeterAnimatorStatus()
    {
        // these lines besides .enabled fix the meter being stuck mid-squish if the animations are disabled mid-squish
        _viendCorruptionUIAnimator.SetFloat(VoidSurvivorController.isCorruptedParamHash, 0);
        _viendCorruptionUIAnimator.SetBool(VoidSurvivorController.corruptionParamHash, false);
        _viendCorruptionUIAnimator.Rebind();
        _viendCorruptionUIAnimator.enabled = ConfigOptions.AllowVoidFiendMeterAnimating.Value;
    }
    internal static IEnumerator DelayEditVoidFiendCorruptionUI()
    {
        yield return null;
        EditVoidFiendCorruptionUI();
    }
    internal static void EditVoidFiendCorruptionUI()
    {
        _viendCorruptionUI.localPosition = new Vector3(1, 0.5f, 0f);
        _viendCorruptionUI.localScale = Vector3.one * 1.2f;

        _viendCorruptionUI.GetComponent<Animator>().speed = 0.75f;

        Transform viendCorruptionFillRoot = _viendCorruptionUI.GetChild(0);
        viendCorruptionFillRoot.localPosition = Vector3.zero;
        Transform corruptionText = viendCorruptionFillRoot.Find("Text");
        corruptionText.localPosition = new Vector3(0f, -60f, 0f);
        corruptionText.DisableImageComponent();
    }
}