using System;
using RoR2;
namespace CleanestHud.HudChanges.NormalHud;


internal static class MonsterSelectionAndItems
{
    // idk what's with the jetbrains stuff here but that's what it autocompleted with so whatever
    internal static void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
    {
        if (artifactDef.cachedName == "MonsterTeamGainsItems" && runArtifactManager.IsArtifactEnabled(artifactDef))
        {
            runArtifactManager?.StartCoroutine(HudDetails.DelayRemoveMonstersItemsPanelDetails());
        }
    }
}