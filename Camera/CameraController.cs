using System;
using DG.Tweening;
using UnityEngine;

namespace ArchCore.CameraControl
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private new Camera camera;

		private Transform cachedTransform;

		public Transform Transform => cachedTransform;

		private void Awake()
		{
			cachedTransform = transform;
		}

		//....
		public void SetCameraToPosition(Vector3 pos)
		{
			camera.transform.position = pos;
		}

		public Camera GetCamera() => camera;

		public void ResetState()
		{
			DoTweenKillAll();
			camera.orthographicSize = 6.1f;
			camera.transform.position = new Vector3(0, 0, -10);
			
		}

		public void DoTweenKillAll()
		{
			camera.DOKill();
			cachedTransform.DOKill();
		}
		
		public void SetActive(bool b)
		{
			if(camera.enabled != b)
				camera.enabled = b;
		}
	}
}
