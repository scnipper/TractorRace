using System;
using System.Collections;
using Common.World;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Random = UnityEngine.Random;

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
		private Transform trTractor;
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

			trTractor = transform;
			if(IsBot)
				StartCoroutine(BotCycle());
			
		}

		private IEnumerator BotCycle()
		{
			int wayPointIndex = 0;
			bool isBotMove = true;
			float rotateSide = -1;
			bool waitWhenNorm = false;

			float timeWaitWhenLadleUp = 0;
			

			
			while (isBotMove)
			{
				var wayPoint = Waypoints[wayPointIndex];

				var pointToMove = wayPoint.mainPoint;

				// двигаемся в обрез пути
				if (wayPoint.isUseFastPoint && cylinderGroundGameObject.activeSelf && cylinderGround.localScale.x >= maxSizeCylinder-1)
				{
					pointToMove = wayPoint.fastPoint;
				}

				if (timeWaitWhenLadleUp > 0)
				{
					timeWaitWhenLadleUp -= Time.deltaTime;
					if (timeWaitWhenLadleUp <= 0)
					{
						if (Random.value < 0.3f)
						{
							UpLadle();
							timeWaitWhenLadleUp = 0;
						}
						else
						{
							timeWaitWhenLadleUp = Random.Range(1f, 5f);
						}
					}
				}
				else
				{
					if (Random.value < 0.004f)
					{
						DownLadle();
						timeWaitWhenLadleUp = Random.Range(1f, 5f);
					}
				}

				

				if (Vector3.Distance(pointToMove.position, trTractor.position) > 1.5f)
				{
					
					Vector3 dirFromAtoB = (pointToMove.position - trTractor.position).normalized;
					float dotProd = Vector3.Dot(dirFromAtoB, trTractor.forward);
					
					float maxSteeringWithRotateSide = maxSteeringAngle * rotateSide;
					
					float newSteeringValue = maxSteeringWithRotateSide * ((1-dotProd) / 0.09f);
					if (newSteeringValue < maxSteeringWithRotateSide || newSteeringValue > maxSteeringWithRotateSide)
					{
						newSteeringValue = maxSteeringWithRotateSide;
					}
					steering = newSteeringValue;



					if (dotProd > 0.96f)
					{
						steering = 0;
					}


					if (dotProd < 0.91 && !waitWhenNorm)
					{
						var crossAngle = Vector3.Angle((pointToMove.position - trTractor.position).normalized,
														trTractor.right);

						rotateSide = crossAngle < 90 ? 1 : -1; 
						waitWhenNorm = true;
					}

					if (waitWhenNorm && dotProd > 0.91)
					{
						waitWhenNorm = false;
						
					}
				}
				else
				{

					wayPointIndex++;

					if (wayPointIndex >= Waypoints.Length)
					{
						isBotMove = false;
					}

					steering = 0;

				}
				
				
				yield return null;
			}
		}


		private void OnCollisionEnter(Collision other)
		{
			if(!isGameOver && !IsBot && other.collider.CompareTag(worldGroundTag))
			{
				CallGameOver();
			}
		}
		

		public void FixedUpdate()
		{
			if(isGameOver) return;
#if UNITY_EDITOR
			//if(!IsBot)
			//	steering = maxSteeringAngle * Input.GetAxis("Horizontal");
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
					else if(wheelHit.collider.CompareTag(worldGroundTag) && !IsBot)
					{
						CallGameOver();
					}
				}
				ApplyLocalPositionToVisuals(axleInfo.visualLeft,axleInfo.leftWheel);
				ApplyLocalPositionToVisuals(axleInfo.visualRight,axleInfo.rightWheel);
			}
		}

		/// <summary>
		/// Вызываем гаме овер
		/// </summary>
		private void CallGameOver()
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

		// finds the corresponding visual wheel
		// correctly applies the transform
		public void ApplyLocalPositionToVisuals(Transform visualWheel,WheelCollider wheelCollider)
		{
			wheelCollider.GetWorldPose(out var position, out var rotation);
			
			rotation *= Quaternion.Euler(0, 0, 90);
			visualWheel.position = position;
			visualWheel.rotation = rotation;
		}


		public void RotateByFactor(float factor)
		{
			steering = maxSteeringAngle * factor;
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
			isStoppingRotate = false;
			tweenRotate = DOTween.To(val => steering = val, steering, 0, delaySteering)
				.OnComplete(()=>isStoppingRotate = true)
				.SetEase(Ease.Linear);
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

			if (!IsBot)
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					DownLadle();
				}

				if (Input.GetKeyUp(KeyCode.Space))
				{
					UpLadle();
				}
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

		public bool IsBot { get; set; }
		public WayPoint[] Waypoints { get; set; }
	}
}