using Common.Units;
using Common.World;
using UnityEngine;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		public Transform startPoint;
		public Transform worldRoot;
		public GameObject gameOverScreen;
		public PlayerCamera playerCamera;
		private Tractor activeTractor;


		private void Start()
		{
			CreateTractor();
		}

		private void CreateTractor()
		{
			gameOverScreen.SetActive(false);
			if (activeTractor != null)
			{
				Destroy(activeTractor.gameObject);
			}

			activeTractor = Instantiate(tractor, startPoint.position, Quaternion.identity,worldRoot);
			activeTractor.onGameOver += GameOver;
			playerCamera.Tractor = activeTractor.transform;
		}

		private void GameOver()
		{
			gameOverScreen.SetActive(true);
		}

		public void RestartGame()
		{
			CreateTractor();
		}

		public Tractor ActiveTractor => activeTractor;
	}
}
