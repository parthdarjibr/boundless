﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using MA.EaseNTween;
using UnityEngine.SceneManagement;

namespace MA{
	public class FadingLevelChanger : MonoBehaviour, ILevelChangeRequest{
		public EasingTypes easing;
		public float animationLength = 0.7f;

		#region ILevelChangeRequest implementation

		public void changeLevelTo (int levelID)
		{
			SimpleFader.instance.fade(1f, animationLength, easing, ()=>{
				TweenManager.Dispose();
				SceneManager.LoadScene(levelID);
			});
		}

		public void changeLevelTo (string levelName)
		{
			SimpleFader.instance.fade(1f, animationLength, easing, ()=>{
				TweenManager.Dispose();
				SceneManager.LoadScene(levelName);
			});
		}


		#endregion
	}
}
