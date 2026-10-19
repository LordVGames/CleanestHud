using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources.ImportantHudTransforms;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud.DifficultyHud;


[MonoDetourTargets]
internal static class Wormgear
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


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