using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using RoR2.UI;
// there's a mod that can use this outside of simulacrum but this feature was made for simulacrum so it goes in here
namespace CleanestHud.HudChanges.Simulacrum;


internal static class SuppressedItemsStrip
{
    internal static void HudDetailEdits()
    {
        ItemInventoryDisplay itemInventoryDisplay = SuppressedItems.GetComponent<ItemInventoryDisplay>();
        itemInventoryDisplay.itemIconPrefabWidth = 68;

        Image suppressedItemsBackground = SuppressedItems.GetComponent<Image>();
        suppressedItemsBackground.sprite = HudAssets.WhiteSprite;

        Color newBackgroundColor = suppressedItemsBackground.color;
        newBackgroundColor.a = 0.1f;
        suppressedItemsBackground.color = newBackgroundColor;
    }
}