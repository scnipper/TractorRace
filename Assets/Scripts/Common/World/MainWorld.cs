using System;
using System.Collections;
using Common.Units;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Util;

namespace Common.World
{
	public class MainWorld : MonoBehaviour
	{
		public Transform wayPointsContainer;
		public Transform[] startPoints;
		// точки которые детектят что мы едим назад
		public Transform[] toBackPoints;
		public Transform finishPoint;
		public GameObject roadMask;
		public Camera[] worldCameras;
		public Camera lowCameraWorld;
		public Vector2 worldSize = new Vector2(100,100);
		
		private Texture2D textureWithPos;
		private bool isCanReadLowTexture = true;


		private void Awake()
		{
			WayPoints = wayPointsContainer.GetComponentsInChildren<WayPoint>();
			var targetTexture = lowCameraWorld.targetTexture;
			textureWithPos = new Texture2D(targetTexture.width,targetTexture.height,targetTexture.graphicsFormat,TextureCreationFlags.None);
			StartCoroutine(ReadTextureCamera());
		}

		private IEnumerator ReadTextureCamera()
		{
			while (true)
			{
				yield return Yielders.EndOfFrame;
				if (isCanReadLowTexture)
				{
					isCanReadLowTexture = false;
					AsyncGPUReadback.Request(lowCameraWorld.targetTexture, 0, ret =>
					{
						if (textureWithPos != null)
						{
							var nativeArray = ret.GetData<uint>();
							textureWithPos.LoadRawTextureData(nativeArray);
							textureWithPos.Apply();
							isCanReadLowTexture = true;
						}
					});
				}
				
			}
		}

		/// <summary>
		/// Определяем текущее положение на чем находится трактор
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public PlaceTractor GetCurrentPlaceTractor(Vector3 position)
		{
			float worldSizeX = position.x / worldSize.x;
			float worldSizeY = position.z / worldSize.y;

			Vector2Int pixelPosition = new Vector2Int((int) (textureWithPos.width * worldSizeX),(int) (textureWithPos.height * worldSizeY));

			var placeTractor = CheckPixel(pixelPosition);
			
			if (placeTractor != PlaceTractor.none)
			{
				return placeTractor;
			}

			/*pixelPosition.x -= 1;
			if (pixelPosition.x < 0) pixelPosition.x = 0;
			
			placeTractor = CheckPixel(pixelPosition);
			
			if (placeTractor != PlaceTractor.none)
			{
				return placeTractor;
			}
			
			pixelPosition.x += 2;
			if (pixelPosition.x > textureWithPos.width) pixelPosition.x = textureWithPos.width;
			
			placeTractor = CheckPixel(pixelPosition);
			
			if (placeTractor != PlaceTractor.none)
			{
				return placeTractor;
			}

			pixelPosition.x -= 1;
			pixelPosition.y += 1;
			
			if (pixelPosition.y > textureWithPos.height) pixelPosition.y = textureWithPos.height;
			
			placeTractor = CheckPixel(pixelPosition);
			
			if (placeTractor != PlaceTractor.none)
			{
				return placeTractor;
			}
			
			pixelPosition.y -= 2;
			
			if (pixelPosition.y < 0) pixelPosition.y = 0;
			
			placeTractor = CheckPixel(pixelPosition);
			
			if (placeTractor != PlaceTractor.none)
			{
				return placeTractor;
			}*/
			
			

			return PlaceTractor.none;
		}

		private PlaceTractor CheckPixel(Vector2Int pixelPosition)
		{
			var pixel = textureWithPos.GetPixel(pixelPosition.x,pixelPosition.y);

			if (Math.Abs(pixel.g - 1) < 0.05f) return PlaceTractor.Ground;
			if (Math.Abs(pixel.r - 1) < 0.05f) return PlaceTractor.WaterBridge;

			return PlaceTractor.none;
		}

		/// <summary>
		/// Очистка всех путей движения
		/// </summary>
		public void ResetWorldCameras()
		{
			StartCoroutine(ClearCameras());
		}

		private IEnumerator ClearCameras()
		{
			foreach (var worldCamera in worldCameras)
			{
				worldCamera.clearFlags = CameraClearFlags.Color;
				roadMask.SetActive(true);
				yield return null;
				worldCamera.clearFlags = CameraClearFlags.Nothing;
				roadMask.SetActive(false);
			}
		}


		public bool IsLookNearestToBackPoint(Transform trTractor)
		{
			float minDistance = float.MaxValue;
			Transform minPoint = null;
			var trTractorPosition = trTractor.position;

			foreach (var backPoint in toBackPoints)
			{
				float distanceToPoint = (backPoint.position - trTractorPosition).sqrMagnitude;
				if (distanceToPoint < minDistance)
				{
					
					Vector3 dirFromAtoB = (backPoint.position - trTractor.position).normalized;
					float dotProd = Vector3.Dot(dirFromAtoB, backPoint.forward);

					if (dotProd < 0)
					{
						minPoint = backPoint;
						minDistance = distanceToPoint;
					}
				}
			}

			if (minPoint != null)
			{
				Vector3 dirFromAtoB = (minPoint.position - trTractor.position).normalized;
				float dotProd = Vector3.Dot(dirFromAtoB, trTractor.forward);
				return dotProd > 0;
			}
			

			return false;
		}
		
		public WayPoint[] WayPoints { get; private set; }
	}
}