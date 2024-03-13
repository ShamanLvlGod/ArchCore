using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArchCore.Utils
{
	public class CanvasController : MonoBehaviour
	{
		[SerializeField] private Canvas canvas;
		[SerializeField] private CanvasScaler canvasScaler;
		[SerializeField] private Transform viewContainer, popupContainer;
		[SerializeField] private bool autoAdjustScale;


		public Canvas Canvas => canvas;

		public Transform ViewContainer => viewContainer;

		public Transform PopupContainer => popupContainer;

		private void Awake()
		{
			AdjustScale();
		}
		
#if UNITY_EDITOR
		private void Update()
		{
			AdjustScale();
		}
#endif

		void AdjustScale()
		{
			if (autoAdjustScale)
			{
				float cr = Screen.width / (float) Screen.height;
				float sr = 9f / 16f;
				canvasScaler.matchWidthOrHeight = cr > sr + 0.0001f ? 1 : 0;
			}
		}
	}

}