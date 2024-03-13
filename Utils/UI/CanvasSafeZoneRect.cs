using System;
using System.Collections;
using System.Collections.Generic;
using ArchCore.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArchCore.Utils.UI
{
    public class CanvasSafeZoneRect : UIBehaviour
    {
        protected override void Awake()
        {
            Resolve();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            Resolve();
        }

        void Resolve()
        {
            Rect safeArea = Screen.safeArea;

            var rect = transform.GetRectTransform();

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
        }


    }


}
