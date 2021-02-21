using UnityEngine;

namespace Common.Units
{
	public class Tractor : MonoBehaviour
	{
		private Rigidbody rb;

		private float addRotation;
		private void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			var speed = rb.velocity.sqrMagnitude;
			if(speed < 5000)
				rb.AddRelativeForce(Force * 200);
			
			if (addRotation != 0)
			{
				rb.AddRelativeTorque(new Vector3(0,addRotation,0));
			}
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.D))
			{
				addRotation = 40;
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				addRotation = -40;
			}
			if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
			{
				addRotation = 0;
			}

			
		}

		public Vector3 Force { get; set; } = Vector3.forward;
	}
}