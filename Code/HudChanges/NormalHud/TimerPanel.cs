using System;
using System.Collections;
using UnityEngine;
using RoR2;
using RoR2.UI;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using UnityEngine.UI;
using MiscFixes.Modules;
namespace CleanestHud.HudChanges.NormalHud;


internal static class TimerPanel
{
    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.TimerPanel?.Find("TimerText")?.GetComponent<HGTextMeshProUGUI>()?.color = Color.white;
        ImportantHudTransforms.TimerPanel?.DisableImageComponent();
        ImportantHudTransforms.TimerPanel?.Find("Wormgear").gameObject.SetActive(false);
    }
}