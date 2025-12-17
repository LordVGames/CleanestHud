using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;


internal static class BarRoots
{
    internal static void HudStructureEdits()
    {
        ImportantHudTransforms.BarRoots.SetParent(BottomCenterCluster);
        ImportantHudTransforms.BarRoots.TryDestroyComponent<VerticalLayoutGroup>();
        RectTransform barRootsRect = ImportantHudTransforms.BarRoots.GetComponent<RectTransform>();
        barRootsRect.rotation = Quaternion.identity;
        barRootsRect.pivot = new Vector2(0.5f, 0.25f);
        barRootsRect.anchoredPosition = Vector2.zero;
        barRootsRect.sizeDelta = new Vector2(-400f, 100f);
    }
}