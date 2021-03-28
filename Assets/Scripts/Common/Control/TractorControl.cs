using Common.Control.Impl;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Control
{
	public class TractorControl : BaseControl,IPointerDownHandler,IPointerUpHandler
	{
		public float timeRotate = 0.25f;
		private Vector2 sizeInput;
		private float axisHorizontal;
		private Sequence tween;
		private Tweener tweenLadle;
		private float ladleDelta;
		private bool isGroundContact;

		private void Start()
		{
			sizeInput = GetComponent<RectTransform>().rect.size;
			DownLadle();
		}
		public void OnPointerDown(PointerEventData eventData)
		{
			var halfWidth = sizeInput.x / 2;
			tween?.Kill();
			Rotate(eventData.position.x > halfWidth);
		}

		private void Rotate(bool isRight)
		{
			tween = DOTween.Sequence();
				
			tween.Append(DOTween.To(value => axisHorizontal = value, axisHorizontal, isRight ? 0.5f : -0.5f, timeRotate));
			tween.Append(DOTween.To(value => axisHorizontal = value, axisHorizontal, isRight ? 1f : -1f, timeRotate));
		}


		private void UpLadle()
		{
			tweenLadle?.Kill();
			tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, 0, 0.15f)
				.SetEase(Ease.Linear)
				.OnComplete(() => isGroundContact = false);
		}

		private void DownLadle()
		{
			tweenLadle?.Kill();
			tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, 1, 0.15f)
				.SetEase(Ease.Linear)
				.OnComplete(() => isGroundContact = true);
		}
		

		public void OnPointerUp(PointerEventData eventData)
		{

			tween?.Kill();
			tween = DOTween.Sequence()
				.Append(DOTween.To(value => axisHorizontal = value, axisHorizontal, 0, timeRotate));
		}

		public override float GetHorizontal()
		{
			return axisHorizontal;
		}

		public override float GetVertical()
		{
			return ladleDelta;
		}

		public override bool IsContactGround()
		{
			return isGroundContact;
		}

		public override void ResetControl()
		{
			tween?.Kill();

			axisHorizontal = 0;
		}

		public override void ForceUpLadle(bool isMoveBack)
		{
			isGroundContact = !isMoveBack;
		}
	}
}