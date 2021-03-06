using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Util.Extensions
{
	public static class TransformExtension
	{
		public delegate void TransElem(Transform child);

		public static void ForEach(this Transform transform, TransElem forElem,Action end = null)
		{
			int childCount = transform.childCount;

			for (int i = 0; i < childCount; i++)
			{
				forElem?.Invoke(transform.GetChild(i));
			}
			end?.Invoke();
		}
		
		public static void ClearAllChildren(this Transform trans)
		{
			var childCount = trans.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(trans.GetChild(i).gameObject);
			}
		}
	}
}