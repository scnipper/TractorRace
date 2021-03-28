using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Control;
using Common.Control.Impl;
using Common.Units;
using Common.World;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;
using Util.Extensions;

namespace Common.Scenes
{
	public class GameScene : MonoBehaviour
	{
		private const string Name = "GameScene";
		public Tractor tractor;
		public GameObject gameOverScreen;
		public GameObject finishScreen;
		public GameObject loadingScreen;
		public GameObject pauseScreen;
		public PlayerCamera playerCamera;
		public BaseControl[] controlPlayer;
		public Transform controlContainer;
		public BaseControl controlCpu;
		public AssetReference[] assetWorlds;
		public Dropdown worldDropDown;
		public Transform worldPlace;
		public GameObject backMoveText;

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

		public void ReloadScene()
		{
			foreach (var assetReference in assetWorlds)
			{
				assetReference.ReleaseAsset();
			}

			SceneManager.LoadScene(Name);
		}
		/// <summary>
		/// Постановка игры на паузу
		/// </summary>
		/// <param name="isPause"></param>
		public void PauseGame(bool isPause)
		{
			pauseScreen.SetActive(isPause);
			Time.timeScale = isPause ? 0 : 1;
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

			mainWorld.ResetWorldCameras();
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

			var activeTractorTransform = activeTractor.transform;

			StartCoroutine(CheckingActiveTractorMoveBack(activeTractorTransform));

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
			playerCamera.Tractor = activeTractorTransform;
			if(tractors.Count  > 1)
				StartCoroutine(UpdatePlaceText());
		}

		/// <summary>
		/// Проверка того что трактор игрока едет назад
		/// </summary>
		/// <param name="activeTractorTransform"></param>
		/// <returns></returns>
		private IEnumerator CheckingActiveTractorMoveBack(Transform activeTractorTransform)
		{
			while (activeTractorTransform != null)
			{
				bool isMoveBack = mainWorld.IsLookNearestToBackPoint(activeTractorTransform);
				
				backMoveText.SetActive(isMoveBack);
				activeTractor.Control.ForceUpLadle(isMoveBack);
				yield return Yielders.WaitSecond(0.2f);
			}

			yield return null;
		}

		/// <summary>
		/// Загрузка мира
		/// </summary>
		private IEnumerator LoadWorld()
		{
			loadingScreen.SetActive(true);
			yield return assetWorlds[indexLoadWorld].LoadAssetAsync<GameObject>();

			var prepareWorld = assetWorlds[indexLoadWorld].Asset;

			var instWorld = Instantiate(prepareWorld, worldPlace) as GameObject;
			if (instWorld != null) mainWorld = instWorld.GetComponent<MainWorld>();
			yield return null;
			loadingScreen.SetActive(false);
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

				if(activeTransform == null) yield break;

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
				

				yield return Yielders.WaitSecond(0.3f);
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
