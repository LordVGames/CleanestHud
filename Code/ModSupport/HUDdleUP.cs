using CleanestHud.HudChanges;
using CleanestHud.HudChanges.SurvivorSpecific;
using HUDdleUP.Bandit;
using HUDdleUP.Loot;
using HUDdleUP.Multiplayer;
using HUDdleUP.Railgunner;
using MonoDetour;
using MonoDetour.DetourTypes;
using MonoDetour.HookGen;
using RoR2.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static CleanestHud.HudResources.ImportantHudTransforms;
namespace CleanestHud.ModSupport;


internal class HUDdleUPMod
{
    private static bool? _modexists;
    internal static bool ModIsRunning
    {
        get
        {
            _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(HUDdleUP.Plugin.GUID);
            return (bool)_modexists;
        }
    }


    [MonoDetourTargets(typeof(DronePanel))]
    [MonoDetourTargets(typeof(LootPanel))]
    [MonoDetourTargets(typeof(ConnectionPanel))]
    [MonoDetourTargets(typeof(RailgunnerAccuracyPanel))]
    [MonoDetourTargets(typeof(BanditComboPanel))]
    private static class Hooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            if (!ModIsRunning)
            {
                return;
            }


            Mdh.HUDdleUP.Loot.LootPanel.Start.Postfix(LootPanel_Start);
            Mdh.HUDdleUP.Loot.DronePanel.Start.Postfix(DronePanel_Start);
            Mdh.HUDdleUP.Multiplayer.ConnectionPanel.Start.Postfix(ConnectionPanel_Start);
            Mdh.HUDdleUP.Railgunner.RailgunnerAccuracyPanel.Start.Postfix(RailgunnerAccuracyPanel_Start);
            Mdh.HUDdleUP.Bandit.BanditComboPanel.Start.Postfix(BanditComboPanel_Start);
        }


        private static void BanditComboPanel_Start(BanditComboPanel self)
        {
            if (!ConfigOptions.AllowHudDetailsEdits.Value)
            {
                return;
            }


            self.panel.gameObject.DisableImageComponent();
            self.panel.label.gameObject.SetActive(false);
        }


        private static void RailgunnerAccuracyPanel_Start(RailgunnerAccuracyPanel self)
        {
            if (!ConfigOptions.AllowHudDetailsEdits.Value)
            {
                return;
            }


            self.panel.gameObject.DisableImageComponent();
            self.panel.label.gameObject.SetActive(false);
        }


        private static void ConnectionPanel_Start(ConnectionPanel self)
        {
            if (!ConfigOptions.AllowHudDetailsEdits.Value)
            {
                return;
            }


            self.panel.gameObject.DisableImageComponent();
            self.panel.label.gameObject.SetActive(false);
        }


        private static void DronePanel_Start(DronePanel self)
        {
            if (!ConfigOptions.AllowHudDetailsEdits.Value)
            {
                return;
            }


            self.panel.gameObject.DisableImageComponent();
            self.panel.label.gameObject.SetActive(false);
        }


        private static void LootPanel_Start(LootPanel self)
        {
            if (!ConfigOptions.AllowHudDetailsEdits.Value)
            {
                return;
            }


            self.panel.gameObject.DisableImageComponent();
            self.panel.label.gameObject.SetActive(false);
        }
    }


    internal static void EditHuddleUpEquipmentCooldownOverlay()
    {
        Transform equipmentSlot = Main.MyHud.equipmentIcons[0].transform;
        if (equipmentSlot == null)
        {
            return;
        }
        Transform cooldownPanel = equipmentSlot.Find("CooldownPanel(Clone)");
        Transform displayRoot = equipmentSlot.Find("DisplayRoot");
        if (cooldownPanel == null || displayRoot == null)
        {
            return;
        }


        cooldownPanel.localPosition = displayRoot.localPosition;
        cooldownPanel.localScale = displayRoot.localScale;
    }
}
