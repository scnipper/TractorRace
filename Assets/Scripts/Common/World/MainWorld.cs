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


		private void Awake()
		{
			WayPoints = wayPointsContainer.GetComponentsInChildren<WayPoint>();
		}
		

		private IEnumerator Start()
		{

			yield return null;
			roadMask.SetActive(false);
		}

		public WayPoint[] WayPoints { get; set; }
	}
}