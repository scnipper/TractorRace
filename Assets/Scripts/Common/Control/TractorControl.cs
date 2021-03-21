using System.Collections;
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
		private Tweener tween;
		private Tweener tweenLadle;
		private float ladleDelta;
		private bool isGroundContact;
		private bool isDown;
		private float delayDown;

		private void Start()
		{
			sizeInput = GetComponent<RectTransform>().rect.size;
			DownLadle();
			StartCoroutine(DelayDown());
		}

		private IEnumerator DelayDown()
		{
			while (true)
			{
				if (delayDown > 0)
				{
					delayDown -= Time.deltaTime;
					if (delayDown <= 0)
					{
						if(!isDown) DownLadle();
					}
				}
				

				yield return null;
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			isDown = true;
			delayDown = 0;
			UpLadle();
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
			isDown = false;
			delayDown = 0.5f;

			tween?.Kill();
			tween = DOTween.To(value => axisHorizontal = value, axisHorizontal, 0, timeRotate);
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
	}
}