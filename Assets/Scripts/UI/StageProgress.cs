using System;
using System.Collections.Generic;
using TFPlay.UI;
using UnityEngine;
using UnityEngine.UI;

public class StageProgress : MonoBehaviour
{
	[Serializable]
	public class Stage
	{
		public Image image;
		public Sprite inactiveSprite;
		public Sprite inProgressSprite;
		public Sprite doneSprite;

		public void Inactive()
		{
			image.sprite = inactiveSprite;
		}

		public void InProgress()
		{
			image.sprite = inProgressSprite;
		}

		public void Done()
		{
			image.sprite = doneSprite;
		}
	}

	[SerializeField] List<Stage> stages;

	protected void InitInternal()
	{
		Reset();
	}

	public void Reset()
	{
		foreach (var stage in stages)
			stage.Inactive();

		stages[0].InProgress();
	}

	// Считается от 1
	public void EndStage(int stage)
	{
		stages[stage - 1].Done();
		stages[stage].InProgress();
	}
}