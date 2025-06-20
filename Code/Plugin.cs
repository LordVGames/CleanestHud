using System;
using BepInEx;
using RoR2;
using R2API.Utils;
using HarmonyLib;

namespace CleanestHud
{
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(MiscFixes.MiscFixesPlugin.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(LookingGlass.PluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(SS2.SS2Main.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RobDriver.DriverPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static PluginInfo PluginInfo { get; private set; }
        public const string PluginAuthor = "LordVGames";
        public const string PluginName = "CleanestHud";
        public const string PluginVersion = "1.0.0";
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public void Awake()
        {
            PluginInfo = Info;
            Log.Init(Logger);
            ModAssets.Init();
            ConfigOptions.BindConfigOptions(Config);
            // prefab changes need to happen very early because when the hud is fully initialized the prefab may have already been spawned and then it can't be edited that way
            HudResources.HudAssets.LoadHudAssets();
            HudChanges.HudDetails.AssetEdits.EditHudElementPrefabDetails();



            On.RoR2.UI.HUD.Awake += Main.OnHooks.HUD_Awake;
            On.RoR2.UI.HUD.OnDestroy += Main.OnHooks.HUD_OnDestroy;
            On.RoR2.CameraModes.CameraModeBase.OnTargetChanged += Main.OnHooks.CameraModeBase_OnTargetChanged;

            On.RoR2.ConVar.BaseConVar.AttemptSetString += Main.OnHooks.BaseConVar_AttemptSetString;
            On.RoR2.UI.AllyCardController.Awake += Main.OnHooks.AllyCardController_Awake;
            On.RoR2.UI.AllyCardController.UpdateInfo += Main.OnHooks.AllyCardController_UpdateInfo;
            On.RoR2.UI.HealthBar.InitializeHealthBar += Main.OnHooks.HealthBar_InitializeHealthBar;
            On.RoR2.UI.DifficultyBarController.OnCurrentSegmentIndexChanged += Main.OnHooks.DifficultyBarController_OnCurrentSegmentIndexChanged;
            On.RoR2.UI.InfiniteTowerWaveProgressBar.OnEnable += Main.OnHooks.InfiniteTowerWaveProgressBar_OnEnable;
            On.RoR2.UI.NotificationUIController.SetUpNotification += Main.OnHooks.NotificationUIController_SetUpNotification;
            On.RoR2.UI.ScoreboardController.Rebuild += Main.OnHooks.ScoreboardController_Rebuild;
            On.RoR2.UI.ScoreboardController.SelectFirstScoreboardStrip += Main.OnHooks.ScoreboardController_SelectFirstScoreboardStrip;
            On.RoR2.VoidSurvivorController.OnOverlayInstanceAdded += Main.OnHooks.VoidSurvivorController_OnOverlayInstanceAdded;
            On.EntityStates.Seeker.Meditate.SetupInputUIIcons += Main.OnHooks.Meditate_SetupInputUIIcons;

            IL.RoR2.BossGroup.UpdateObservations += Main.ILHooks.BossGroup_UpdateObservations;
            IL.RoR2.UI.BuffDisplay.UpdateLayout += Main.ILHooks.BuffDisplay_UpdateLayout;
            IL.RoR2.UI.ItemIcon.SetItemIndex += Main.ILHooks.ItemIcon_SetItemIndex;

            InfiniteTowerRun.onWaveInitialized += Main.Events.InfiniteTowerRun_onWaveInitialized;
            Run.onRunStartGlobal += Main.Events.Run_onRunStartGlobal;
            RunArtifactManager.onArtifactEnabledGlobal += Main.Events.RunArtifactManager_onArtifactEnabledGlobal;

            if (ModSupport.Starstorm2.ModIsRunning)
            {
                CharacterBody.onBodyInventoryChangedGlobal += ModSupport.Starstorm2.CharacterBody_onBodyInventoryChangedGlobal;
            }
            if (ModSupport.LookingGlassMod.ModIsRunning)
            {
                HudChanges.HudDetails.OnHudDetailEditsFinished += ModSupport.LookingGlassMod.OnHudDetailEditsFinished;
            }
            if (ModSupport.DriverMod.ModIsRunning)
            {
                Harmony harmony = new(PluginGUID);
                harmony.CreateClassProcessor(typeof(ModSupport.DriverMod.HarmonyPatches)).Patch();
                HudChanges.HudColor.OnHudColorUpdate += ModSupport.DriverMod.OnHudColorUpdate;
                On.RoR2.UI.HUD.OnDestroy += ModSupport.DriverMod.HUD_OnDestroy;
            }
            if (ModSupport.Myst.ModIsRunning)
            {
                Main.OnSurvivorSpecificHudEditsFinished += ModSupport.Myst.OnSurvivorSpecificHudEditsFinished;
                HudChanges.HudColor.OnHudColorUpdate += ModSupport.Myst.OnHudColorUpdate;
                On.RoR2.UI.HUD.OnDestroy += ModSupport.Myst.HUD_OnDestroy;
            }
        }
    }
}