using System;
using Common.Control.Impl;
using DG.Tweening;
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
		public float decelerationRotate = 20;
		public float maxSteeringAngle;
		public float delaySteering = 0.1f;
		public GameObject pathDrawer;
		public GameObject ladlePathDrawer;
		public GameObject body;

		public event Action onGameOver;
		public event Action onFinish;

		private Tweener tweenRotate;
		private Vector3 ladleLocalPosition;
		private Vector3 cylinderPos;
		private Vector3 saveCylinderScale;
		private GameObject cylinderGroundGameObject;
		private float steering;

		private static string waterTag = "Water";
		private static string groundTag = "Ground";
		private static string worldGroundTag = "WorldGround";
		private static string finishPoint = "FinishPoint";
		private bool isOnWater;
		private bool isStoppingRotate = true;

		[Serializable]
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

		


		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(finishPoint))
			{
				onFinish?.Invoke();
				IsGameOver = true;
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			if(!IsGameOver && !IsBot && other.collider.CompareTag(worldGroundTag))
			{
				CallGameOver();
			}
		}
		

		public void FixedUpdate()
		{
			if(IsGameOver) return;
			
			steering = maxSteeringAngle * Control.GetHorizontal();

			ladle.localPosition = Vector3.Lerp(ladleLocalPosition, new Vector3(0, 0.1f, ladleLocalPosition.z),
												Control.GetVertical());

			if (Control.IsContactGround())
			{
				cylinderGroundGameObject.SetActive(true);
				ladlePathDrawer.SetActive(true);
			}
			else
			{
				ladlePathDrawer.SetActive(false);
			}
			foreach (AxleInfo axleInfo in axleInfos) {
				if (axleInfo.steering) {
					axleInfo.leftWheel.steerAngle = steering;
					axleInfo.rightWheel.steerAngle = steering;
				}
				if (axleInfo.motor) {
					float absHorizontal = decelerationRotate * Mathf.Abs(Control.GetHorizontal());
					float motorTorque = maxMotorTorque - absHorizontal;
					if (IsBot)
					{
						motorTorque += motorTorque * 0.1f;
					}
					axleInfo.leftWheel.motorTorque = motorTorque;
					axleInfo.rightWheel.motorTorque = motorTorque;
				}
				
				if (axleInfo.leftWheel.GetGroundHit(out var leftWheelHit))
				{
					CheckWheelHit(leftWheelHit);
				}
				else if (axleInfo.rightWheel.GetGroundHit(out var rightWheelHit))
				{
					CheckWheelHit(rightWheelHit);
				}
				
				ApplyLocalPositionToVisuals(axleInfo.visualLeft,axleInfo.leftWheel);
				ApplyLocalPositionToVisuals(axleInfo.visualRight,axleInfo.rightWheel);
			}
		}

		/// <summary>
		/// Определеяем коллизию колеса
		/// </summary>
		/// <param name="wheelHit"></param>
		private void CheckWheelHit(WheelHit wheelHit)
		{
			if (wheelHit.collider.CompareTag(waterTag))
			{
				isOnWater = true;
				ladlePathDrawer.SetActive(false);
				pathDrawer.SetActive(true);
				if (!cylinderGroundGameObject.activeSelf)
				{
					CheckGameOver();
				}
			}
			else if(wheelHit.collider.CompareTag(groundTag))
			{
				isOnWater = false;
				pathDrawer.SetActive(false);
				ladlePathDrawer.SetActive(Control.IsContactGround());
			}
			else if(wheelHit.collider.CompareTag(worldGroundTag))
			{
				pathDrawer.SetActive(false);

				if(!IsBot)
					CallGameOver();
			}
		}

		/// <summary>
		/// Вызываем гаме овер
		/// </summary>
		private void CallGameOver()
		{
			if(IsGameOver) return;
			IsGameOver = true;
			StopMove();
			onGameOver?.Invoke();
		}

		public void StopMove()
		{
			foreach (var ax in axleInfos)
			{
				ax.motor = false;
				ax.leftWheel.motorTorque = 0;
				ax.rightWheel.motorTorque = 0;
			}
		}

		// finds the corresponding visual wheel
		// correctly applies the transform
		public void ApplyLocalPositionToVisuals(Transform visualWheel,WheelCollider wheelCollider)
		{
			wheelCollider.GetWorldPose(out var position, out var rotation);
			
			rotation *= Quaternion.Euler(0, 0, 90);
			visualWheel.position = position;
			visualWheel.rotation = rotation;
		}

		
	

		public void StopRotate()
		{
			tweenRotate?.Kill();
			isStoppingRotate = false;
			tweenRotate = DOTween.To(val => steering = val, steering, 0, delaySteering)
				.OnComplete(()=>isStoppingRotate = true)
				.SetEase(Ease.Linear);
		}
		

		private void Update()
		{
			if(IsGameOver) return;

			
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
				if (Control.IsContactGround() && !isOnWater)
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
						pathDrawer.SetActive(false);
						CheckGameOver();
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

		private void CheckGameOver()
		{
			if (isOnWater)
			{
				body.layer = LayerMask.NameToLayer("IgnoreWater");
				foreach (var ax in axleInfos)
				{
					ax.leftWheel.gameObject.layer = LayerMask.NameToLayer("IgnoreWater");
					ax.rightWheel.gameObject.layer = LayerMask.NameToLayer("IgnoreWater");
				}
			}
		}

		public GameObject CylinderGroundGameObject => cylinderGroundGameObject;
		public bool IsGameOver { get; set; }
		public bool IsBot { get; set; }
		public BaseControl Control { get; set; }
	}
}