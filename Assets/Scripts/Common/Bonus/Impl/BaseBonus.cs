using Common.Units;
using UnityEngine;
using Util.Extensions;

namespace Common.Bonus.Impl
{
	public abstract class BaseBonus : MonoBehaviour
	{
		public float timeBonus = 3;
		private bool isBonusAdded;

		private void OnTriggerEnter(Collider other)
		{

			if (!isBonusAdded)
			{
				isBonusAdded = true;
				CarIn(other.GetComponentInParent<Tractor>());
				this.RunAfter(timeBonus, () =>
				{
					isBonusAdded = false;
					EndTimeBonus();
				});
			}
			
		}

		private void OnTriggerExit(Collider other)
		{
			CarOut(other.GetComponentInParent<Tractor>());
		}

		protected abstract void EndTimeBonus();

		/// <summary>
		/// Машина задела бонус
		/// </summary>
		/// <param name="tractor"></param>
		protected abstract void CarIn(Tractor tractor);
		/// <summary>
		/// Машина вышла из бонуса
		/// </summary>
		/// <param name="tractor"></param>
		protected abstract void CarOut(Tractor tractor);
	}
}