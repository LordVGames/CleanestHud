using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
// there's a mod that can use this outside of simulacrum but this feature was made for simulacrum so it goes in here
namespace CleanestHud.HudChanges.Simulacrum;


[MonoDetourTargets]
internal static class SuppressedItemsStrip
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


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