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

            public void LateUpdate()
            {
                if (backgroundImage.color.a < newColor.a)
                {
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
            Transform longBackground;

            RectTransform classBackgroundRect;
            public Vector3 ClassBackgroundRectLocalPosition;
            public Vector3 ClassBackgroundRectLocalScale;
            public Vector2 ClassBackgroundRectPivot;

            RectTransform itemsBackgroundRect;
            public Vector3 ItemsBackgroundRectLocalPosition;
            public Vector2 ItemsBackgroundRectPivot;

            RectTransform equipmentBackgroundRect;
            public Vector3 EquipmentBackgroundRectLocalPosition;
            public Vector3 EquipmentBackgroundRectLocalScale;
            public Vector2 EquipmentBackgroundRectPivot;

            public void Start()
            {
                longBackground = this.transform.GetChild(0);
                classBackgroundRect = longBackground.Find("ClassBackground").GetComponent<RectTransform>();
                equipmentBackgroundRect = longBackground.Find("EquipmentBackground").GetComponent<RectTransform>();
                itemsBackgroundRect = longBackground.Find("ItemsBackground").GetComponent<RectTransform>();
            }

            public void Update()
            {
                if (itemsBackgroundRect.localPosition != ItemsBackgroundRectLocalPosition)
                {
                    itemsBackgroundRect.localPosition = ItemsBackgroundRectLocalPosition;
                    itemsBackgroundRect.pivot = ItemsBackgroundRectPivot;
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
