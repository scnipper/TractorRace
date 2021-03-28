using System.Collections;
using UnityEngine;

namespace Common.World
{
	public class MainWorld : MonoBehaviour
	{
		public Transform wayPointsContainer;
		public Transform[] startPoints;
		// точки которые детектят что мы едим назад
		public Transform[] toBackPoints;
		public Transform finishPoint;
		public GameObject roadMask;
		public Camera[] worldCameras;


		private void Awake()
		{
			WayPoints = wayPointsContainer.GetComponentsInChildren<WayPoint>();
		}

		/// <summary>
		/// Очистка всех путей движения
		/// </summary>
		public void ResetWorldCameras()
		{
			StartCoroutine(ClearCameras());
		}

		private IEnumerator ClearCameras()
		{
			foreach (var worldCamera in worldCameras)
			{
				worldCamera.clearFlags = CameraClearFlags.Color;
				roadMask.SetActive(true);
				yield return null;
				worldCamera.clearFlags = CameraClearFlags.Nothing;
				roadMask.SetActive(false);
			}
		}


		public bool IsLookNearestToBackPoint(Transform trTractor)
		{
			float minDistance = float.MaxValue;
			Transform minPoint = null;
			var trTractorPosition = trTractor.position;

			foreach (var backPoint in toBackPoints)
			{
				float distanceToPoint = (backPoint.position - trTractorPosition).sqrMagnitude;
				if (distanceToPoint < minDistance)
				{
					
					Vector3 dirFromAtoB = (backPoint.position - trTractor.position).normalized;
					float dotProd = Vector3.Dot(dirFromAtoB, backPoint.forward);

					if (dotProd < 0)
					{
						minPoint = backPoint;
						minDistance = distanceToPoint;
					}
				}
			}

			if (minPoint != null)
			{
				Vector3 dirFromAtoB = (minPoint.position - trTractor.position).normalized;
				float dotProd = Vector3.Dot(dirFromAtoB, trTractor.forward);
				return dotProd > 0;
			}
			

			return false;
		}
		
		public WayPoint[] WayPoints { get; private set; }
	}
}