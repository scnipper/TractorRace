using System.Collections.Generic;
using Common.Units;
using Common.World;
using UnityEngine;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		public Transform[] startPoints;
		public Transform worldRoot;
		public GameObject gameOverScreen;
		public PlayerCamera playerCamera;
		public Transform[] waypoints;
		private Tractor activeTractor;
		private List<Tractor> tractors;


		private void Start()
		{
			tractors = new List<Tractor>();
			CreateTractor();
		}

		private void CreateTractor()
		{
			gameOverScreen.SetActive(false);
			if (activeTractor != null)
			{
				Destroy(activeTractor.gameObject);
			}

			ClearTractors();
			activeTractor = Instantiate(tractor, startPoints[0].position, Quaternion.identity,worldRoot);
			activeTractor.onGameOver += GameOver;

			
			for (var i = 1; i < startPoints.Length; i++)
			{
				var botTractor = Instantiate(tractor, startPoints[i].position, Quaternion.identity,worldRoot);
				botTractor.Waypoints = waypoints;
				botTractor.IsBot = true;
				tractors.Add(botTractor);
			}
			playerCamera.ResetCamera();
			playerCamera.Tractor = activeTractor.transform;
		}

		private void GameOver()
		{
			gameOverScreen.SetActive(true);
			playerCamera.GameOverMove();
		}

		private void ClearTractors()
		{
			foreach (var tr in tractors)
			{
				Destroy(tr.gameObject);
			}
			tractors.Clear();
		}

		public void RestartGame()
		{
			CreateTractor();
		}

		public Tractor ActiveTractor => activeTractor;
	}
}
