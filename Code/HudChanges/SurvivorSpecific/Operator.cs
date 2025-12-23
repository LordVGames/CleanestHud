using System;
using RoR2;
using static CleanestHud.Main;
using static CleanestHud.HudResources.ImportantHudTransforms;
using UnityEngine;
using UnityEngine.UI;
using MonoDetour.HookGen;
using MonoDetour;
using System.Collections.Generic;
namespace CleanestHud.HudChanges.SurvivorSpecific;


internal static class Operator
{
    private static Transform _droneQueueUI;
    private static ChildLocator _droneQueueUIChildLocator;
    private static bool IsHudEditable
    {
        get
        {
            return Main.IsHudEditable
            && ConfigOptions.AllowSurvivorSpecificEdits.Value
            && HudCameraTargetBody != null
            && HudCameraTargetBody.bodyIndex == DLC3Content.BodyPrefabs.DroneTechBody.bodyIndex;
        }
    }


    // why is this not even in the ror2 class? it's just sitting at the root of ror2's code
    [MonoDetourTargets(typeof(DroneTechSurvivorUIController))]
    private static class Hooks
    {
        [MonoDetourHookInitialize]
        private static void Setup()
        {
            Mdh.DroneTechSurvivorUIController.TryUpdateIcons.Postfix(TryUpdateIcons);
        }

        private static void TryUpdateIcons(DroneTechSurvivorUIController droneUI, ref List<DroneInfo> DroneQueue, ref List<DroneInfo> AllDrones, ref bool returnValue)
        {
            if (!returnValue || droneUI == null || !IsHudEditable)
            {
                return;
            }
            if (_droneQueueUI == null || _droneQueueUIChildLocator == null)
            {
                _droneQueueUI = droneUI.overlayInstanceChildLocator.FindChild("Secondary Queue");
                _droneQueueUIChildLocator = _droneQueueUI.GetComponent<ChildLocator>();
            }


            // local position ingame doesn't say it's different after this change but it is
            _droneQueueUI.localPosition = new Vector3(-192, -64, 0);
            for (int i = 1; i < 4; i++)
            {
                Transform droneSlot = _droneQueueUIChildLocator.FindChild($"Drone {i}");
                // there's another ChildLocator component on each of these but if i don't have to use them i won't in order to save on GetComponent calls
                // also the first one is setup differently from the other 2 slots so
                if (i == 1)
                {
                    Transform slotOutline = droneSlot.Find("Outline");
                    Color droneTypeColor = slotOutline.GetComponent<RawImage>().color;
                    slotOutline.gameObject.SetActive(false);

                    // background color for current commandable drone
                    Transform noDrone = droneSlot.Find("NoDrone");
                    noDrone.gameObject.SetActive(true);
                    if (DroneQueue != null && DroneQueue.Count > 0 && DroneQueue[0] != null)
                    {
                        noDrone.GetComponent<Image>().color = Helpers.GetAdjustedColor(droneTypeColor, transparencyMultiplier: 0.5f, saturationMultiplier: 0.9f);
                    }
                }
                else
                {
                    droneSlot.Find("Background").gameObject.SetActive(false);
                }
            }
        }
    }
}