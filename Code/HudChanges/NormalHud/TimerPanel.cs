using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets]
internal static class TimerPanel
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
    }


    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.TimerPanel?.Find("TimerText")?.GetComponent<HGTextMeshProUGUI>()?.color = Color.white;
        ImportantHudTransforms.TimerPanel?.DisableImageComponent();
        ImportantHudTransforms.TimerPanel?.Find("Wormgear").gameObject.SetActive(false);
    }
}