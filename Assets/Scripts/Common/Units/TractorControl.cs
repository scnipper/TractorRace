using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Units
{
	public class TractorControl : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
	{
		public Tractor tractor;
		private Vector2 sizeInput;

		private void Start()
		{
			sizeInput = GetComponent<RectTransform>().rect.size;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			var halfWidth = sizeInput.x / 2;
			if (eventData.position.x > halfWidth)
			{
				tractor.RotateRight();
			}
			else
			{
				tractor.RotateLeft();
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			tractor.StopRotate();
		}
	}
}