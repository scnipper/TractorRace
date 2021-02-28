using Common.Units;
using UnityEngine;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		private Transform tractorTransform;
		private Vector3 tractorSavePosition;
		private Quaternion tractorSaveRotation;


		private void Start()
		{
			tractorTransform = tractor.transform;
			tractorSavePosition = tractorTransform.position;
			tractorSaveRotation = tractorTransform.rotation;
		}

		public void RestartGame()
		{
			tractorTransform.rotation = tractorSaveRotation;
			tractorTransform.position = tractorSavePosition;
		}
	}
}
