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
		private bool isSpaceDown;

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				isSpaceDown = true;
				MoveLadle(true);
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				isSpaceDown = false;
				MoveLadle(false);
			}
		}

		private void MoveLadle(bool isDown)
		{
			print("move ladle is down "+isDown);
			tweenLadle?.Kill();
			tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, isDown ? 1:0, 0.35f)
				.SetEase(Ease.Linear)
				.OnComplete(() => isGroundContact = isDown);
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

		public override void ForceUpLadle(bool isMoveBack)
		{
			if (isSpaceDown)
			{
				isGroundContact = !isMoveBack;
			}
		}
	}
}