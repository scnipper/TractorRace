using UnityEngine;

namespace Common.UI
{
	public class RotatingLoading : MonoBehaviour
	{
		private Transform tr;

		private void Start()
		{
			tr = transform;
		}

		private void Update()
		{
			tr.rotation *= Quaternion.Euler(0,0,-4);
		}
	}
}