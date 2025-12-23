using System;
using System.Collections;
using UnityEngine;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.UI;
using MiscFixes.Modules;
using static CleanestHud.Main;
using static CleanestHud.HudChanges.HudColor;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using MonoDetour.HookGen;
using MonoDetour;
using System.Linq;
namespace CleanestHud.HudChanges.NormalHud.DifficultyHud;


internal static class DifficultyBar
{
    internal static void HudDetailEdits()
    {
        ImportantHudTransforms.DifficultyBar.DisableImageComponent();
        ImportantHudTransforms.DifficultyBar.Find("Marker, Backdrop").DisableImageComponent();
        ImportantHudTransforms.DifficultyBar.Find("Scroll View").DisableImageComponent();

        SetDifficultyPanel.DisableImageComponent();

        RectTransform difficultyIconRect = SetDifficultyPanel.Find("DifficultyIcon").GetComponent<RectTransform>();
        difficultyIconRect.localPosition = new Vector3(20f, 0f, -0.5f);

        RunInfoHudPanel.Find("OutlineImage").gameObject.SetActive(false);

        Transform objectivePanel = RightInfoBar.Find("ObjectivePanel");
        objectivePanel.DisableImageComponent();
        objectivePanel.Find("Label").gameObject.SetActive(false);
    }


    internal static void HudColorEdits()
    {
        if (Helpers.IsGameModeSimulacrum)
        {
            return;
        }


        Color[] difficultyBarSegmentColors = [];
        if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value)
        {
            difficultyBarSegmentColors = [.. Enumerable.Repeat(SurvivorColor, 9)];
        }
        else
        {
            // darker colors as difficulty increases
            difficultyBarSegmentColors = [
                SurvivorColor,
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (8f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (7f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (6f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (5f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (4f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (3f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (2f / 9f)),
                    Helpers.GetAdjustedColor(SurvivorColor, brightnessMultiplier: (1f / 9f))
            ];
        }



        DifficultyBarController difficultyBarController = ImportantHudTransforms.DifficultyBar.GetComponent<DifficultyBarController>();
        // this changes the newColor flash when the next difficulty BackgroundImage is reached
        for (int i = 0; i < difficultyBarController.segmentDefs.Length; i++)
        {
            difficultyBarController.segmentDefs[i].color = difficultyBarSegmentColors[i];
        }
        // this actually changes the colors of the difficulty segments
        for (int i = 0; i < difficultyBarController.images.Length; i++)
        {
            HudEditorComponents.DifficultyScalingBarColorChanger coloredDifficultyBarImage = difficultyBarController.images[i].GetOrAddComponent<HudEditorComponents.DifficultyScalingBarColorChanger>();
            coloredDifficultyBarImage.newColor = difficultyBarSegmentColors[i];
        }
        // coloring the backdrop needs to happen as it fades in or else it gets overridden
        MyHud?.StartCoroutine(ColorBackdropImageOverFadeIn(DifficultyBarBackdrop));
    }
    private static IEnumerator ColorBackdropImageOverFadeIn(Transform backdrop)
    {
        Image backdropImage = backdrop.GetComponent<Image>();
        while (!Helpers.AreColorsEqualIgnoringAlpha(backdropImage.color, SurvivorColor))
        {
            Color tempSurvivorColor = SurvivorColor;
            tempSurvivorColor.a = 0.055f;
            backdropImage.color = tempSurvivorColor;
            yield return null;
        }
    }


    [MonoDetourTargets(typeof(DifficultyBarController))]
    private static class DifficultyBarControllerHook
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.RoR2.UI.DifficultyBarController.OnCurrentSegmentIndexChanged.Postfix(OnCurrentSegmentIndexChanged);
        }

        private static void OnCurrentSegmentIndexChanged(DifficultyBarController self, ref int newSegmentIndex)
        {
            if (IsHudUserBlacklisted)
            {
                return;
            }

            Log.Debug("DifficultyBarController_OnCurrentSegmentIndexChanged");
            Log.Debug($"newSegmentIndex is {newSegmentIndex}");
            Log.Debug($"ConfigOptions.EnableConsistentDifficultyBarBrightness.Value is {ConfigOptions.EnableConsistentDifficultyBarBrightness.Value}");

            if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value)
            {
                InfiniteLastDifficulty.SetFakeInfiniteLastDifficultySegmentStatus();
            }
        }
    }
}