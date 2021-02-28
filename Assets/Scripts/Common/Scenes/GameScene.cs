using Common.Units;
using UnityEngine;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		public Transform startPoint;
		public Transform worldRoot;
		public GameObject gameOverScreen;
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
		}

		private void GameOver()
		{
			gameOverScreen.SetActive(true);
		}

		public void RestartGame()
		{
			CreateTractor();
		}
	}
}
