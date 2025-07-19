using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using RiskOfOptions;

namespace CleanestHud.ModSupport
{
    internal class RiskOfOptionsMod
    {
        private static bool? _modexists;
        internal static bool ModIsRunning
        {
            get
            {
                _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(RiskOfOptions.PluginInfo.PLUGIN_GUID);
                return (bool)_modexists;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void SetupModCategoryInfo()
        {
            ModSettingsManager.SetModIcon(ModAssets.AssetBundle.LoadAsset<Sprite>("CleanestHudIcon.png"));
            ModSettingsManager.SetModDescription("A revival/continuation of HIFU's CleanerHud mod. Edits the hud to be cleaner, re-organizes some things, and integrates the color of your survivor throughout the hud.");
        }
    }
}