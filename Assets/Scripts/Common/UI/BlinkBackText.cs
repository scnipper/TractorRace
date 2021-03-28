using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
	public class BlinkBackText : MonoBehaviour
	{

		private void Start()
		{
			var textBack = GetComponent<Text>();
			textBack.DOColor(Color.clear, 0.5f)
				.SetLoops(-1,LoopType.Yoyo)
				.SetEase(Ease.Linear);
		}

	}
}