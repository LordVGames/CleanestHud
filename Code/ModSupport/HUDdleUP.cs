using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CleanestHud.ModSupport
{
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



        internal static void RemoveNewHudPanelBackgrounds()
        {
            if (HudResources.ImportantHudTransforms.RightInfoBar == null)
            {
                return;
            }

            HudResources.ImportantHudTransforms.RightInfoBar.Find("LootPanel")?.DisableImageComponent();
            HudResources.ImportantHudTransforms.RightInfoBar.Find("RailgunnerAccuracyPanel")?.DisableImageComponent();
            HudResources.ImportantHudTransforms.RightInfoBar.Find("BanditComboPanel")?.DisableImageComponent();
            HudResources.ImportantHudTransforms.RightInfoBar.Find("ConnectionPanel")?.DisableImageComponent();
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
}
