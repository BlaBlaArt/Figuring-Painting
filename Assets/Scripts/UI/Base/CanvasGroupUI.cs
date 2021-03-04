using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupUI : MonoBehaviour
{
	[SerializeField] private float fadeTime = 0.2f;

	private CanvasGroup canvasGroup;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public virtual void Show()
	{
		gameObject.SetActive();

		StopAllCoroutines();
		this.LerpCoroutine(
			time: fadeTime,
			from: canvasGroup.alpha,
			to: 1,
			action: a => canvasGroup.alpha = a
		);
	}

	public virtual void Hide()
	{
		StopAllCoroutines();
		this.LerpCoroutine(
			time: fadeTime,
			from: canvasGroup.alpha,
			to: 0,
			action: a => canvasGroup.alpha = a,
			onEnd: () => gameObject.SetInactive()
		);
	}
}