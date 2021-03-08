using Common.Scenes;
using UnityEngine;
using UnityEngine.EventSystems;
using Util;

namespace Common.Control
{
	public class TractorControl : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
	{
		public GameScene gameScene;
		public DownUpButton ladleButton;
		private Vector2 sizeInput;

		private void Start()
		{
			sizeInput = GetComponent<RectTransform>().rect.size;
			ladleButton.onDown += DownLadle;
			ladleButton.onUp += UpLadle;
		}

		private void UpLadle()
		{
			gameScene.ActiveTractor.UpLadle();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			var halfWidth = sizeInput.x / 2;
			if (eventData.position.x > halfWidth)
			{
				gameScene.ActiveTractor.RotateRight();
			}
			else
			{
				gameScene.ActiveTractor.RotateLeft();
			}
		}

		public void DownLadle()
		{
			gameScene.ActiveTractor.DownLadle();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			gameScene.ActiveTractor.StopRotate();
		}
	}
}