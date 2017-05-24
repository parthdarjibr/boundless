using UnityEngine;
using System;
using System.Collections;
using MA.EaseNTween;

namespace MA{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasGroupTransition : MonoBehaviour, IMenuTransition {
		[SerializeField]
		private EasingTypes easing;
		public float animationLength = 1f;
		[SerializeField]
		private bool unscaled;
		[SerializeField]
		private bool onlyInteractableAndRayCast;
		private CanvasGroup cgroup;

		[SerializeField] private float minAlpha = 0f, maxAlpha = 1f;

		void Awake () {
			cgroup = GetComponent<CanvasGroup>();
		}

		#region IAnimatable implementation

		public void openAnimation(Action onFinished)
		{
			cgroup.blocksRaycasts = true;
			cgroup.interactable = true;
			if(!onlyInteractableAndRayCast) animate(onFinished, maxAlpha);
			else onFinished();
		}
		public void closeAnimation(Action onFinished)
		{
			cgroup.blocksRaycasts = false;
			cgroup.interactable = false;
			if(!onlyInteractableAndRayCast) animate(onFinished, minAlpha);
			else onFinished();
		}

		/// <summary>
		/// Closes the animation. When using this function, turn raycast blocking to off on OnFinished action.
		/// </summary>
		/// <param name="onFinished">On finished.</param>
		/// <param name="lateUpdate">If set to <c>true</c> late update.</param>
		public void closeAnimation(Action onFinished, bool lateUpdate = false) {
			if (lateUpdate == false) {
#if UNITY_5_5_OR_NEWER
                closeAnimation(onFinished);
#else
                cgroup.blocksRaycasts = false;
                cgroup.interactable = false;
                if (!onlyInteractableAndRayCast) animate(onFinished, minAlpha);
                else onFinished();
#endif
            } else {
				cgroup.interactable = false;
				if (!onlyInteractableAndRayCast) {
					animate (onFinished, minAlpha);
				}
				else
					onFinished ();
			}
		}

#endregion

		private void animate(Action OnFinished, float alphatarget)
		{
			cgroup.FadeTo(alphatarget, length, easing, unscaled, Tween.TweenRepeat.Once, OnFinished);
		}

		public float length {
			get {
				return animationLength;
			}
			set {
				animationLength = value;
			}
		}
	}
}
