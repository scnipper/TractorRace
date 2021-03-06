using System;
using System.Collections;
using UnityEngine;

namespace Util.Extensions
{
	public static class MonoBehaviourExtension
	{

		/// <summary>
		/// Запуск на следующем кадре
		/// </summary>
		/// <param name="mono"></param>
		/// <param name="runAction"></param>
		public static void RunNextFrame(this MonoBehaviour mono, Action runAction)
		{
			mono.StartCoroutine(RunNextFrameCoroutine(runAction));
		}

		/// <summary>
		/// Запуск через какоето время
		/// </summary>
		/// <param name="mono"></param>
		/// <param name="time"></param>
		/// <param name="runAction"></param>
		public static void RunAfter(this MonoBehaviour mono,float time, Action runAction)
		{
			mono.StartCoroutine(RunAfterCoroutine(time, runAction));
		}
		
		/// <summary>
		/// Запуск через какоето время
		/// </summary>
		/// <param name="mono"></param>
		/// <param name="time"></param>
		/// <param name="runAction"></param>
		public static void RunAfterUnscaled(this MonoBehaviour mono,float time, Action runAction)
		{
			mono.StartCoroutine(RunAfterUnscaledCoroutine(time, runAction));
		}

		private static IEnumerator RunNextFrameCoroutine(Action runAction)
		{
			yield return null;
			runAction?.Invoke();
		}

		private static IEnumerator RunAfterUnscaledCoroutine(float time, Action runAction)
		{
			yield return new WaitForSecondsRealtime(time);
			runAction?.Invoke();
		}
		private static IEnumerator RunAfterCoroutine(float time, Action runAction)
		{
			yield return new WaitForSeconds(time);
			runAction?.Invoke();
		}
	}
}