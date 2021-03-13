using Common.Control.Impl;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Control
{
	public class TractorControl : BaseControl,IPointerDownHandler,IPointerUpHandler
	{
		public float timeRotate = 0.2f;
		private Vector2 sizeInput;
		private float axisHorizontal;
		private Tweener tween;

		private void Start()
		{
			sizeInput = GetComponent<RectTransform>().rect.size;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			var halfWidth = sizeInput.x / 2;
			tween?.Kill();
			if (eventData.position.x > halfWidth)
			{
				tween = DOTween.To(value => axisHorizontal = value, axisHorizontal, 1, timeRotate);
			}
			else
			{
				tween = DOTween.To(value => axisHorizontal = value, axisHorizontal, -1, timeRotate);
			}
		}
		

		public void OnPointerUp(PointerEventData eventData)
		{
			tween?.Kill();
			tween = DOTween.To(value => axisHorizontal = value, axisHorizontal, 0, timeRotate);
		}

		public override float GetHorizontal()
		{
			return axisHorizontal;
		}

		public override float GetVertical()
		{
			return 0;
		}

		public override bool IsContactGround()
		{
			return false;
		}

		public override void ResetControl()
		{
			tween?.Kill();

			axisHorizontal = 0;
		}
	}
}