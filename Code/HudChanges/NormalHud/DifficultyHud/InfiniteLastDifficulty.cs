using System;
using RoR2;
using RoR2.UI;
using UnityEngine;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using UnityEngine.UI;
using System.Collections;
using MiscFixes.Modules;
namespace CleanestHud.HudChanges.NormalHud.DifficultyHud;


internal static class InfiniteLastDifficulty
{
    internal static void SetFakeInfiniteLastDifficultySegmentStatus()
    {
        Log.Debug("Attempting SetFakeInfiniteLastDifficultySegmentStatus");
        Transform difficultyBar = RunInfoHudPanel.Find("DifficultyBar");
        DifficultyBarController difficultyBarController = difficultyBar.GetComponent<DifficultyBarController>();
        // can't see the changes below "IM COMING FOR YOU" so
        if (difficultyBarController.currentSegmentIndex < 7)
        {
            Log.Debug("Difficulty bar is not far enough to matter, not doing SetFakeInfiniteLastDifficultySegmentStatus");
            return;
        }


        Transform segmentTemplate = difficultyBar.GetChild(4);
        Transform scrollView = difficultyBar.GetChild(1);
        Transform backdrop = scrollView.GetChild(0);


        Log.Debug($"ConfigOptions.EnableConsistentDifficultyBarBrightness.Value is {ConfigOptions.EnableConsistentDifficultyBarBrightness.Value}");
        if (ConfigOptions.EnableConsistentDifficultyBarBrightness.Value)
        {
            SetupFakeInfiniteDifficultySegment(backdrop, segmentTemplate);
        }
        else
        {
            UndoFakeInfiniteDifficultySegment(backdrop);
        }
    }

    private static void SetupFakeInfiniteDifficultySegment(Transform backdrop, Transform segmentTemplate)
    {
        Image backdropImage = backdrop.GetComponent<Image>();

        Vector3 slightlyShorter = new(1, 0.9f, 1);
        backdrop.localScale = slightlyShorter;

        backdropImage.sprite = segmentTemplate.GetComponent<Image>().sprite;
        Color noMoreTransparency = HudColor.SurvivorColor;
        noMoreTransparency.a = 1;
        MyHud?.StartCoroutine(TempComponentBackgroundImage(backdropImage, noMoreTransparency));
    }
    private static IEnumerator TempComponentBackgroundImage(Image backdropImage, Color newColor)
    {
        // this is stupid
        var transparencyRemover = backdropImage.GetOrAddComponent<HudEditorComponents.DifficultyBarBackgroundTransparencyRemover>();
        transparencyRemover.survivorColorNoTransparency = newColor;
        transparencyRemover.enabled = true;
        yield return new WaitForSeconds(2f);
        transparencyRemover.enabled = false;
    }

    private static void UndoFakeInfiniteDifficultySegment(Transform backdrop)
    {
        Log.Debug("UndoFakeInfiniteDifficultySegment");
        Image backdropImage = backdrop.GetComponent<Image>();

        backdrop.localScale = Vector3.one;
        // i can't figure out loading the original texture and this is basically the same so it's good enough
        backdropImage.sprite = HudAssets.WhiteSprite;
        Color normalTransparency = backdropImage.color;
        normalTransparency.a = 0.055f;
        backdropImage.color = normalTransparency;
    }
}