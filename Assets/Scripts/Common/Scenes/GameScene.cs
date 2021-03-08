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
		public PlayerCamera playerCamera;
		public WayPoint[] waypoints;
		public StickTractorControl stickTractorControl;
		public Transform finishPoint;
		public Text placeText;
		private Tractor activeTractor;
		private List<Tractor> tractors;
		private bool isUpdatePlaceText;


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
			playerCamera.Tractor = activeTractor.transform;
			StartCoroutine(UpdatePlaceText());
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

				int place = sortDistances.Count;
				for (var i = 0; i < sortDistances.Count; i++)
				{
					if (mainDistance < sortDistances[i])
					{
						place = i;
					}
				}


				placeText.text = $"Place: {place + 1}/{distances.Count + 1}";
				

				yield return new WaitForSeconds(0.3f);
			}
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
