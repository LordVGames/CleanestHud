using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudChanges.HudColor;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets]
internal static class InspectionPanel
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
        HudColor.OnHudColorEditsBegun += HudColorEdits;
    }


    internal static void HudDetailEdits()
    {
        RemoveInspectIconDetails();
        SetInspectPanelMaxAlpha();
        SetInspectPanelFadeInStatus();
    }
    internal static void RemoveInspectIconDetails()
    {
        Transform inspectIconContainer = ScoreboardPanelHorizontalBox.GetChild(0);
        inspectIconContainer.DisableImageComponent();
        // GetChild(0) is inspectVisualBackground
        inspectIconContainer.GetChild(0).DisableImageComponent();
    }
    internal static void SetInspectPanelMaxAlpha()
    {
        UIJuice uiJuice = InspectPanelArea.GetComponent<UIJuice>();
        if (uiJuice == null)
        {
            return;
        }


        uiJuice.originalAlpha = ConfigOptions.HudTransparency.Value;
        uiJuice.transitionEndAlpha = ConfigOptions.HudTransparency.Value;
        if (!ConfigOptions.AllowInspectPanelFadeIn.Value)
        {
            InspectPanelArea.GetComponent<CanvasGroup>()?.alpha = ConfigOptions.HudTransparency.Value;
        }
    }
    internal static void SetInspectPanelFadeInStatus()
    {
        UIJuice uiJuice = InspectPanelArea.GetComponent<UIJuice>();
        CanvasGroup canvasGroup = InspectPanelArea.GetComponent<CanvasGroup>();
        if (uiJuice == null || canvasGroup == null)
        {
            return;
        }


        if (ConfigOptions.AllowInspectPanelFadeIn.Value)
        {
            uiJuice.canvasGroup = canvasGroup;
            uiJuice.transitionDuration = ConfigOptions.InspectPanelFadeInDuration.Value;
        }
        else
        {
            uiJuice.transitionDuration = 0;
            uiJuice.canvasGroup = null;
            canvasGroup.alpha = ConfigOptions.HudTransparency.Value;
        }
    }


    internal static void HudColorEdits()
    {
        Image inspectionPanelImage = ScoreboardPanelContainer.Find("InspectPanel").GetChild(0).GetChild(0).GetComponent<Image>();
        inspectionPanelImage.sprite = HudAssets.WhiteSprite;
        inspectionPanelImage.color = Helpers.GetAdjustedColor(SurvivorColor, transparencyMultiplier: 0.15f);
    }
}