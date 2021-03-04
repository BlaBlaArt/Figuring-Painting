using System.Collections.Generic;
using UnityEngine;

public class WinUI : CanvasGroupUI
{
	[SerializeField] private List<ParticleSystem> effects;

	public override void Show()
	{
		base.Show();

		foreach (var effect in effects)
			effect.Play();
	}

	public override void Hide()
	{
		base.Hide();
		GameC.Instance.NextLevel();
	}
}