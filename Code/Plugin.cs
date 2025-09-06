using System;
using BepInEx;
using RoR2;
using R2API.Utils;
using HarmonyLib;
using MiscFixes.Modules;

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
        public const string PluginVersion = "1.1.0";
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public void Awake()
        {
            PluginInfo = Info;
            Log.Init(Logger);
            ModAssets.Init();
            ConfigOptions.BindConfigOptions(Config);
            Config.WipeConfig();
            // asset edits need to happen very early because once the hud is fully initialized a prefab may have already been spawned and then it can't be edited this way
            HudResources.HudAssets.SetupAssets();



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
            On.RoR2.VoidSurvivorController.OnOverlayInstanceAdded += HudChanges.SurvivorSpecific.VoidFiend.VoidSurvivorController_OnOverlayInstanceAdded;
            On.EntityStates.Seeker.Meditate.SetupInputUIIcons += HudChanges.SurvivorSpecific.Seeker.Meditate_SetupInputUIIcons;

            IL.RoR2.BossGroup.UpdateObservations += Main.ILHooks.BossGroup_UpdateObservations;
            IL.RoR2.UI.BuffDisplay.UpdateLayout += Main.ILHooks.BuffDisplay_UpdateLayout;
            IL.RoR2.UI.ItemIcon.SetItemIndex += Main.ILHooks.ItemIcon_SetItemIndex;
            IL.RoR2.EscapeSequenceController.SetHudCountdownEnabled += Main.ILHooks.EscapeSequenceController_SetHudCountdownEnabled;
            //IL.RoR2.UI.HealthBar.UpdateBarInfos += Main.ILHooks.HealthBar_UpdateBarInfos;

            InfiniteTowerRun.onWaveInitialized += Main.Events.InfiniteTowerRun_onWaveInitialized;
            Run.onRunStartGlobal += Main.Events.Run_onRunStartGlobal;
            RunArtifactManager.onArtifactEnabledGlobal += Main.Events.RunArtifactManager_onArtifactEnabledGlobal;

            Main.OnSurvivorSpecificHudEditsFinished += HudChanges.SurvivorSpecific.Seeker.RepositionSeekerLotusUI;
            ConfigOptions.OnShowSkillKeybindsChanged += HudChanges.SurvivorSpecific.Seeker.RepositionSeekerLotusUI;

            Main.OnSurvivorSpecificHudEditsFinished += HudChanges.SurvivorSpecific.VoidFiend.SetupViendEdits;

            if (ModSupport.Starstorm2.ModIsRunning)
            {
                CharacterBody.onBodyInventoryChangedGlobal += ModSupport.Starstorm2.CharacterBody_onBodyInventoryChangedGlobal;
            }
            if (ModSupport.LookingGlassMod.ModIsRunning)
            {
                HudChanges.HudDetails.OnHudDetailEditsFinished += ModSupport.LookingGlassMod.OnHudDetailEditsFinished;
            }
            if (ModSupport.Driver.ModIsRunning)
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
            }
            if (ModSupport.Myst.ModIsRunning)
            {
                Main.OnSurvivorSpecificHudEditsFinished += ModSupport.Myst.OnSurvivorSpecificHudEditsFinished;
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
}