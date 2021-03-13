using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Common.World
{
	public class PlayerCamera : MonoBehaviour
	{
		private Transform tr;
		private float rotX;
		private bool isLookAt;
		private Vector3 savePos;
		private Quaternion saveRotate;
		private TweenerCore<Vector3, Vector3, VectorOptions> moveToUpTween;
		private Coroutine moveCameraToTopCoroutine;

		private void Start()
		{
			tr = transform;
			savePos = tr.position;
			saveRotate = tr.rotation;
			rotX = tr.eulerAngles.x;
		}

		public void ResetCamera()
		{
			if (moveCameraToTopCoroutine != null)
			{
				StopCoroutine(moveCameraToTopCoroutine);
				moveCameraToTopCoroutine = null;
			}
			
			moveToUpTween?.Kill();

			isLookAt = false;
			tr.position = savePos;
			tr.rotation = saveRotate;
		}
		private void Update()
		{
			if (Tractor != null)
			{
				if (isLookAt)
				{
					tr.LookAt(Tractor);
				
				}
				else
				{
					var cameraPos = Tractor.position - Tractor.forward * 1.8f;
					cameraPos.y = 1.7f;

					var rotationCamera = new Vector3(rotX,Tractor.eulerAngles.y,0);

					tr.eulerAngles = rotationCamera;
					tr.position = cameraPos;
				}
			}
			
			
		}

		/// <summary>
		/// Перемещаем камеру наверх при геймовере
		/// </summary>
		public void GameOverMove()
		{
			if(moveCameraToTopCoroutine == null)
				moveCameraToTopCoroutine = StartCoroutine(MoveCameraAfterStop());
		}

		private IEnumerator MoveCameraAfterStop()
		{
			var rbTractor = Tractor.GetComponent<Rigidbody>();

			yield return new WaitUntil(()=>rbTractor != null && rbTractor.velocity.sqrMagnitude < 3f);
		
			isLookAt = true;
			var tractorPosition = Tractor.position;

			tractorPosition.y = 10;
			moveToUpTween = tr.DOMove(tractorPosition, 3f);
		}

		public Transform Tractor { get; set; }
	}
}