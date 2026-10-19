using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets]
internal static class ItemInventoryDisplayArea
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }




    internal static void HudStructureEdits()
    {
        Transform itemInventoryDisplayRoot = MyHud.itemInventoryDisplay.transform.parent;
        RectTransform itemInventoryDisplayRootRect = itemInventoryDisplayRoot.GetComponent<RectTransform>();
        // make it a lil smaller since it goes to the edges of the hud on ultrawide otherwise
        itemInventoryDisplayRootRect.sizeDelta = new Vector2(itemInventoryDisplayRootRect.sizeDelta.x * 0.75f, itemInventoryDisplayRootRect.sizeDelta.y);
    }




    internal static void HudDetailEdits()
    {
        Image itemInventoryDisplayImage = MyHud.itemInventoryDisplay.GetComponent<Image>();
        itemInventoryDisplayImage.color = Color.clear;
    }
}