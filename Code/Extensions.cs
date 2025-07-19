using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CleanestHud
{
    internal static class Extensions
    {
        internal static Transform FindWithPartialMatch(this Transform thisTransform, string partialName)
        {
            if (thisTransform.childCount == 0)
            {
                return null;
            }
            for (int i = 0; i < thisTransform.childCount; i++)
            {
                Transform child = thisTransform.GetChild(i);
                if (child.name.Contains(partialName))
                {
                    return child;
                }
            }
            return null;
        }

        internal static List<Transform> FindListOfPartialMatches(this Transform thisTransform, string partialName)
        {
            List<Transform> list = [];
            if (thisTransform.childCount == 0)
            {
                return null;
            }
            for (int i = 0; i < thisTransform.childCount; i++)
            {
                Transform child = thisTransform.GetChild(i);
                if (child.name.Contains(partialName))
                {
                    list.Add(child);
                }
            }
            return list;
        }



        internal static void DisableImageComponent(this Transform transform)
        {
            if (transform.TryGetComponent<Image>(out Image imageComponent))
            {
                imageComponent.enabled = false;
            }
            else
            {
                Log.Error($"Could not find image component in transform {transform.name}");
            }
        }

        internal static void DisableImageComponent(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent<Image>(out Image imageComponent))
            {
                imageComponent.enabled = false;
            }
            else
            {
                Log.Error($"Could not find image component in GameObject {gameObject.name}");
            }
        }

        internal static void DisableImageComponent(this AllyCardController allyCardController)
        {
            if (allyCardController.TryGetComponent<Image>(out Image imageComponent))
            {
                imageComponent.enabled = false;
            }
            else
            {
                Log.Error($"Could not find image component in allyCardController {allyCardController.name}");
            }
        }



        internal static void DisableRawImageComponent(this Transform transform)
        {
            if (transform.TryGetComponent<RawImage>(out RawImage imageComponent))
            {
                imageComponent.enabled = false;
            }
            else
            {
                Log.Error($"Could not find image component in transform {transform.name}");
            }
        }

        internal static void DisableRawImageComponent(this GameObject gameObject)
        {
            if (gameObject.TryGetComponent<RawImage>(out RawImage imageComponent))
            {
                imageComponent.enabled = false;
            }
            else
            {
                Log.Error($"Could not find image component in allyCardController {gameObject.name}");
            }
        }
    }
}
