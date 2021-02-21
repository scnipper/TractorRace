using DG.Tweening;
using UnityEngine;

namespace Common.Units
{
	public class Tractor : MonoBehaviour
	{
		private Rigidbody rb;

		private float addRotation;
		private Tweener tweenRotate;

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			var speed = rb.velocity.sqrMagnitude;
			if(speed < 5000)
				rb.AddRelativeForce(Force * 200);
			
			if (addRotation != 0)
			{
				rb.AddRelativeTorque(new Vector3(0,addRotation,0));
			}
		}

		public void RotateLeft()
		{ 
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => addRotation = val, 0, -40, 0.1f).SetEase(Ease.Linear);
		}
		public void RotateRight()
		{ 
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => addRotation = val, 0, 40, 0.1f).SetEase(Ease.Linear);
		}

		public void StopRotate()
		{
			tweenRotate?.Kill();
			addRotation = 0;
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.D))
			{
				RotateRight();
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				RotateLeft();
			}
			if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
			{
				StopRotate();
			}

			
		}

		public Vector3 Force { get; set; } = Vector3.forward;
	}
}