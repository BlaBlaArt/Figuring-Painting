using System.Collections.Generic;
using UnityEngine;

namespace TFPlay.UI
{
	public class TutorialController : MonoBehaviour
	{
		[SerializeField] private List<TutorialItem> tutorialItems;

		private int currentTutorialNumber;

		private void Start()
		{
			GameC.Instance.OnLevelStart += ShowTutorial;
			GameC.Instance.OnLevelEnd += Pass;

			foreach (var tutorialItem in tutorialItems)
				tutorialItem.SetInactive();
		}

		private void ShowTutorial(int levelNumber)
		{
			GameC.Instance.OnLevelStart -= ShowTutorial;
			return;

			// Логику ниже можно переписать под себя

			// Пример
			if (levelNumber == 1)
			{
				// Назначаем номер туториала
				currentTutorialNumber = 0;

				// Проверяем есть ли туториал в списке пройденных
				if (SLS.Data.TutorialData.Passed.Value.Contains(currentTutorialNumber))
					return;

				// Запускаем туториал
				tutorialItems[currentTutorialNumber]?.Play();
			}
		}

		private void Pass(bool _)
		{
			GameC.Instance.OnLevelEnd -= Pass;
			SLS.Data.TutorialData.Passed.Value.Add(currentTutorialNumber);
			SLS.Save();
		}
	}
}