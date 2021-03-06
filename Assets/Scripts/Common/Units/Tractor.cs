using System;
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
		public float delaySteering = 0.1f;
		public GameObject pathDrawer;
		public GameObject ladlePathDrawer;
		public GameObject body;

		public event Action onGameOver;

		private Tweener tweenRotate;
		private TweenerCore<Vector3, Vector3, VectorOptions> ladleMoveTween;
		private Vector3 ladleLocalPosition;
		private bool isGroundContact;
		private Vector3 cylinderPos;
		private Vector3 saveCylinderScale;
		private GameObject cylinderGroundGameObject;
		private float steering;

		private static string waterTag = "Water";
		private static string groundTag = "Ground";
		private static string worldGroundTag = "WorldGround";
		private bool isOnWater;
		private bool isGameOver;

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


		/*private void OnCollisionEnter(Collision other)
		{
			print("collision stay "+other.collider.name);
			if(other.collider.CompareTag(groundTag))
			{
				isCreatingPath = false;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(waterTag))
			{
				isCreatingPath = true;
			}
		}*/
		

		public void FixedUpdate()
		{
			if(isGameOver) return;
#if UNITY_EDITOR
			steering = maxSteeringAngle * Input.GetAxis("Horizontal");
#endif
            
			foreach (AxleInfo axleInfo in axleInfos) {
				if (axleInfo.steering) {
					axleInfo.leftWheel.steerAngle = steering;
					axleInfo.rightWheel.steerAngle = steering;
				}
				if (axleInfo.motor) {
					axleInfo.leftWheel.motorTorque = maxMotorTorque;
					axleInfo.rightWheel.motorTorque = maxMotorTorque;
				}
				
				if (axleInfo.leftWheel.GetGroundHit(out var wheelHit))
				{
					if (wheelHit.collider.CompareTag(waterTag))
					{
						isOnWater = true;
						ladlePathDrawer.SetActive(false);
						if (!cylinderGroundGameObject.activeSelf)
						{
							CheckGameOver();
						}
					}
					else if(wheelHit.collider.CompareTag(groundTag))
					{
						isOnWater = false;
						pathDrawer.SetActive(false);
						ladlePathDrawer.SetActive(isGroundContact);
					}
					else if(wheelHit.collider.CompareTag(worldGroundTag))
					{
						isGameOver = true;
						foreach (var ax in axleInfos)
						{
							ax.motor = false;
							ax.leftWheel.motorTorque = 0;
							ax.rightWheel.motorTorque = 0;
						}
						onGameOver?.Invoke();
					}
				}
				ApplyLocalPositionToVisuals(axleInfo.visualLeft,axleInfo.leftWheel);
				ApplyLocalPositionToVisuals(axleInfo.visualRight,axleInfo.rightWheel);
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
	

		public void RotateLeft()
		{
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => steering = val, steering, -maxSteeringAngle, delaySteering).SetEase(Ease.Linear);
		}
		public void RotateRight()
		{ 
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => steering = val, steering, maxSteeringAngle, delaySteering).SetEase(Ease.Linear);
		}

		public void StopRotate()
		{
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => steering = val, steering, 0, delaySteering).SetEase(Ease.Linear);
		}

		public void DownLadle()
		{
			ladleMoveTween?.Kill();
			ladleMoveTween = ladle.DOLocalMove(new Vector3(0,0.1f,ladleLocalPosition.z),0.35f )
				.SetEase(Ease.Linear)
				.OnComplete(()=>
				{
					cylinderGroundGameObject.SetActive(true);
					isGroundContact = true;
					ladlePathDrawer.SetActive(true);
				});
		}

		public void UpLadle()
		{
			isGroundContact = false;

			ladlePathDrawer.SetActive(false);

			ladleMoveTween?.Kill();

			ladleMoveTween = ladle.DOLocalMove(ladleLocalPosition,0.35f ).SetEase(Ease.Linear);
		}

		private void Update()
		{
			if(isGameOver) return;

			if (Input.GetKeyDown(KeyCode.Space))
			{
				DownLadle();
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				UpLadle();
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
				pathDrawer.SetActive(true);
				var scale = cylinderGround.localScale;

				var posCylinder = cylinderGround.localPosition;


				float deltaCylinder = Time.deltaTime;
				if (isGroundContact && !isOnWater)
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
	}
}