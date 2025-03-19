using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace CleanestHud
{
    internal static class Extensions
    {
        public static Transform FindWithPartialMatch(this Transform thisTransform, string partialName)
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

        public static List<Transform> FindListOfPartialMatches(this Transform thisTransform, string partialName)
        {
            List<Transform> list = new List<Transform>();
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
    }
}
