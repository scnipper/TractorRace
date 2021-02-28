using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Util
{
	public class DownUpButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
	{
		public event UnityAction onDown;
		public event UnityAction onUp;
		public void OnPointerDown(PointerEventData eventData)
		{
			onDown?.Invoke();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			onUp?.Invoke();
		}
	}
}