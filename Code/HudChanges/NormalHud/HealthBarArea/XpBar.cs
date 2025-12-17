using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;

internal static class XpBar
{
    internal static void HudStructureEdits()
    {
        Image expBarRootImage = ImportantHudTransforms.ExpBarRoot.GetComponent<Image>();
        expBarRootImage.sprite = HudAssets.WhiteSprite;
        expBarRootImage.color = Color.clear;

        RectTransform expBarRootRect = ImportantHudTransforms.ExpBarRoot.GetComponent<RectTransform>();
        expBarRootRect.pivot = new Vector2(0.5f, 0.5f);
        expBarRootRect.sizeDelta = new Vector2(0, expBarRootRect.sizeDelta.y);
        expBarRootRect.localScale = new Vector3(1.002f, 1, 1);

        Transform shrunkenExpBarRoot = ImportantHudTransforms.ExpBarRoot.GetChild(0);
        RectTransform shrunkenExpBarRootRect = shrunkenExpBarRoot.GetComponent<RectTransform>();
        shrunkenExpBarRootRect.localScale = new Vector3(1.05f, 1.66f, 1f);
    }
}