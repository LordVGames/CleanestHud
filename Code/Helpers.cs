using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using RoR2;
using UnityEngine;
namespace CleanestHud;


internal static class Helpers
{
    public static Color GetAdjustedColor(Color rgbColor, float saturationMultiplier = 1, float brightnessMultiplier = 1, float colorIntensityMultiplier = 1, float transparencyMultiplier = 1)
    {
        Color.RGBToHSV(rgbColor, out float hsvHue, out float hsvSaturation, out float hsvBrightness);
        Color adjustedColor = Color.HSVToRGB(hsvHue, hsvSaturation * saturationMultiplier, hsvBrightness * brightnessMultiplier, true);
        adjustedColor.r = rgbColor.r * colorIntensityMultiplier;
        adjustedColor.g = rgbColor.g * colorIntensityMultiplier;
        adjustedColor.b = rgbColor.b * colorIntensityMultiplier;
        adjustedColor.a = rgbColor.a * transparencyMultiplier;
        return adjustedColor;
    }


    internal static bool AreColorsEqualIgnoringAlpha(Color color1, Color color2)
    {
        return (color1.r == color2.r
                && color1.g == color2.g
                && color1.b == color2.b);
    }


    internal static Color ChangeColorWhileKeepingAlpha(Color originalColor, Color newColor)
    {
        return new Color(newColor.r, newColor.b, newColor.g, originalColor.a);
    }


    internal static Transform GetContainerFromScoreboardPanel(Transform scoreboardPanel)
    {
        Transform container = scoreboardPanel.GetChild(0);
        if (container.name == "CommandQueueScoreboardWrapper")
        {
            // commandqueue puts the entire scoreboard container in a new transform
            // and it needs to be accounted for or else it causes an NRE
            container = container.GetChild(0);
        }
        return container;
    }
    

    internal static Transform GetScoreboardPanelFromContainer(Transform container)
    {
        Transform scoreboardPanel = container.parent;
        if (scoreboardPanel.name == "CommandQueueScoreboardWrapper")
        {
            // commandqueue puts the entire scoreboard container in a new transform
            // and it needs to be accounted for or else it causes an NRE
            scoreboardPanel = scoreboardPanel.parent;
        }
        return scoreboardPanel;
    }


    internal static bool TestLevelDisplayClusterAvailability()
    {
        // LevelDisplayCluster sometimes doesn't exist even though IsHudFinishedLoading is true???????
        // it works when CameraModeBase_OnTargetChanged goes off though so as long as we can catch it and handle it we're fine

        Transform levelDisplayCluster;
        try
        {
            // BarRoots is at BottomLeftCluster before ChangeRestOfHud changes it to BottomCenterCluster
            levelDisplayCluster = HudResources.ImportantHudTransforms.BarRoots.Find("LevelDisplayCluster");
        }
        catch
        {
            Log.Info("levelDisplayCluster could not be found\nthis can mean something messed up, but more than likely everthing is fine");
            return false;
        }
        return true;
    }


    internal static bool IsGameModeSimulacrum
    {
        get
        {
            return Run.instance.gameModeIndex == GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
        }
    }


    internal static bool AreSimulacrumWavesRunning
    {
        get
        {
            InfiniteTowerRun infiniteTowerRun = Run.instance as InfiniteTowerRun;
            if (infiniteTowerRun.waveController != null)
            {
                return true;
            }
            return false;
        }
    }
}