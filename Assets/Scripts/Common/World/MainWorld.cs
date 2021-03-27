using System.Collections;
using UnityEngine;

namespace Common.World
{
	public class MainWorld : MonoBehaviour
	{
		public Transform wayPointsContainer;
		public Transform[] startPoints;
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
		

		public WayPoint[] WayPoints { get; set; }
	}
}