using TMPro;
using UnityEngine;

namespace TFPlay.UI
{
	public class Tutorial_TypeText : TutorialItem
	{
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private float time;
		[SerializeField] private TimeType timeType;

		private enum TimeType
		{
			TimeForEachChar,
			FullTime
		}

		public override void Play()
		{
			base.Play();

			var chars = text.text.ToCharArray();
			text.text = "";

			var interval = timeType == TimeType.FullTime ? time / chars.Length : time;

			this.RepeatCoroutine(
				repetitions: chars.Length,
				interval: interval,
				action: i => text.text += chars[i]
			);
		}
	}
}