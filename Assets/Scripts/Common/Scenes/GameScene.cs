using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Control;
using Common.Control.Impl;
using Common.Units;
using Common.World;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Util.Extensions;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		public Tractor tractor;
		public GameObject gameOverScreen;
		public GameObject finishScreen;
		public PlayerCamera playerCamera;
		public BaseControl[] controlPlayer;
		public Transform controlContainer;
		public BaseControl controlCpu;
		public AssetReference[] assetWorlds;
		public Dropdown worldDropDown;
		public Transform worldPlace;

		public Text placeText;

		private MainWorld mainWorld;
		private int indexLoadWorld;
		private Tractor activeTractor;
		private List<Tractor> tractors;
		private bool isUpdatePlaceText;
		private int placeMainTractor;
		private int lastUseControl;


		private void Start()
		{
			tractors = new List<Tractor>();

			for (var i = 0; i < assetWorlds.Length; i++)
			{
				worldDropDown.options.Add(new Dropdown.OptionData($"world {i+1}"));
			}
			worldDropDown.onValueChanged.AddListener(idWorld=>indexLoadWorld = idWorld);

			worldDropDown.SetValueWithoutNotify(0);
			worldDropDown.RefreshShownValue();
		}

		public void CreateTractor(int controlNum)
		{
			StartCoroutine(StartGame(controlNum));
		}

		private IEnumerator StartGame(int controlNum)
		{
			if(mainWorld == null)
				yield return LoadWorld();
			
			var worldRoot = mainWorld.transform;

			lastUseControl = controlNum;
			playerCamera.ResetCamera();

			isUpdatePlaceText = false;
			gameOverScreen.SetActive(false);
			finishScreen.SetActive(false);
			if (activeTractor != null)
			{
				Destroy(activeTractor.gameObject);
			}
			
			controlContainer.ClearAllChildren();

			ClearTractors();
			activeTractor = Instantiate(tractor, mainWorld.startPoints[0].position, Quaternion.identity,worldRoot);
			activeTractor.onGameOver += GameOver;
			activeTractor.onFinish += FinishGame;
			activeTractor.Control = CreateControl(controlPlayer[controlNum]);

			
			for (var i = 1; i < mainWorld.startPoints.Length; i++)
			{
				var botTractor = Instantiate(tractor, mainWorld.startPoints[i].position, Quaternion.identity,worldRoot);
				botTractor.IsBot = true;
				botTractor.onFinish += FinishGame;
				var botTractorControl = CreateControl(controlCpu) as BotTractorControl;
				if (botTractorControl != null)
				{
					botTractorControl.Waypoints = mainWorld.WayPoints;
					botTractorControl.MainTractor = botTractor;
				}
				botTractor.Control = botTractorControl;
				tractors.Add(botTractor);
			}
			playerCamera.Tractor = activeTractor.transform;
			if(tractors.Count  > 1)
				StartCoroutine(UpdatePlaceText());
		}

		/// <summary>
		/// Загрузка мира
		/// </summary>
		private IEnumerator LoadWorld()
		{
			yield return assetWorlds[indexLoadWorld].LoadAssetAsync<GameObject>();

			var prepareWorld = assetWorlds[indexLoadWorld].Asset;

			var instWorld = Instantiate(prepareWorld, worldPlace) as GameObject;
			if (instWorld != null) mainWorld = instWorld.GetComponent<MainWorld>();
		}

		private BaseControl CreateControl(BaseControl control)
		{
			var baseControl = Instantiate(control, controlContainer);
			baseControl.ResetControl();
			return baseControl;
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
			var finishPointPosition = mainWorld.finishPoint.position;
			finishPointPosition.y = 0;
			while (isUpdatePlaceText)
			{
				distances.Clear();
				foreach (var tr in tractorsTransform)
				{
					if (tr != null)
					{
						var trPosition = tr.position;
						trPosition.y = 0;
						distances.Add((finishPointPosition - trPosition).sqrMagnitude);
					}
				}

				var sortDistances = distances.OrderBy(dist=>dist).ToList();


				var activeTransformPosition = activeTransform.position;
				activeTransformPosition.y = 0;
				var mainDistance = (finishPointPosition - activeTransformPosition).sqrMagnitude;

				placeMainTractor = sortDistances.Count;
				for (var i = 0; i < sortDistances.Count; i++)
				{
					if (mainDistance < sortDistances[i])
					{
						placeMainTractor = i;
						break;
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
			CreateTractor(lastUseControl);
		}
	}
}
