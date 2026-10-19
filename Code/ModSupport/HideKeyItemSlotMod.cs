using MonoDetour;
using MonoDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
namespace CleanestHud.ModSupport;


internal static class HideKeyItemSlotMod
{
    internal const string GUID = "kina.HideKeyItemSlot";
    private static bool? _modexists;
    internal static bool ModIsRunning
    {
        get
        {
            _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);
            return (bool)_modexists;
        }
    }
}