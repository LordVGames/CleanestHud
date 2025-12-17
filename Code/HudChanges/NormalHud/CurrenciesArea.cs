using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud;


internal static class CurrenciesArea
{
    internal static void HudStructureEdits()
    {
        VerticalLayoutGroup upperLeftClusterVerticalLayoutGroup = UpperLeftCluster.GetComponent<VerticalLayoutGroup>();
        upperLeftClusterVerticalLayoutGroup.spacing = 0;
    }


    internal static void HudDetailEdits()
    {
        UpperLeftCluster.DisableImageComponent();
    }
}