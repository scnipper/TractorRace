using Common.Bonus.Impl;
using Common.Units;

namespace Common.Bonus
{
	public class SpeedBonus : BaseBonus
	{
		public float increaseSpeed = 3;
		private float motorTorque;
		private Tractor saveTractor;

		protected override void CarIn(Tractor tractor)
		{
			if (tractor != null)
			{
				saveTractor = tractor;
				motorTorque = tractor.maxMotorTorque;

				tractor.maxMotorTorque *= increaseSpeed;
				
			}
			
		}

		protected override void EndTimeBonus()
		{
			if (saveTractor != null)
			{
				saveTractor.maxMotorTorque = motorTorque;
			}

		}

		protected override void CarOut(Tractor tractor)
		{
			
		}
	}
}