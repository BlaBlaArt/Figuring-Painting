using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : CanvasGroupUI
{
	[SerializeField] private List<ParticleSystem> effects;
	[SerializeField] private Button closeButton;

	public override void Show()
	{
		base.Show();

		closeButton.interactable = true;
		
		foreach (var effect in effects)
			effect.Play();
	}

	public override void Hide()
	{
		base.Hide();
		GameC.Instance.NextLevel();
	}
}