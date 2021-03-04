using UnityEngine;
using UnityEngine.UI;

namespace TFPlay.UI
{
	public class Tutorial_TapToPlay : TutorialItem
	{
		[SerializeField] private RectTransform hand;
		[SerializeField] private Image circle;
		[SerializeField] private float moveTime;
		[SerializeField] private float interval;

		public override void Play()
		{
			base.Play();

			circle.color = Color.white.WithAlpha(0);

			this.RepeatCoroutine(
				interval: 2 * moveTime + interval,
				action: i => {
					this.ScaleTo(
						time: moveTime,
						transform: hand,
						target: new Vector3(.9f, .9f, .9f),
						settings: new CoroutineTemplate.Settings(
							curve: CoroutineTemplate.EaseInOutCurve,
							pingPong: true
						)
					);

					this.ChangeAlpha(
						time: moveTime * .35f,
						graphic: circle,
						to: 1,
						settings: new CoroutineTemplate.Settings(
							curve: CoroutineTemplate.EaseInOutCurve,
							delay: moveTime * .65f,
							pingPong: true
						)
					);
				}
			);
		}
	}
}