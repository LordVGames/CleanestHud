using System;
using System.Collections;
using UnityEngine;
using RoR2;
using RoR2.UI;
using TMPro;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using UnityEngine.UI;
using MiscFixes.Modules;
using MonoDetour.HookGen;
using MonoDetour;
namespace CleanestHud.HudChanges.Simulacrum;


internal static class WavePanel
{
    internal static void HudStructureEdits()
    {
        if (!Helpers.IsGameModeSimulacrum)
        {
            return;
        }
        Transform wavePanel = ImportantHudTransforms.RunInfoHudPanel.Find("WavePanel");
        if (!wavePanel)
        {
            return;
        }


        Transform waveText = wavePanel.Find("WaveText");
        HGTextMeshProUGUI waveTextMesh = waveText.GetComponent<HGTextMeshProUGUI>();
        waveTextMesh.color = Color.white;
    }


    internal static void HudDetailEdits()
    {
        if (!Helpers.IsGameModeSimulacrum)
        {
            return;
        }

        SimulacrumWavePanel.DisableImageComponent();
    }



    [MonoDetourTargets(typeof(InfiniteTowerWaveProgressBar))]
    private static class ProgressBarHook
    {
        [MonoDetourHookInitialize]
        private static void SetupHooks()
        {
            // even though it's OnEnable, it's called every time a wave starts, not just the first wave of a map
            Mdh.RoR2.UI.InfiniteTowerWaveProgressBar.OnEnable.Postfix(EditWaveProgressBar);
        }

        private static void EditWaveProgressBar(InfiniteTowerWaveProgressBar self)
        {
            if (IsHudUserBlacklisted)
            {
                return;
            }

            // i would make a HudDetails method for this but it's literally one line
            self.barImage.sprite = HudAssets.WhiteSprite;
            HudColor.ColorSimulacrumWaveProgressBar(self.barImage.transform.parent);
            HudDetails.SetSimulacrumWaveBarAnimatorStatus();
        }
    }
}