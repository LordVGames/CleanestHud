using HG;
using MiscFixes.Modules;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.HudResources;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.Simulacrum;


// both this, the wave panel and a few other things are within the whole wave ui transform
[MonoDetourTargets]
internal static class DefaultWaveUI
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        HudStructure.OnHudStructureEditsBegun += HudStructureEdits;
        HudDetails.OnHudDetailEditsBegun += HudDetailEdits;
        HudColor.OnHudColorEditsBegun += HudColorEdits;
    }


    internal static void HudStructureEdits()
    {
        if (!Helpers.IsGameModeSimulacrum)
        {
            return;
        }

        Transform defaultFillBarRoot = ImportantHudTransforms.SimulacrumDefaultWaveUI.Find("FillBarRoot");

        HudEditorComponents.SimulacrumBarEditor defaultFillBarRootBarPositioner = defaultFillBarRoot.gameObject.AddComponent<HudEditorComponents.SimulacrumBarEditor>();
        defaultFillBarRootBarPositioner.idealLocalPosition = new Vector3(-120f, -21.15f, 0f);
        defaultFillBarRootBarPositioner.idealLocalScale = new Vector3(0.725f, 1f, 1f); ;

        Transform defaultRemainingEnemies = ImportantHudTransforms.SimulacrumDefaultWaveUI.Find("RemainingEnemiesRoot");
        Transform defaultRemainingEnemiesTitle = defaultRemainingEnemies.Find("RemainingEnemiesTitle");
        TextMeshProUGUI defaultRemainingEnemiesTitleMesh = defaultRemainingEnemiesTitle.GetComponent<TextMeshProUGUI>();
        defaultRemainingEnemiesTitleMesh.color = Color.white;

        Transform defaultRemainingEnemiesCounter = defaultRemainingEnemies.Find("RemainingEnemiesCounter");
        TextMeshProUGUI defaultRemainingEnemiesCounterMesh = defaultRemainingEnemiesCounter.GetComponent<TextMeshProUGUI>();
        defaultRemainingEnemiesCounterMesh.color = Color.white;
    }


    internal static void HudDetailEdits()
    {
        if (!Helpers.IsGameModeSimulacrum)
        {
            return;
        }

        Transform defaultFillBarRoot = ImportantHudTransforms.SimulacrumDefaultWaveUI.Find("FillBarRoot");
        defaultFillBarRoot.Find("Fillbar Backdrop").DisableImageComponent();
        defaultFillBarRoot.Find("FillBar Backdrop Inner").DisableImageComponent();
    }


    internal static void HudColorEdits()
    {
        Transform simulacrumWaveUI = ImportantHudTransforms.RunInfoHudPanel.Find("InfiniteTowerDefaultWaveUI(Clone)");
        if (!Helpers.IsGameModeSimulacrum || !simulacrumWaveUI || !Helpers.AreSimulacrumWavesRunning)
        {
            return;
        }
        Transform fillBarRoot = simulacrumWaveUI.Find("FIllBarRoot");

        Transform animated = fillBarRoot.GetChild(2);
        Image animatedImage = animated.GetComponent<Image>();
        animatedImage.color = Helpers.GetAdjustedColor(HudColor.SurvivorColor, colorIntensityMultiplier: 0.5f);

        Transform fillBar = fillBarRoot.GetChild(3);
        Image fillBarImage = fillBar.GetComponent<Image>();
        var barImageColorChanger = fillBarImage.EnsureComponent<HudEditorComponents.SimulacrumBarColorChanger>();
        barImageColorChanger.newFillBarColor = Helpers.GetAdjustedColor(HudColor.SurvivorColor, colorIntensityMultiplier: 0.5f);
    }
}