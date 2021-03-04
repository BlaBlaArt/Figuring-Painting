using UnityEngine;

namespace TFPlay.UI
{
	public class Tutorial_HoldToMove : TutorialItem
	{
		[SerializeField] private RectTransform hand;
		[SerializeField] private float moveTime;
		[SerializeField] private float pressTime;
		[SerializeField] private float interval;

		public override void Play()
		{
			base.Play();

			this.RepeatCoroutine(
				interval: 2 * moveTime + 2 * pressTime + interval,
				action: i => {
					this.ScaleTo(
						chainHead: true,
						time: pressTime,
						transform: hand,
						target: Vector3.one * .8f
					).Then(this.LerpCoroutine(
						chainHead: false,
						time: moveTime / 2,
						from: .5f,
						to: 0,
						action: p => hand.pivot = new Vector2(p, .5f),
						settings: new CoroutineTemplate.Settings(curve: CoroutineTemplate.EaseInOutCurve)
					)).Then(this.LerpCoroutine(
						chainHead: false,
						time: moveTime,
						from: 0,
						to: 1,
						action: p => hand.pivot = new Vector2(p, .5f),
						settings: new CoroutineTemplate.Settings(curve: CoroutineTemplate.EaseInOutCurve)
					)).Then(this.LerpCoroutine(
						chainHead: false,
						time: moveTime / 2,
						from: 1,
						to: .5f,
						action: p => hand.pivot = new Vector2(p, .5f),
						settings: new CoroutineTemplate.Settings(curve: CoroutineTemplate.EaseInOutCurve)
					)).Then(this.LerpCoroutine(
						chainHead: false,
						time: pressTime,
						from: Vector3.one * .8f,
						to: Vector3.one,
						action: s => hand.transform.localScale = s
					));
				}
			);
		}
	}
}