using EntityStates.Missions.ChampRush;
using MonoDetour;
using MonoDetour.HookGen;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;
namespace CleanestHud.HudChanges.NormalHud;


[MonoDetourTargets(typeof(EntityStates.Missions.ChampRush.StartChampRush))]
internal static class TrialCountdown
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.EntityStates.Missions.ChampRush.StartChampRush.OnEnter.Postfix(BossRushStart);
    }


    private static void BossRushStart(StartChampRush self)
    {
        //MyHud?.StartCoroutine(DelayThenEditPanel());
    }


    private static IEnumerator DelayThenEditPanel()
    {
        yield return new WaitForSeconds(1.5f);
        EditTrialHud();
        yield break;
    }


    private static void EditTrialHud()
    {
        // the GetChild(0) x2 is to go into "Juice" then "Container" transforms
        Transform trialHudCountdown = TopCenterCluster.Find("TrialHudCountdownPanel(Clone)").GetChild(0).GetChild(0);
    }
}