using Common.Control.Impl;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Control
{
	public class StickTractorControl : BaseControl,IDragHandler,IEndDragHandler
	{
		public RectTransform stick;
		public RectTransform stickArea;
		private Vector2 offsetFromBorder;
		private Vector2 defaultPositionStick;
		private float workWidth;
		private float workHeight;
		private Vector2 upDefault;
		private Vector2 downDefault;
		private bool isDownLadle;
		private float deltaRotate;
		private float deltaLadle;


		private void Start()
		{
			offsetFromBorder = stick.anchoredPosition;

			var rectStickArea = stickArea.rect;

			upDefault = new Vector2(rectStickArea.width / 2, 0);
			downDefault = new Vector2(rectStickArea.width / 2, -(rectStickArea.height-Mathf.Abs(offsetFromBorder.y)));
			
			ResetControl();
			workWidth = rectStickArea.width - offsetFromBorder.x * 2;
			workHeight = rectStickArea.height + offsetFromBorder.y * 2;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(stickArea,eventData.position,null,out var pos))
			{
				if (pos.x < offsetFromBorder.x) pos.x = offsetFromBorder.x;
				if (pos.y > offsetFromBorder.y) pos.y = offsetFromBorder.y;

				var rect = stickArea.rect;
				var rectWidth = rect.width;
				var rectHeight = rect.height;
				if (pos.x > rectWidth-offsetFromBorder.x) pos.x = rectWidth-offsetFromBorder.x;
				if (pos.y < -rectHeight-offsetFromBorder.y) pos.y = -rectHeight-offsetFromBorder.y;

				float halfW = workWidth/2; 
				deltaRotate = (pos.x - offsetFromBorder.x-halfW)/ (workWidth-halfW);

				deltaLadle = Mathf.Abs((pos.y - offsetFromBorder.y) / workHeight);


				if (deltaLadle >= 1 && !isDownLadle)
				{
					defaultPositionStick = downDefault;
					isDownLadle = true;
				}
				else if(deltaLadle <= 0 && isDownLadle)
				{
					isDownLadle = false;
					defaultPositionStick = upDefault;
				}
				stick.anchoredPosition = pos;
			}

		}
		

		public void OnEndDrag(PointerEventData eventData)
		{
			stick.DOAnchorPos(defaultPositionStick, 0.08f).SetEase(Ease.Linear);
			deltaRotate = 0;

		}


		public override void ResetControl()
		{
			defaultPositionStick = upDefault;
			stick.anchoredPosition = defaultPositionStick;
			isDownLadle = false;
		}

		public override void ForceUpLadle(bool isMoveBack)
		{
			
		}

		public override float GetHorizontal()
		{
			return deltaRotate;
		}

		public override float GetVertical()
		{
			return deltaLadle;
		}

		public override bool IsContactGround()
		{
			return isDownLadle;
		}
	}
}