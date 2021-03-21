using System.Collections;
using Common.Control.Impl;
using Common.Units;
using Common.World;
using DG.Tweening;
using UnityEngine;

namespace Common.Control
{
	public class BotTractorControl : BaseControl
	{
		private float ladleDelta;
		private bool isGroundContact;
		private Tweener tweenLadle;
		private float steering;
		private Transform trTractor;

		private void Start()
		{
			trTractor = MainTractor.transform;
			StartCoroutine(BotCycle());
		}

		private IEnumerator BotCycle()
		{
			int wayPointIndex = 0;
			bool isBotMove = true;
			float rotateSide = -1;
			bool waitWhenNorm = false;

			float timeWaitWhenLadleUp = 0;


			bool isMoveToFast = false;
			
			while (isBotMove)
			{
				var wayPoint = Waypoints[wayPointIndex];

				var pointToMove = wayPoint.mainPoint;

				float rangeMinusMaxSizeCylinder = Random.Range(1, 2.5f);
				// двигаемся в обрез пути
				if (wayPoint.isUseFastPoint && MainTractor.CylinderGroundGameObject.activeSelf
											&& MainTractor.cylinderGround.localScale.x >= MainTractor.maxSizeCylinder-rangeMinusMaxSizeCylinder)
				{
					pointToMove = wayPoint.fastPoint;
					isMoveToFast = true;
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
					
					//float maxSteeringWithRotateSide = maxSteeringAngle * rotateSide;
					float maxSteeringWithRotateSide = rotateSide;
					
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

					if (isMoveToFast)
					{
						wayPointIndex += wayPoint.passPointsWhenMoveToFast;
					}

					//print($"Point: {wayPointIndex}/{Waypoints.Length}");
					if (wayPointIndex >= Waypoints.Length)
					{
						isBotMove = false;
						print("End bot move");
					}

					isMoveToFast = false;
					steering = 0;

				}
				
				
				yield return null;
			}
		}

		private void UpLadle()
		{
			tweenLadle?.Kill();
			tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, 0, 0.35f)
				.SetEase(Ease.Linear)
				.OnComplete(() => isGroundContact = false);
		}

		private void DownLadle()
		{
			tweenLadle?.Kill();
			tweenLadle = DOTween.To(value => ladleDelta = value, ladleDelta, 1, 0.35f)
				.SetEase(Ease.Linear)
				.OnComplete(() => isGroundContact = true);
		}

		public override float GetHorizontal()
		{
			return steering;
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

		public WayPoint[] Waypoints { get; set; }

		public Tractor MainTractor { get; set; }
	}
}