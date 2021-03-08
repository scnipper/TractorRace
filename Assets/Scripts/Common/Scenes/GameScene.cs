using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Control;
using Common.Units;
using Common.World;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		public Transform[] startPoints;
		public Transform worldRoot;
		public GameObject gameOverScreen;
		public GameObject finishScreen;
		public PlayerCamera playerCamera;
		public WayPoint[] waypoints;
		public StickTractorControl stickTractorControl;
		public Transform finishPoint;
		public Text placeText;
		private Tractor activeTractor;
		private List<Tractor> tractors;
		private bool isUpdatePlaceText;
		private int placeMainTractor;


		private void Start()
		{
			tractors = new List<Tractor>();
			CreateTractor();
		}

		private void CreateTractor()
		{
			playerCamera.ResetCamera();

			isUpdatePlaceText = false;
			stickTractorControl.ResetControl();
			gameOverScreen.SetActive(false);
			finishScreen.SetActive(false);
			if (activeTractor != null)
			{
				Destroy(activeTractor.gameObject);
			}

			ClearTractors();
			activeTractor = Instantiate(tractor, startPoints[0].position, Quaternion.identity,worldRoot);
			activeTractor.onGameOver += GameOver;
			activeTractor.onFinish += FinishGame;

			
			for (var i = 1; i < startPoints.Length; i++)
			{
				var botTractor = Instantiate(tractor, startPoints[i].position, Quaternion.identity,worldRoot);
				botTractor.Waypoints = waypoints;
				botTractor.IsBot = true;
				botTractor.onFinish += FinishGame;
				tractors.Add(botTractor);
			}
			playerCamera.Tractor = activeTractor.transform;
			StartCoroutine(UpdatePlaceText());
		}

		/// <summary>
		/// Когда доехали до финиша
		/// </summary>
		private void FinishGame()
		{
			activeTractor.IsGameOver = true;
			activeTractor.StopRotate();
			activeTractor.StopMove();
			if (placeMainTractor == 0 && !gameOverScreen.activeSelf)
			{
				finishScreen.SetActive(true);
				playerCamera.GameOverMove();
			}
			else
			{
				GameOver();
			}
		}

		private IEnumerator UpdatePlaceText()
		{
			var distances = new List<float>();
			var tractorsTransform = tractors.Select(tr => tr.transform).ToList();
			var activeTransform = activeTractor.transform;
			isUpdatePlaceText = true;
			while (isUpdatePlaceText)
			{
				distances.Clear();
				foreach (var tr in tractorsTransform)
				{
					if(tr == null) yield break;
					distances.Add((finishPoint.position - tr.position).sqrMagnitude);
				}

				var sortDistances = distances.OrderByDescending(dist=>dist).ToList();
				
				

				var mainDistance = (finishPoint.position - activeTransform.position).sqrMagnitude;

				placeMainTractor = sortDistances.Count;
				for (var i = 0; i < sortDistances.Count; i++)
				{
					if (mainDistance < sortDistances[i])
					{
						placeMainTractor = i;
					}
				}


				placeText.text = $"Place: {placeMainTractor + 1}/{distances.Count + 1}";
				

				yield return new WaitForSeconds(0.3f);
			}
		}

		private void GameOver()
		{
			if (!finishScreen.activeSelf)
			{
				gameOverScreen.SetActive(true);
				playerCamera.GameOverMove();
			}
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
