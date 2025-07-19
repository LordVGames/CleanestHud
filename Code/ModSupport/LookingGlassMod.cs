using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static CleanestHud.Main;
using CleanestHud.HudChanges;
using MiscFixes.Modules;

namespace CleanestHud.ModSupport
{
    // do not rename class to lookingglass, the class where the mod's pluginfo is stored in that same class name
    internal class LookingGlassMod
    {
        private static bool? _modexists;
        internal static bool ModIsRunning
        {
            get
            {
                _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LookingGlass.PluginInfo.PLUGIN_GUID);
                return (bool)_modexists;
            }
        }
        internal static bool ItemCountersConfigValue
        {
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            get
            {
                if (ModIsRunning)
                {
                    return LookingGlass.ItemCounters.ItemCounter.itemCounters.Value;
                }
                else
                {
                    return false;
                }
            }
        }



        internal static bool StatsPanelConfigValue
        {
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            get
            {
                if (ModIsRunning)
                {
                    return LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay.Value;
                }
                return false;
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void HookStatsPanelConfig_SettingChanged()
        {
            if (LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay != null)
            {
                LookingGlass.StatsDisplay.StatsDisplayClass.statsDisplay.SettingChanged += StatsPanelConfig_SettingChanged;
            }
        }
        internal static void StatsPanelConfig_SettingChanged(object sender, EventArgs e)
        {
            if (!IsHudEditable)
            {
                return;
            }

            if (StatsPanelConfigValue)
            {
                MyHud?.StartCoroutine(DelayRemoveLookingGlassStatsPanelBackground());
            }
        }



        internal static IEnumerator DelayRemoveLookingGlassStatsPanelBackground()
        {
            yield return null;
            Transform rightInfoBar = HudResources.ImportantHudTransforms.RunInfoHudPanel.Find("RightInfoBar");
            Transform playerStats = null;

            while (!rightInfoBar.Find("PlayerStats"))
            {
                Log.Debug("Couldn't find LookingGlass PlayerStats, waiting a lil bit");
                yield return null;
            }
            playerStats = rightInfoBar.Find("PlayerStats");
            while (!playerStats.TryGetComponent<RoR2.UI.SkinControllers.PanelSkinController>(out _))
            {
                Log.Debug("Couldn't find LookingGlass PlayerStats PanelSkinController, waiting a lil bit");
                yield return null;
            }
            playerStats.TryDestroyComponent<RoR2.UI.SkinControllers.PanelSkinController>();
            playerStats.TryDestroyComponent<Image>();
        }



        internal static void OnHudDetailEditsFinished()
        {
            if (ModIsRunning && StatsPanelConfigValue)
            {
                MyHud?.StartCoroutine(DelayRemoveLookingGlassStatsPanelBackground());
            }
        }
    }
}