using CleanestHud.ModSupport;
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
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets]
internal static class MapNameText
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


    internal static void HudDetailEdits()
    {
        HGTextMeshProUGUI subtextMesh = MapNameClusterSubtext.GetComponent<HGTextMeshProUGUI>();
        subtextMesh.color = Color.white;
    }
}