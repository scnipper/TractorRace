using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Common.UI
{
	
	[RequireComponent(typeof(Text))]
	public class FpsViewer : MonoBehaviour
	{
		private Text textView;

		private void Start()
		{
			textView = GetComponent<Text>();
			StartCoroutine(UpdateFps());
		}

		private IEnumerator UpdateFps()
		{
			while (true)
			{
				textView.text = $"FPS: {Mathf.Floor(1.0f / Time.deltaTime)}";
				yield return new WaitForSeconds(0.3f);
			}
		}
	}
}