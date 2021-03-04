using UnityEngine;
using UnityEngine.UI;

namespace TFPlay.UI
{
	public class Tutorial_Swipe : TutorialItem
	{
		[SerializeField] private Image hand;
		[SerializeField] private float moveTime;

		public override void Play()
		{
			base.Play();

			this.RepeatCoroutine(
				interval: moveTime,
				action: i => {
					this.LerpCoroutine(
						chainHead: true,
						time: moveTime / 2,
						from: 1,
						to: 0,
						action: p => {
							hand.rectTransform.pivot = new Vector2(p / 2, .5f);
							hand.color = hand.color.WithAlpha(p);
						},
						settings: new CoroutineTemplate.Settings(curve: CoroutineTemplate.EaseInOutCurve)
					).Then(this.LerpCoroutine(
						time: moveTime / 2,
						from: 0,
						to: 1,
						action: p => {
							hand.rectTransform.pivot = new Vector2(.5f + p / 2, .5f);
							hand.color = hand.color.WithAlpha(1 - p);
						},
						settings: new CoroutineTemplate.Settings(curve: CoroutineTemplate.EaseInOutCurve)
					));
				}
			);
		}
	}
}