using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using RoR2.UI;
using CleanestHud.ModSupport;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.HudChanges.NormalHud;


internal static class MapNameText
{
    internal static void HudDetailEdits()
    {
        HGTextMeshProUGUI subtextMesh = MapNameClusterSubtext.GetComponent<HGTextMeshProUGUI>();
        subtextMesh.color = Color.white;
    }
}