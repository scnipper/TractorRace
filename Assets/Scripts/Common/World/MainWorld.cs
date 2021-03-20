using System.Collections;
using UnityEngine;

namespace Common.World
{
	public class MainWorld : MonoBehaviour
	{
		public WayPoint[] waypoints;
		public Transform[] startPoints;
		public Transform finishPoint;
		public GameObject roadMask;


		private IEnumerator Start()
		{
			yield return null;
			roadMask.SetActive(false);
		}
	}
}