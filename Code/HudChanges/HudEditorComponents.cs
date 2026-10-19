using CleanestHud.HudChanges.NormalHud.SkillsAndEquipsArea;
using RoR2.UI;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
namespace CleanestHud.HudChanges;


public static class HudEditorComponents
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


    public class TransformLocalPositionSetter : MonoBehaviour
    {
        public Transform rectTransform;
        public Vector3 NewLocalPosition;


        public void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            NewLocalPosition = this.transform.localPosition;
        }

        
        public void Update()
        {
            rectTransform.localPosition = NewLocalPosition;
        }


        public void SetNewLocalPositionForSetterDelayed(Vector3 vector3)
        {
            Main.MyHud?.StartCoroutine(DoTheThingDelayed(vector3));
        }
        private IEnumerator DoTheThingDelayed(Vector3 vector3)
        {
            yield return null;
            this.NewLocalPosition = vector3;
            yield break;
        }
    }
}