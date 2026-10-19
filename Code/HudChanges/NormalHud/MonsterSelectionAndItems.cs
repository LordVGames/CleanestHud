using CleanestHud.HudChanges;
using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using System;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets]
internal static class MonsterSelectionAndItems
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
    }


    // idk what's with the jetbrains stuff here but that's what it autocompleted with so whatever
    internal static void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
    {
        if (artifactDef.cachedName == "MonsterTeamGainsItems" && runArtifactManager.IsArtifactEnabled(artifactDef))
        {
            runArtifactManager?.StartCoroutine(HudDetails.DelayRemoveMonstersItemsPanelDetails());
        }
    }
}