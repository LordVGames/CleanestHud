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
internal static class ArtifactPanel
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.ArtifactPanel.DisableImageComponent();
    }
}