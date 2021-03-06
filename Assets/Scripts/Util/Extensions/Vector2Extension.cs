using UnityEngine;

namespace Util.Extensions
{
	public static class Vector2Extension {
		public static Vector2 Rotate(this Vector2 v, float degrees) {
			float radians = degrees * Mathf.Deg2Rad;
			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);
         
			float tx = v.x;
			float ty = v.y;
			v.Set(cos * tx - sin * ty, sin * tx + cos * ty);
			return v;
		}
		
		public static float AngleBetweenVector2(this Vector2 vec1, Vector2 vec2)
		{
			Vector2 difference = vec2 - vec1;
			float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
			return Vector2.Angle(Vector2.right, difference) * sign;
		}
	}
}