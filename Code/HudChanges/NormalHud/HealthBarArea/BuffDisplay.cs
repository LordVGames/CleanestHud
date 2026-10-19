using System;
using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud.HealthBarArea;


[MonoDetourTargets]
internal static class BuffDisplay
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudChanges.HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
    }


    internal static void HudStructureEdits()
    {
        BuffDisplayRoot.TryDestroyComponent<HorizontalLayoutGroup>();
        BuffDisplayRoot.SetParent(ImportantHudTransforms.BarRoots);

        RectTransform buffDisplayRootRect = BuffDisplayRoot.GetComponent<RectTransform>();
        buffDisplayRootRect.localPosition = new Vector3(-25f, -45f, 0f);
    }
}