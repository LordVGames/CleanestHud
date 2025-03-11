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
            private RectTransform classBackgroundRect;
            public Vector3 ClassBackgroundRect_LocalPosition;
            public Vector3 ClassBackgroundRect_LocalScale;
            public Vector2 ClassBackgroundRect_Pivot;

            private RectTransform itemsBackgroundRect;
            public Vector3 ItemsBackgroundRectLocalPosition;
            public Vector2 ItemsBackgroundRectPivot;
            public Vector2 ItemsBackgroundAnchoredPosition;
            public Vector2 ItemsBackgroundSizeDelta;


            private RectTransform equipmentBackgroundRect;
            public Vector3 EquipmentBackgroundRectLocalPosition;
            public Vector3 EquipmentBackgroundRectLocalScale;
            public Vector2 EquipmentBackgroundRectPivot;

            private RectTransform nameLabelRect;
            public Vector3 NameLabelRectLocalPosition;

            public void Start()
            {
                Transform longBackground = this.transform.GetChild(0);
                classBackgroundRect = longBackground.GetChild(1).GetComponent<RectTransform>();
                equipmentBackgroundRect = longBackground.GetChild(7).GetComponent<RectTransform>();
                itemsBackgroundRect = longBackground.GetChild(6).GetComponent<RectTransform>();
                nameLabelRect = longBackground.GetChild(2).GetComponent<RectTransform>();

                CalculateAndSetWidthBasedPositions();
            }

            public void CalculateAndSetWidthBasedPositions()
            {
                Log.Debug("CalculateAndSetWidthBasedPositions");
                Transform longBackground = this.transform.GetChild(0);
                RectTransform longBackgroundRect = longBackground.GetComponent<RectTransform>();

                // the X position is based on the size of the entire strip's rect width
                // this way it'll work no matter the screen width
                // the +2 at the end is to keep the icon from going out of the strip
                ClassBackgroundRect_LocalPosition = new Vector3((longBackgroundRect.rect.xMin + classBackgroundRect.sizeDelta.x / 2) + 2, classBackgroundRect.localPosition.y, classBackgroundRect.localPosition.z);
                Log.Debug($"ClassBackgroundRect_LocalPosition is {ClassBackgroundRect_LocalPosition}");
            }

            public void Update()
            {
                if (itemsBackgroundRect.localPosition != ItemsBackgroundRectLocalPosition)
                {
                    itemsBackgroundRect.localPosition = ItemsBackgroundRectLocalPosition;
                    itemsBackgroundRect.pivot = ItemsBackgroundRectPivot;
                    itemsBackgroundRect.anchoredPosition = ItemsBackgroundAnchoredPosition;
                    itemsBackgroundRect.sizeDelta = ItemsBackgroundSizeDelta;
                    // it clips into the money/item numbers so it needs to be a lil less wide
                    Vector3 newScale = itemsBackgroundRect.localScale;
                    newScale.x = 0.95f;
                    itemsBackgroundRect.localScale = newScale;
                }

                if (equipmentBackgroundRect.localPosition != EquipmentBackgroundRectLocalPosition)
                {
                    equipmentBackgroundRect.localPosition = EquipmentBackgroundRectLocalPosition;
                    equipmentBackgroundRect.localScale = EquipmentBackgroundRectLocalScale;
                    equipmentBackgroundRect.pivot = EquipmentBackgroundRectPivot;
                }

                if (classBackgroundRect.localPosition != ClassBackgroundRect_LocalPosition)
                {
                    // make classbackground go to left edge of scoreboardstrip - regardless of the screen width
                    classBackgroundRect.localPosition = ClassBackgroundRect_LocalPosition;
                    classBackgroundRect.localScale = ClassBackgroundRect_LocalScale;
                    classBackgroundRect.pivot = ClassBackgroundRect_Pivot;
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