using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Common.Units
{
	public class Tractor : MonoBehaviour
	{
		public Transform ladle;
		public Transform cylinderGround;
		public float maxSizeCylinder = 4;
		
		public AxleInfo[] axleInfos; // the information about each individual axle
		public float maxMotorTorque;     // maximum torque the motor can apply to wheel
		public float maxSteeringAngle;

		private Tweener tweenRotate;
		private TweenerCore<Vector3, Vector3, VectorOptions> ladleMoveTween;
		private Vector3 ladleLocalPosition;
		private bool isGroundContact;
		private Vector3 cylinderPos;
		private Vector3 saveCylinderScale;
		private GameObject cylinderGroundGameObject;

		
		[System.Serializable]
		public class AxleInfo {
			public WheelCollider leftWheel;
			public WheelCollider rightWheel;
			public Transform visualLeft;
			public Transform visualRight;
			public bool motor;    // is this wheel attached to motor?
			public bool steering; // does this wheel apply steer angle?
		}
		private void Start()
		{
			cylinderGroundGameObject = cylinderGround.gameObject;
			saveCylinderScale = cylinderGround.localScale;
			cylinderGroundGameObject.SetActive(false);
			ladleLocalPosition = ladle.localPosition;
			cylinderPos = cylinderGround.localPosition;
			cylinderGround.DOLocalRotate(new Vector3(40, 0, 90), 0.5f)
				.SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Incremental);
		}

		public void FixedUpdate()
		{
			float motor = maxMotorTorque;
			float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
            
			foreach (AxleInfo axleInfo in axleInfos) {
				if (axleInfo.steering) {
					axleInfo.leftWheel.steerAngle = steering;
					axleInfo.rightWheel.steerAngle = steering;
				}
				if (axleInfo.motor) {
					axleInfo.leftWheel.motorTorque = motor;
					axleInfo.rightWheel.motorTorque = motor;
				}
				ApplyLocalPositionToVisuals(axleInfo.visualLeft,axleInfo.leftWheel);
				ApplyLocalPositionToVisuals(axleInfo.visualRight,axleInfo.rightWheel);
			}
		}
		
		// finds the corresponding visual wheel
		// correctly applies the transform
		public void ApplyLocalPositionToVisuals(Transform visualWheel,WheelCollider wheelCollider)
		{

			Vector3 position;
			Quaternion rotation;
			wheelCollider.GetWorldPose(out position, out rotation);

			rotation *= Quaternion.Euler(0, 0, 90);
			visualWheel.position = position;
			visualWheel.rotation = rotation;
		}
	

		public void RotateLeft()
		{ 
			tweenRotate?.Kill();
			//tweenRotate = DOTween.To(val => addRotation = val, 0, -10, 0.1f).SetEase(Ease.Linear);
		}
		public void RotateRight()
		{ 
			tweenRotate?.Kill();
			//tweenRotate = DOTween.To(val => addRotation = val, 0, 10, 0.1f).SetEase(Ease.Linear);
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
			

			if (Input.GetKeyDown(KeyCode.Space))
			{
				ladleMoveTween?.Kill();
				ladleMoveTween = ladle.DOLocalMove(new Vector3(0,0.1f,ladleLocalPosition.z),0.7f )
					.SetEase(Ease.Linear)
					.OnComplete(()=>
					{
						cylinderGround.gameObject.SetActive(true);
						isGroundContact = true;
					});
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				isGroundContact = false;

				ladleMoveTween?.Kill();

				ladleMoveTween = ladle.DOLocalMove(ladleLocalPosition,0.7f ).SetEase(Ease.Linear);
			}

			
			UpdateSizeCylinderGround();
			

		}

	

		/// <summary>
		/// Обновляем размер комка земли перед ковшом
		/// </summary>
		private void UpdateSizeCylinderGround()
		{
			if (cylinderGroundGameObject.activeSelf)
			{
				var scale = cylinderGround.localScale;

				var posCylinder = cylinderGround.localPosition;


				float deltaCylinder = Time.deltaTime;
				if (isGroundContact)
				{
					scale.x += deltaCylinder;
					scale.z += deltaCylinder;
					if (scale.x >= maxSizeCylinder)
					{
						scale.x = maxSizeCylinder;
						scale.z = maxSizeCylinder;
					}
				}
				else
				{
					scale.x -= deltaCylinder;
					scale.z -= deltaCylinder;
					if (scale.x <= saveCylinderScale.x)
					{
						scale = saveCylinderScale;
						cylinderGroundGameObject.SetActive(false);
					}
				}


				float scaleX = scale.x - 1;
				if (scaleX < 0) scaleX = 0;
				posCylinder.y = cylinderPos.y + (scaleX / 2);
				posCylinder.z = cylinderPos.z + (scaleX / 2);
				cylinderGround.localPosition = posCylinder;

				cylinderGround.localScale = scale;
			}

		}

	}
}