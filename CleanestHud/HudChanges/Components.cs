using UnityEngine;
using UnityEngine.UI;

namespace CleanestHud.HudChanges
{
    internal class Components
    {
        public class DifficultyBarBackgroundColorChanger : MonoBehaviour
        {
            public Color newColor;
            public Image backgroundImage;

            public void Start()
            {
                backgroundImage = GetComponent<Image>();
            }

            public void Update()
            {
                Log.Debug($"backgroundImage.color.a is {backgroundImage.color.a}");
                Log.Debug($"newColor.a is {newColor.a}");
                if (backgroundImage.color.a < newColor.a)
                {
                    Log.Debug("resetting color");
                    backgroundImage.color = newColor;
                }
            }
        }

        public class DifficultyScalingBarColorChanger : MonoBehaviour
        {
            public Color newColor;
            public Image segment;

            public void Start()
            {
                segment = GetComponent<Image>();
            }

            public void Update()
            {
                segment.color = newColor;
            }
        }

        public class ScoreboardStripEditor : MonoBehaviour
        {
            RectTransform classBackgroundRect;
            public Vector3 ClassBackgroundRectLocalPosition;
            public Vector3 ClassBackgroundRectLocalScale;
            public Vector2 ClassBackgroundRectPivot;

            RectTransform itemsBackgroundRect;
            public Vector3 ItemsBackgroundRectLocalPosition;
            public Vector2 ItemsBackgroundRectPivot;
            public Vector2 ItemsBackgroundAnchoredPosition;
            public Vector2 ItemsBackgroundSizeDelta;


            RectTransform equipmentBackgroundRect;
            public Vector3 EquipmentBackgroundRectLocalPosition;
            public Vector3 EquipmentBackgroundRectLocalScale;
            public Vector2 EquipmentBackgroundRectPivot;

            RectTransform nameLabelRect;
            public Vector3 NameLabelRectLocalPosition;

            public void Start()
            {
                Transform longBackground = this.transform.GetChild(0);
                classBackgroundRect = longBackground.GetChild(1).GetComponent<RectTransform>();
                equipmentBackgroundRect = longBackground.GetChild(7).GetComponent<RectTransform>();
                itemsBackgroundRect = longBackground.GetChild(6).GetComponent<RectTransform>();
                nameLabelRect = longBackground.GetChild(2).GetComponent<RectTransform>();
            }

            public void Update()
            {
                if (itemsBackgroundRect.localPosition != ItemsBackgroundRectLocalPosition)
                {
                    itemsBackgroundRect.localPosition = ItemsBackgroundRectLocalPosition;
                    itemsBackgroundRect.pivot = ItemsBackgroundRectPivot;
                    itemsBackgroundRect.anchoredPosition = ItemsBackgroundAnchoredPosition;
                    itemsBackgroundRect.sizeDelta = ItemsBackgroundSizeDelta;
                }

                if (equipmentBackgroundRect.localPosition != EquipmentBackgroundRectLocalPosition)
                {
                    equipmentBackgroundRect.localPosition = EquipmentBackgroundRectLocalPosition;
                    equipmentBackgroundRect.localScale = EquipmentBackgroundRectLocalScale;
                    equipmentBackgroundRect.pivot = EquipmentBackgroundRectPivot;
                }

                if (classBackgroundRect.localPosition != ClassBackgroundRectLocalPosition)
                {
                    classBackgroundRect.localPosition = ClassBackgroundRectLocalPosition;
                    classBackgroundRect.localScale = ClassBackgroundRectLocalScale;
                    classBackgroundRect.pivot = ClassBackgroundRectPivot;
                }

                if (nameLabelRect.localPosition != NameLabelRectLocalPosition)
                {
                    nameLabelRect.localPosition = NameLabelRectLocalPosition;
                }
            }
        }

        public class SimulacrumBarEditor : MonoBehaviour
        {
            public Vector3 idealLocalPosition;
            public Vector3 idealLocalScale;
            public RectTransform rectTrans;

            public void Start()
            {
                rectTrans = GetComponent<RectTransform>();
            }

            // positioning needs to be done on LateUpdate instead of Update otherwise it will have no effect due to the fillbar's animator
            public void LateUpdate()
            {
                rectTrans.localPosition = idealLocalPosition;
                rectTrans.localScale = idealLocalScale;
            }
        }

        public class SimulacrumBarColorChanger : MonoBehaviour
        {
            public Image fillBar;
            public Color newFillBarColor;

            public void Start()
            {
                fillBar = GetComponent<Image>();
            }

            // LateUpdate is also needed here to get around the animator
            public void LateUpdate()
            {
                fillBar.color = newFillBarColor;
            }
        }
    }
}