using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace CleanestHud.HudChanges
{
    internal class HudEditorComponents
    {
        public class DifficultyBarBackgroundTransparencyRemover : MonoBehaviour
        {
            public Color survivorColorNoTransparency;
            private Image BackgroundImage;

            public void Start()
            {
                BackgroundImage = GetComponent<Image>();
                survivorColorNoTransparency = HudColor.SurvivorColor;
                survivorColorNoTransparency.a = 1;
            }

            public void LateUpdate()
            {
                if (BackgroundImage.color.a < 1)
                {
                    BackgroundImage.color = survivorColorNoTransparency;
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
            private bool hasStarted = false;
            // local position values are made public so they can be edited ingame

            private RectTransform classBackgroundRect;
            /// <remarks>
            /// Don't set manually - the component sets this itself
            /// </remarks>
            public Vector3 ClassBackgroundRect_LocalPosition;
            public Vector3 ClassBackgroundRect_LocalScale;
            public Vector2 ClassBackgroundRect_Pivot;

            private RectTransform itemsBackgroundRect;
            /// <remarks>
            /// Don't set manually - the component sets this itself
            /// </remarks>
            public Vector3 ItemsBackgroundRect_LocalPosition;
            private Image itemsBackgroundImage;

            private RectTransform equipmentBackgroundRect;
            /// <remarks>
            /// Don't set manually - the component sets this itself
            /// </remarks>
            public Vector3 EquipmentBackgroundRect_LocalPosition;
            public Vector3 EquipmentBackgroundRect_LocalScale;
            public Vector2 EquipmentBackgroundRect_Pivot;

            private RectTransform nameLabelRect;
            /// <remarks>
            /// Don't set manually - the component sets this itself
            /// </remarks>
            public Vector3 NameLabelRectLocalPosition;
            private Vector3 normalNameLabelRectLocalPosition;

            private Transform itemHeader;
            /// <remarks>
            /// Don't set manually - the component sets this itself
            /// </remarks>
            public Vector3 ItemHeader_LocalPosition;

            private Transform playerHeader;

            public void Start()
            {
                // scoreboardstrip > stripcontainer > container > headergroup
                Transform headerGroup = this.transform.parent.parent.GetChild(0);
                playerHeader = headerGroup.GetChild(0);
                itemHeader = headerGroup.GetChild(1);

                Transform longBackground = this.transform.GetChild(0);
                classBackgroundRect = longBackground.GetChild(1).GetComponent<RectTransform>();
                equipmentBackgroundRect = longBackground.GetChild(7).GetComponent<RectTransform>();
                Transform itemsBackground = longBackground.GetChild(6);
                itemsBackgroundRect = itemsBackground.GetComponent<RectTransform>();
                itemsBackgroundImage = itemsBackground.GetComponent<Image>();

                nameLabelRect = longBackground.GetChild(2).GetComponent<RectTransform>();
                normalNameLabelRectLocalPosition = nameLabelRect.localPosition;

                hasStarted = true;
                CalculateAndSetSizeBasedPositions();
            }

            public void CalculateAndSetSizeBasedPositions()
            {
                // this is stupid
                if (!hasStarted)
                {
                    return;
                }

                Transform longBackground = this.transform.GetChild(0);
                RectTransform longBackgroundRect = longBackground.GetComponent<RectTransform>();

                // some position coordinates are based on the size of the entire strip's rect width
                // this way they'll be positioned nicely no matter the screen width
                // the +/-2 at the end of the next 2 are to keep the icons from going out of the strip
                ClassBackgroundRect_LocalPosition = new Vector3(
                    (longBackgroundRect.rect.xMin + classBackgroundRect.sizeDelta.x / 2) + 2,
                    classBackgroundRect.localPosition.y,
                    classBackgroundRect.localPosition.z
                );
                EquipmentBackgroundRect_LocalPosition = new Vector3(
                    ((longBackgroundRect.rect.xMin * -1) - equipmentBackgroundRect.sizeDelta.x / 2) - 2,
                    equipmentBackgroundRect.localPosition.y,
                    equipmentBackgroundRect.localPosition.z
                );
                ItemsBackgroundRect_LocalPosition = new Vector3(
                    -170,
                    (int)Math.Ceiling(longBackgroundRect.rect.height * 0.07f),
                    itemsBackgroundRect.localPosition.z
                );
                ItemHeader_LocalPosition = new Vector3(
                    ItemsBackgroundRect_LocalPosition.x + (itemsBackgroundRect.sizeDelta.x / 2),
                    playerHeader.localPosition.y,
                    itemHeader.localPosition.z
                );

                if (ModSupport.LookingGlassMod.ModIsRunning)
                {
                    if (ModSupport.LookingGlassMod.ItemCountersConfigValue)
                    {
                        NameLabelRectLocalPosition = normalNameLabelRectLocalPosition;
                    }
                }
                else
                {
                    NameLabelRectLocalPosition = new Vector3(
                        -324.08f - (nameLabelRect.sizeDelta.x / 2),
                        normalNameLabelRectLocalPosition.y,
                        normalNameLabelRectLocalPosition.z
                    );
                }
            }

            public void Update()
            {
                // make background image invisible instead of disabling to allow game's automatic ui scaling to happen
                if (itemsBackgroundImage.color.a != 0)
                {
                    itemsBackgroundImage.color = Color.clear;
                }
                if (itemsBackgroundRect.localPosition.x != 0)
                {
                    itemsBackgroundRect.localPosition = ItemsBackgroundRect_LocalPosition;
                }

                if (itemHeader.localPosition != ItemHeader_LocalPosition)
                {
                    itemHeader.localPosition = ItemHeader_LocalPosition;
                }

                if (equipmentBackgroundRect.localPosition != EquipmentBackgroundRect_LocalPosition)
                {
                    equipmentBackgroundRect.localPosition = EquipmentBackgroundRect_LocalPosition;
                    equipmentBackgroundRect.localScale = EquipmentBackgroundRect_LocalScale;
                    equipmentBackgroundRect.pivot = EquipmentBackgroundRect_Pivot;
                }

                if (classBackgroundRect.localPosition != ClassBackgroundRect_LocalPosition)
                {
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