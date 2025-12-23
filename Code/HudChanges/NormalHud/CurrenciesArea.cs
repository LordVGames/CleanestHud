using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudChanges.HudColor;
using static CleanestHud.HudResources.ImportantHudTransforms;
using RoR2.UI;
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


    internal static void HudColorEdits()
    {
        Color colorToUse = Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);


        MoneyRoot.Find("BackgroundPanel").GetComponent<RawImage>().color = colorToUse;
        MoneyRoot.Find("ValueText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
        MoneyRoot.Find("DollarSign").GetComponent<HGTextMeshProUGUI>().color = Color.white;


        LunarCoinRoot.Find("BackgroundPanel").GetComponent<RawImage>().color = colorToUse;
        LunarCoinRoot.Find("ValueText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
        LunarCoinRoot.Find("LunarCoinSign").GetComponent<HGTextMeshProUGUI>().color = Color.white;


        VoidCoinRoot.Find("BackgroundPanel").GetComponent<RawImage>().color = colorToUse;
        VoidCoinRoot.Find("ValueText").GetComponent<HGTextMeshProUGUI>().color = Color.white;
        VoidCoinRoot.Find("VoidCoinSign").GetComponent<HGTextMeshProUGUI>().color = Color.white;
    }
}