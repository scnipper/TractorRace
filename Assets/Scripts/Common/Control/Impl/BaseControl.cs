using UnityEngine;

namespace Common.Control.Impl
{
	public abstract class BaseControl : MonoBehaviour
	{
		public abstract float GetHorizontal();
		public abstract float GetVertical();

		public abstract bool IsContactGround();
		public abstract void ResetControl();
	}
}