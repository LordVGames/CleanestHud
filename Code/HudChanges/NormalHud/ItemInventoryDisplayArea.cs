using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using RoR2.UI;
namespace CleanestHud.HudChanges.NormalHud;


internal static class ItemInventoryDisplayArea
{
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