using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using RoR2.UI;
namespace CleanestHud.HudChanges.NormalHud;


internal static class InspectionPanel
{
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
}