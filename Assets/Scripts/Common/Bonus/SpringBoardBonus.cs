using Common.Bonus.Impl;
using Common.Units;
using UnityEngine;

namespace Common.Bonus
{
	public class SpringBoardBonus : BaseBonus
	{
		public float impulse = 2;
		protected override void EndTimeBonus()
		{
			
		}

		protected override void CarIn(Tractor tractor)
		{
			print("IMpulse");
			tractor.GetComponent<Rigidbody>().AddForce(tractor.TrTractor.forward * impulse,ForceMode.Impulse);
		}

		protected override void CarOut(Tractor tractor)
		{
			
		}
	}
}