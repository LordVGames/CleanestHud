using System;
using System.Collections;
using UnityEngine;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine.UI;
using MiscFixes.Modules;
using static CleanestHud.Main;
using static CleanestHud.HudResources;
using static CleanestHud.HudResources.ImportantHudTransforms;
using MonoDetour.HookGen;
using MonoDetour;
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


    [MonoDetourTargets(typeof(RoR2.UI.DifficultyBarController))]
    private static class DifficultyBarController
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.RoR2.UI.DifficultyBarController.OnCurrentSegmentIndexChanged.Postfix(OnCurrentSegmentIndexChanged);
        }

        private static void OnCurrentSegmentIndexChanged(RoR2.UI.DifficultyBarController self, ref int newSegmentIndex)
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