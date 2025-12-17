using System;
using System.Collections;
using RoR2;
using UnityEngine;
using static CleanestHud.Main;
namespace CleanestHud.HudChanges.Simulacrum;

internal static class WavePopup
{
    internal static void InfiniteTowerRun_onWaveInitialized(InfiniteTowerWaveController obj)
    {
        if (IsHudUserBlacklisted)
        {
            return;
        }

        MyHud?.StartCoroutine(DelayRemoveSimulacrumWavePopUpPanelDetails());
        MyHud?.StartCoroutine(DelayRemoveTimeUntilNextWaveBackground());
    }


    internal static IEnumerator DelayRemoveSimulacrumWavePopUpPanelDetails()
    {
        // panel usually doesn't appear on the first frame
        yield return null;
        Transform crosshairExtras = MyHudLocator.FindChild("CrosshairExtras");
        Transform simulacrumWavePopUp;
        while (!TryGetSimulacrumWavePopUpTransform(crosshairExtras, out simulacrumWavePopUp))
        {
            Log.Info($"Couldn't find any wave pop up panel, waiting a lil bit");
            yield return null;
        }
        RemoveSimulacrumWavePopUpPanelDetails(simulacrumWavePopUp);
    }
    private static bool TryGetSimulacrumWavePopUpTransform(Transform crosshairExtras, out Transform simulacrumWavePopUp)
    {
        for (int i = 0; i < crosshairExtras.childCount; i++)
        {
            Transform childTransform = crosshairExtras.GetChild(i);
            if (childTransform.name != "InfiniteTowerNextWaveUI(Clone)" && childTransform.name.Contains("InfiniteTower"))
            {
                Log.Debug($"childTransform.name is {childTransform.name}");
                simulacrumWavePopUp = childTransform;
                return true;
            }
        }
        simulacrumWavePopUp = null;
        return false;
    }
    private static void RemoveSimulacrumWavePopUpPanelDetails(Transform simulacrumWavePopUp)
    {
        Transform simulacrumWaveUiOffset = simulacrumWavePopUp.GetChild(0);
        simulacrumWaveUiOffset.DisableImageComponent();
        // GetChild(2) is outline
        simulacrumWaveUiOffset.GetChild(2).gameObject.SetActive(false);
    }


    internal static IEnumerator DelayRemoveTimeUntilNextWaveBackground()
    {
        Transform crosshairExtras = MyHudLocator.FindChild("CrosshairExtras");
        while (!crosshairExtras.Find("InfiniteTowerNextWaveUI(Clone)"))
        {
            Log.Debug("Couldn't find InfiniteTowerNextWaveUI(Clone), waiting a lil bit");
            yield return null;
        }
        // passing in the UI as a parameter to safe on doing a few more ".Find" calls
        // as if we haven't done enough already with the while loop & my FindWithPartialMatch extension
        RemoveTimeUntilNextWaveBackground(crosshairExtras.Find("InfiniteTowerNextWaveUI(Clone)"));
    }
    private static void RemoveTimeUntilNextWaveBackground(Transform timeUntilNextWaveUI)
    {
        // timeUntilNextWaveUI > offset > timeUntilNextWaveRoot > backdrop1
        timeUntilNextWaveUI.GetChild(0).GetChild(0).GetChild(0).DisableImageComponent();
    }
}