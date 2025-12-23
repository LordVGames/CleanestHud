using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.HudChanges.HudColor;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;

internal static class ExpBar
{
    internal static void HudStructureEdits()
    {
        Image expBarRootImage = ExpBarRoot.GetComponent<Image>();
        expBarRootImage.sprite = HudAssets.WhiteSprite;
        expBarRootImage.color = Color.clear;

        RectTransform expBarRootRect = ExpBarRoot.GetComponent<RectTransform>();
        expBarRootRect.pivot = new Vector2(0.5f, 0.5f);
        expBarRootRect.sizeDelta = new Vector2(0, expBarRootRect.sizeDelta.y);
        expBarRootRect.localScale = new Vector3(1.002f, 1, 1);

        Transform shrunkenExpBarRoot = ExpBarRoot.GetChild(0);
        RectTransform shrunkenExpBarRootRect = shrunkenExpBarRoot.GetComponent<RectTransform>();
        shrunkenExpBarRootRect.localScale = new Vector3(1.05f, 1.66f, 1f);
    }


    internal static void HudColorEdits()
    {
        Color tempSurvivorColor = SurvivorColor;
        tempSurvivorColor.a = 0.72f;
        
        Image fillPanelImage = ExpBarFillPanel.GetComponent<Image>();
        fillPanelImage.color = tempSurvivorColor;
    }
}