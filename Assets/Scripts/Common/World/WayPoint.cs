using UnityEngine;

namespace Common.World
{
	public class WayPoint : MonoBehaviour
	{
		public Transform mainPoint;
		public Transform fastPoint;
		public bool isUseFastPoint;

		private void Start()
		{
			if(!isUseFastPoint) Destroy(fastPoint.gameObject);
		}
	}
}