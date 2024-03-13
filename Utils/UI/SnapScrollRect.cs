using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace ArchCore.Utils.UI
{
	[RequireComponent(typeof(ScrollRect))]
	public class SnapScrollRect : UIBehaviour, IDragHandler, IEndDragHandler
	{
		[SerializeField] bool autoInit;
		[SerializeField] int screens;
		float[] points;
		public int speed = 50;
		float stepSize;

		ScrollRect scroll;
		bool lerp;
		float target;

		public int Index => index;
		public float Pos => (scroll.horizontal ? scroll.horizontalNormalizedPosition : scroll.verticalNormalizedPosition) * (screens - 1);
		public int Screens => screens;

		[SerializeField] private int index = 0;

		public Action<int> onPageChanged;
		public Action<int> onPageSet;

		public Action onScrolled, onSlided;

		private bool initilized;
		private float f;

		public bool BlockSlide
		{
			get => blockSlide;
			set
			{
				blockSlide = value;
				if (scroll.enabled == blockSlide)
					scroll.enabled = !blockSlide;
			}
		}

		private bool blockSlide;

		private void Start()
		{
			if (autoInit) InitPoints(screens);
		}

		public void InitPoints(int itemCount)
		{
			scroll = GetComponent<ScrollRect>();

			screens = itemCount;

			if (screens < 1) return;


			points = new float[screens];
			if (screens > 1)
			{
				stepSize = 1 / (float) (screens - 1);

				for (int i = 0; i < screens; i++)
				{
					points[i] = i * stepSize;
				}
			}
			else
			{
				points[0] = 0;
			}

			initilized = true;
		}

		void Update()
		{
			if (!initilized || screens <= 1) return;

			if (!lerp) return;
			if (scroll.horizontal)
			{
				scroll.horizontalNormalizedPosition =
					Mathf.SmoothDamp(scroll.horizontalNormalizedPosition, target, ref f, 10f / speed);
				if (Mathf.Abs(scroll.horizontalNormalizedPosition - target) < 0.001)
				{
					scroll.horizontalNormalizedPosition = target;
					lerp = false;
					onPageSet?.Invoke(index);
				}
			}
			else
			{
				Mathf.SmoothDamp(scroll.verticalNormalizedPosition, target, ref f, 10f / speed);
				if (Mathf.Abs(scroll.verticalNormalizedPosition - target) < 0.001)
				{
					scroll.verticalNormalizedPosition = target;
					lerp = false;
					onPageSet?.Invoke(index);
				}
			}

			onScrolled?.Invoke();
			onSlided?.Invoke();
		}

		public void OnEndDrag(PointerEventData data)
		{
			if(BlockSlide) return;
			if (!initilized || screens <= 1) return;

			if (Mathf.Abs(scroll.velocity.x) > 100)
			{
				int delta = scroll.velocity.x > 0 ? -1 : 1;
				index += delta;
				
				if (delta != 0) onPageChanged?.Invoke(Mathf.Clamp(index, 0, screens - 1));
			}
			else
			{
				int newIndex = scroll.horizontal ? FindNearest(scroll.horizontalNormalizedPosition, points) :
					scroll.vertical ? FindNearest(scroll.verticalNormalizedPosition, points) : index;

				//if (index != newIndex && onPageChanged != null) onPageChanged(newIndex);

				index = newIndex;
			}

			MoveToPage(index);
		}

		public void NextPage()
		{
			MoveToPage(index + 1);
		}

		public void PreviousPage()
		{
			MoveToPage(index - 1);
		}

		public void MoveToPage(int pageIndex)
		{
			int tIndex = Mathf.Clamp(pageIndex, 0, screens - 1);
			if (tIndex != index) onPageChanged?.Invoke(tIndex);
			index = tIndex;
			target = points[index];
			lerp = true;

			onPageChanged?.Invoke(index);
		}

		public void SetPage(int pageIndex)
		{
			if (points == null || points.Length == 0) return;

			int tIndex = Mathf.Clamp(pageIndex, 0, screens - 1);
			if (tIndex != index) onPageChanged?.Invoke(tIndex);
			index = tIndex;
			target = points[index];
			if (scroll.horizontal) scroll.horizontalNormalizedPosition = target;
			else scroll.verticalNormalizedPosition = target;
			
			onPageSet?.Invoke(pageIndex);
		}

		public void OnDrag(PointerEventData data)
		{
			if(BlockSlide) return;
			onScrolled?.Invoke();
			lerp = false;
		}

		int FindNearest(float f, float[] array)
		{
			float distance = Mathf.Infinity;
			int output = 0;
			for (int index = 0; index < array.Length; index++)
			{
				if (Mathf.Abs(array[index] - f) < distance)
				{
					distance = Mathf.Abs(array[index] - f);
					output = index;
				}
			}

			return output;
		}
	}
}