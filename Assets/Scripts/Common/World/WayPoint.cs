using UnityEngine;

namespace Common.World
{
	public class WayPoint : MonoBehaviour
	{
		public Transform mainPoint;
		public Transform fastPoint;
		public bool isUseFastPoint;
		[Header("Сколько пропустить следующих вейпоинтов при сокращении")]
		public int passPointsWhenMoveToFast;

		private void Start()
		{
			if(!isUseFastPoint) Destroy(fastPoint.gameObject);
		}
	}
}