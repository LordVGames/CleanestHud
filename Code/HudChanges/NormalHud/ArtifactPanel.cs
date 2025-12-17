using System;
using MiscFixes.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using RoR2.UI;
namespace CleanestHud.HudChanges.NormalHud;

internal static class ArtifactPanel
{
    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.ArtifactPanel.DisableImageComponent();
    }
}