using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using R2API.Utils;
using MonoDetour.HookGen;
using MonoDetour;
namespace CleanestHud.HudChanges.NormalHud.DifficultyHud;


[MonoDetourTargets]
internal static class DifficultyBarSegments
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudChanges.HudStructure.OnHudStructureEditsBegun += NormalHud.DifficultyHud.DifficultyBarSegments.HudStructureEdits;
    }


    internal static void HudStructureEdits()
    {
        if (Helpers.IsGameModeSimulacrum)
        {
            return;
        }


        for (int i = 0; i < DifficultyBarContent.childCount; i++)
        {
            Transform difficultyBarSegment = DifficultyBarContent.GetChild(i);
            
            Image segmentImage = difficultyBarSegment.GetComponent<Image>();
            segmentImage.preserveAspect = true;

            RectTransform segmentRect = difficultyBarSegment.GetComponent<RectTransform>();
            segmentRect.localScale = new Vector3(1f, 1.1f, 1f);
        }
    }
}