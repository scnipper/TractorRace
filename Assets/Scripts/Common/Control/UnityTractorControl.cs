using Common.Control.Impl;
using DG.Tweening;
using UnityEngine;

namespace Common.Control
{
	public class UnityTractorControl : BaseControl
	{
		private float ladleDelta;
		private bool isGroundContact;
		private Tweener tweenLadle;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				tweenLadle?.Kill();
				tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, 1, 0.35f)
					.SetEase(Ease.Linear)
					.OnComplete(() => isGroundContact = true);
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				tweenLadle?.Kill();
				tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, 0, 0.35f)
					.SetEase(Ease.Linear)
					.OnComplete(() => isGroundContact = false);
			}
		}

		public override float GetHorizontal()
		{
			return Input.GetAxis("Horizontal");
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
			isGroundContact = false;
			ladleDelta = 0;
		}
	}
}