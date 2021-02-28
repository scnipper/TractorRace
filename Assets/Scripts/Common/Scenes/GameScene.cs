using Common.Units;
using UnityEngine;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		public Transform startPoint;
		public Transform worldRoot;
		private Tractor activeTractor;


		private void Start()
		{
			CreateTractor();
		}

		private void CreateTractor()
		{
			if (activeTractor != null)
			{
				Destroy(activeTractor.gameObject);
			}

			activeTractor = Instantiate(tractor, startPoint.position, Quaternion.identity,worldRoot);
		}

		public void RestartGame()
		{
			CreateTractor();
		}
	}
}
