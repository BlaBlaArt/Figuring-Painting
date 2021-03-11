using DG.Tweening;
using UnityEngine;

namespace TFPlay.UI
{
	public abstract class TutorialItem : MonoBehaviour
	{
		public virtual void Play()
		{
			gameObject.SetActive();
			GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetEase(Ease.Flash);
		}

		public void Hide()
		{
			GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Flash).OnComplete(() => { gameObject.SetInactive(); });
		}
	}
}