using System.Collections.Generic;
using UnityEngine;

namespace Util
{
	public static class Yielders {
 
		static Dictionary<float, WaitForSeconds> _timeInterval = new Dictionary<float, WaitForSeconds>(100);
		static Dictionary<float, WaitForSecondsRealtime> _timeIntervalReal = new Dictionary<float, WaitForSecondsRealtime>(100);
 
		static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
		public static WaitForEndOfFrame EndOfFrame => _endOfFrame;

		static WaitForFixedUpdate _fixedUpdate = new WaitForFixedUpdate();
		public static WaitForFixedUpdate FixedUpdate => _fixedUpdate;

		public static WaitForSeconds WaitSecond(float seconds){
			if(!_timeInterval.ContainsKey(seconds))
				_timeInterval.Add(seconds, new WaitForSeconds(seconds));
			return _timeInterval[seconds];
		}
		
		public static WaitForSecondsRealtime WaitSecondRealtime(float seconds){
			if(!_timeIntervalReal.ContainsKey(seconds))
				_timeIntervalReal.Add(seconds, new WaitForSecondsRealtime(seconds));
			return _timeIntervalReal[seconds];
		}
   
	}
}