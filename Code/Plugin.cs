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
[BepInDependency(SS2.SS2Main.GUID, BepInDependency.DependencyFlags.SoftDependency)]
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
        SurvivorSpecific.AddEventSubscriptions();
        // don't really know where else to sub to ror2 events so whatever
        InfiniteTowerRun.onWaveInitialized += HudChanges.Simulacrum.WavePopup.InfiniteTowerRun_onWaveInitialized;
        Run.onRunStartGlobal += HudColor.Run_onRunStartGlobal;
        RunArtifactManager.onArtifactEnabledGlobal += HudChanges.NormalHud.MonsterSelectionAndItems.RunArtifactManager_onArtifactEnabledGlobal;
        

        if (ModSupport.LookingGlassMod.ModIsRunning)
        {
            HudDetails.OnHudDetailEditsFinished += ModSupport.LookingGlassMod.OnHudDetailEditsFinished;
        }
        /* if (ModSupport.Driver.ModIsRunning)
        {
            // doing a try/catch block for this stuff so the hud doesn't super die when driver adds support itself
            try
            {
                Harmony harmony = new(PluginGUID);
                harmony.CreateClassProcessor(typeof(ModSupport.Driver.HarmonyPatches)).Patch();
                Main.OnSurvivorSpecificHudEditsFinished += ModSupport.Driver.OnSurvivorSpecificHudEditsFinished;
                HudChanges.HudColor.OnHudColorUpdate += ModSupport.Driver.OnHudColorUpdate;
                On.RoR2.UI.HUD.OnDestroy += ModSupport.Driver.HUD_OnDestroy;
                ConfigOptions.OnShowSkillKeybindsChanged += ModSupport.Driver.SetWeaponTextStatus;
            }
            catch
            {
                Log.Error("Couldn't setup Driver mod support! Either Driver mod added support itself or something else went wrong.");
            }
        } */
        if (ModSupport.Myst.ModIsRunning)
        {
            HudChanges.SurvivorSpecific.SurvivorSpecific.OnSurvivorSpecificHudEditsFinished += ModSupport.Myst.OnSurvivorSpecificHudEditsFinished;
            HudChanges.HudColor.OnHudColorUpdate += ModSupport.Myst.OnHudColorUpdate;
            On.RoR2.UI.HUD.OnDestroy += ModSupport.Myst.HUD_OnDestroy;
            ConfigOptions.OnShowSprintAndInventoryKeybindsChanged += ModSupport.Myst.ConfigOptions_OnShowSprintAndInventoryKeybindsChanged;
            ConfigOptions.OnShowSkillKeybindsChanged += ModSupport.Myst.ConfigOptions_OnShowSkillKeybindsChanged;
        }
        if (ModSupport.HUDdleUPMod.ModIsRunning)
        {
            HudChanges.HudDetails.OnHudDetailEditsFinished += ModSupport.HUDdleUPMod.RemoveNewHudPanelBackgrounds;
        }
    }
}