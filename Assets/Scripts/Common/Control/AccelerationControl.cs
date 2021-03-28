using System.Collections;
using Common.Control.Impl;
using DG.Tweening;
using UnityEngine;

namespace Common.Control
{
	public class AccelerationControl : BaseControl
	{
		private Tweener tweenLadle;
		private float ladleDelta;
		private bool isGroundContact;
		private float delayDown;
		private bool isDown;


		private void Start()
		{
			DownLadle();
			StartCoroutine(DelayDown());
		}

		public override float GetHorizontal()
		{
			float axis = Input.acceleration.x;
#if UNITY_EDITOR
			axis = Input.GetAxis("Horizontal");
#endif
			float absAxis = Mathf.Abs(axis);

			if (absAxis > 0.1f && !isDown)
			{
				isDown = true;
				delayDown = 0;
				UpLadle();
			}
			else if(absAxis < 0.1f && isDown)
			{
				isDown = false;
				delayDown = 0.5f;
			}
			return axis;
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
			
		}

		public override void ForceUpLadle(bool isMoveBack)
		{
			
		}
	}
}