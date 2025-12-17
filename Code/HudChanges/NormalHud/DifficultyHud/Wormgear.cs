using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using RoR2.UI;
namespace CleanestHud.HudChanges.NormalHud.DifficultyHud;


internal static class Wormgear
{
    internal static void HudDetailEdits()
    {
        if (Helpers.IsGameModeSimulacrum)
        {
            return;
        }

        GameObject wormGear = HudResources.ImportantHudTransforms.DifficultyBar.Find("Wormgear").gameObject;
        wormGear.SetActive(false);
    }
}