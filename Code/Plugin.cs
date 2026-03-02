using System;
using BepInEx;
using RoR2;
using R2API.Utils;
using HarmonyLib;
using MiscFixes.Modules;
using MonoDetour;
using CleanestHud.HudChanges;
using CleanestHud.HudChanges.SurvivorSpecific;
namespace CleanestHud;


[NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
[BepInDependency(MiscFixes.MiscFixesPlugin.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(RobDriver.DriverPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    public static PluginInfo PluginInfo { get; private set; }
    public void Awake()
    {
        PluginInfo = Info;
        Log.Init(Logger);
        ModAssets.Init();
        ConfigOptions.BindConfigOptions(Config);
        // asset edits need to happen very early because once the hud is fully initialized a prefab may have already been spawned and then it can't be edited this way
        HudResources.HudAssets.SetupAssets();
        // beginning of HUD editing process starts in the HudAwakeAndDestroy class in Main.cs
        // HUD coloring happens from camera target changes which are in the CameraTargetHooks class in Main.cs
        MonoDetourManager.InvokeHookInitializers(typeof(Plugin).Assembly);
        HudStructure.AddEventSubscriptions();
        HudDetails.AddEventSubscriptions();
        HudColor.AddEventSubscriptions();
        SurvivorSpecific.AddEventSubscriptions();
    }
}