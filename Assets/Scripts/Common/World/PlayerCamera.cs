using UnityEngine;

namespace Common.World
{
	public class PlayerCamera : MonoBehaviour
	{
		private Transform tr;
		private float rotX;

		private void Start()
		{
			tr = transform;
			rotX = tr.eulerAngles.x;
		}

		private void Update()
		{
			var cameraPos = Tractor.position - Tractor.forward * 1.8f;
			cameraPos.y = 1.7f;

			var rotationCamera = new Vector3(rotX,Tractor.eulerAngles.y,0);

			tr.eulerAngles = rotationCamera;
			tr.position = cameraPos;
		}

		public Transform Tractor { get; set; }
	}
}