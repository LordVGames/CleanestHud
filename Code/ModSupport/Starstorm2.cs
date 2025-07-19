using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using HarmonyLib;
using RoR2;
using RoR2.UI;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using SS2;
using static CleanestHud.Main;
using CleanestHud.HudChanges;
using static CleanestHud.HudChanges.HudColor;
using MiscFixes.Modules;

namespace CleanestHud.ModSupport
{
    // do not rename class to SS2
    internal class Starstorm2
    {
        private static bool? _modexists;
        internal static bool ModIsRunning
        {
            get
            {
                _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(SS2Main.GUID);
                return (bool)_modexists;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (!IsHudFinishedLoading)
            {
                Log.Debug("onBodyInventoryChangedGlobal happened while the HUD was not done loading, returning");
                return;
            }

            int currentInjectorItemCount = characterBody.inventory.GetItemCount(SS2Content.Items.CompositeInjector);
            if (currentInjectorItemCount != CompositeInjectorSupport.KnownInjectorSlotCount)
            {
                CompositeInjectorSupport.KnownInjectorSlotCount = currentInjectorItemCount;
                MyHud?.StartCoroutine(CompositeInjectorSupport.DelayEditInjectorSlots());
                MyHud?.StartCoroutine(CompositeInjectorSupport.DelayColorInjectorSlots());
            }
        }

        internal static class CompositeInjectorSupport
        {
            internal static int KnownInjectorSlotCount = -1;

            internal static IEnumerator DelayEditInjectorSlots()
            {
                yield return null;
                EditInjectorSlots();
            }
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void EditInjectorSlots()
            {
                if (!ModIsRunning || !IsHudFinishedLoading)
                {
                    return;
                }
                SS2.Items.CompositeInjector.IconHolder ss2EquipmentIconHolder;
                bool compositeInjectorExists = MyHud.gameObject.TryGetComponent<SS2.Items.CompositeInjector.IconHolder>(out ss2EquipmentIconHolder);
                if (!compositeInjectorExists)
                {
                    return;
                }

                for (int i = 0; i < ss2EquipmentIconHolder.icons.Length; i++)
                {
                    if (!ss2EquipmentIconHolder.icons[i].displayRoot.activeSelf)
                    {
                        return;
                    }
                    Transform injectorSlotDisplayRoot = ss2EquipmentIconHolder.icons[i].displayRoot.transform;

                    // base position is moved enough away from mul-t's other equipment slot
                    // the position is also manually set because the postition & localposition change at some point while loading for no reason
                    // even if i store the position after i've modified it it still gets randomly changed
                    //ss2EquipmentIconHolder.icons[i].transform.position = new Vector3(4.5f, -5.2f, 12.8f);

                    // set localposition of injector icon displayroot to the mul-t alt equipment icon displayroot
                    ss2EquipmentIconHolder.icons[i].transform.GetChild(0).position = MyHud.equipmentIcons[1].transform.GetChild(0).position;
                    RectTransform displayRootRect = ss2EquipmentIconHolder.icons[i].transform.GetChild(0).GetComponent<RectTransform>();
                    Vector3 localPositionChange = Vector3.zero;
                    localPositionChange.x = displayRootRect.rect.width * 3;
                    // divide by 10 as an int for the row number
                    localPositionChange.y -= 107 * (int)(i / 10);
                    if (i < 10)
                    {
                        // modulus 10 for the right x position regardless of row
                        localPositionChange.x += 100 * (i % 10);
                    }
                    else
                    {
                        // make the injector slots past the 10th start at the 5th slot's x coordinate
                        // this is to make it not overlap with the hp bar at giga ultrawide resolutions (i.e. 3840x1080)
                        localPositionChange.x += 100 * (((i - 11) % 10) + 5);
                    }
                    ss2EquipmentIconHolder.icons[i].transform.localPosition += localPositionChange;

                    // the isready panel is weirdly not aligned + injector slots never have cooldown text anyways so they're getting removed
                    Transform injectorSlotIsReadyPanel = injectorSlotDisplayRoot.GetChild(0);
                    injectorSlotIsReadyPanel.gameObject.SetActive(false);
                    // we can still use it to check if we've scaled the slot already or not
                    Vector3 alreadyScaled = new(1.1f, 1.1f, 1.1f);
                    if (injectorSlotIsReadyPanel.localScale != alreadyScaled)
                    {
                        HudStructure.ScaleEquipmentSlot(injectorSlotDisplayRoot, 1.1f);
                    }
                }
            }

            internal static IEnumerator DelayColorInjectorSlots()
            {
                yield return null;
                ColorInjectorSlots();
            }
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void ColorInjectorSlots()
            {
                if (!ModIsRunning || !IsHudFinishedLoading)
                {
                    return;
                }
                SS2.Items.CompositeInjector.IconHolder ss2EquipmentIconHolder;
                bool compositeInjectorExists = MyHud.gameObject.TryGetComponent<SS2.Items.CompositeInjector.IconHolder>(out ss2EquipmentIconHolder);
                if (!compositeInjectorExists)
                {
                    return;
                }

                foreach (SS2.Items.CompositeInjector.EquipmentIconButEpic injectorSlot in ss2EquipmentIconHolder.icons)
                {
                    if (!injectorSlot.displayRoot.activeSelf)
                    {
                        // doing return instead of continue since if we've hit an inactive slot that means the rest will also be inactive
                        return;
                    }

                    Transform injectorSlotDisplayRoot = injectorSlot.displayRoot.transform;

                    Transform injectorSlotIsReadyPanel = injectorSlotDisplayRoot.GetChild(0);
                    Image injectorSlotEquipmentIsReadyPanelImage = injectorSlotIsReadyPanel.GetComponent<Image>();
                    injectorSlotEquipmentIsReadyPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);

                    Transform injectorSlotBGPanel = injectorSlotDisplayRoot.Find("BGPanel");
                    Image injectorSlotBGPanelImage = injectorSlotBGPanel.GetComponent<Image>();
                    injectorSlotBGPanelImage.color = Main.Helpers.GetAdjustedColor(SurvivorColor, colorIntensityMultiplier: DefaultHudColorIntensity);
                }
            }
        }
    }
}