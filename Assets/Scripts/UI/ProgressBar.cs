using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TFPlay.UI
{
	public class ProgressBar : MonoBehaviour
	{
		[SerializeField] private Image bar;
		[SerializeField] private TextMeshProUGUI text;
		[SerializeField] private string format = "{0} %";

		protected void InitInternal()
		{
			Reset();
		}

		public void Reset()
		{
			SetProgress(0);
		}

		public void SetProgress(float progress)
		{
			bar.fillAmount = progress;
			text.text = string.Format(format, 100 * progress);
		}

		public void SetProgress(int current, int count)
		{
			bar.fillAmount = (float) current / count;
			text.text = string.Format(format, current, count);
		}
	}
}