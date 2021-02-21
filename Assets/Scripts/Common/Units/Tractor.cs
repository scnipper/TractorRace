using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Common.Units
{
	public class Tractor : MonoBehaviour
	{
		public Transform ladle;
		public Transform cylinderGround;
		public float maxSizeCylinder = 4;
		private Rigidbody rb;

		private float addRotation;
		private Tweener tweenRotate;
		private TweenerCore<Vector3, Vector3, VectorOptions> ladleMoveTween;
		private Vector3 ladleLocalPosition;
		private bool isGroundContact;
		private Vector3 cylinderPos;
		private Vector3 saveCylinderScale;
		private GameObject cylinderGroundGameObject;

		private void Start()
		{
			cylinderGroundGameObject = cylinderGround.gameObject;
			saveCylinderScale = cylinderGround.localScale;
			cylinderGroundGameObject.SetActive(false);
			rb = GetComponent<Rigidbody>();
			ladleLocalPosition = ladle.localPosition;
			cylinderPos = cylinderGround.localPosition;
			cylinderGround.DOLocalRotate(new Vector3(40, 0, 90), 0.5f)
				.SetEase(Ease.Linear)
				.SetLoops(-1, LoopType.Incremental);
		}

		private void FixedUpdate()
		{
			var speed = rb.velocity.sqrMagnitude;
			if(speed < 5000)
				rb.AddRelativeForce(Vector3.forward * 200);
			
			if (addRotation != 0)
			{
				rb.AddRelativeTorque(new Vector3(0,addRotation,0));
			}
		}

		public void RotateLeft()
		{ 
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => addRotation = val, 0, -40, 0.1f).SetEase(Ease.Linear);
		}
		public void RotateRight()
		{ 
			tweenRotate?.Kill();
			tweenRotate = DOTween.To(val => addRotation = val, 0, 40, 0.1f).SetEase(Ease.Linear);
		}

		public void StopRotate()
		{
			tweenRotate?.Kill();
			addRotation = 0;
		}
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.D))
			{
				RotateRight();
			}

			if (Input.GetKeyDown(KeyCode.A))
			{
				RotateLeft();
			}
			if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
			{
				StopRotate();
			}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				ladleMoveTween?.Kill();
				ladleMoveTween = ladle.DOLocalMove(new Vector3(0,1.46f,ladleLocalPosition.z),0.7f )
					.SetEase(Ease.Linear)
					.OnComplete(()=>
					{
						cylinderGround.gameObject.SetActive(true);
						isGroundContact = true;
					});
			}

			if (Input.GetKeyUp(KeyCode.Space))
			{
				isGroundContact = false;

				ladleMoveTween?.Kill();

				ladleMoveTween = ladle.DOLocalMove(ladleLocalPosition,0.7f ).SetEase(Ease.Linear);
			}

			
			UpdateSizeCylinderGround();
			

		}

	

		/// <summary>
		/// Обновляем размер комка земли перед ковшом
		/// </summary>
		private void UpdateSizeCylinderGround()
		{
			if (cylinderGroundGameObject.activeSelf)
			{
				var scale = cylinderGround.localScale;

				var posCylinder = cylinderGround.localPosition;


				float deltaCylinder = Time.deltaTime;
				if (isGroundContact)
				{
					scale.x += deltaCylinder;
					scale.z += deltaCylinder;
					if (scale.x >= maxSizeCylinder)
					{
						scale.x = maxSizeCylinder;
						scale.z = maxSizeCylinder;
					}
				}
				else
				{
					scale.x -= deltaCylinder;
					scale.z -= deltaCylinder;
					if (scale.x <= saveCylinderScale.x)
					{
						scale = saveCylinderScale;
						cylinderGroundGameObject.SetActive(false);
					}
				}


				float scaleX = scale.x - 1;
				if (scaleX < 0) scaleX = 0;
				posCylinder.y = cylinderPos.y + (scaleX / 2);
				posCylinder.z = cylinderPos.z + (scaleX / 2);
				cylinderGround.localPosition = posCylinder;

				cylinderGround.localScale = scale;
			}

		}

	}
}