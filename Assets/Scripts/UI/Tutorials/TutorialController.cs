using System;
using System.Collections.Generic;
using UnityEngine;

namespace TFPlay.UI
{
	public class TutorialController : MonoBehaviour
	{
		[SerializeField] private List<TutorialItem> tutorialItems;

		private int currentTutorialNumber;

		private bool isAssembleStageInput;
		private bool isFirstInput;
		private bool isSecondInput;
		private bool is2Stage;
		
		private void Start()
		{
			is2Stage = false;
			isFirstInput = true;
			isSecondInput = true;
			GameC.Instance.OnLevelStart += ShowTutorial;
			GameC.Instance.OnLevelEnd += Pass;
			GameC.Instance.OnLevelUnload += OnLevelUnload; 
			GameC.Instance.OnLevelStart += OnLevelStart;
			GameC.Instance.OnShowTutorial += OnShowTutorial;
			GameC.Instance.OnAssembleStage += OnAssembleStage;
			InputSystem.Instance.OnTouch += OnTouch;
			
			foreach (var tutorialItem in tutorialItems)
				tutorialItem.SetInactive();
		}

		private void OnLevelUnload()
		{
			foreach (var tutorialItem in tutorialItems)
				tutorialItem.SetInactive();
		}

		private void OnSwipe(InputSystem.SwipeDirection obj)
		{
			Debug.Log("SWIPE");
			if (isSecondInput)
			{
				Debug.Log("SWIPE_1");
				isSecondInput = false;
				OnDiableTutorial(1);
				InputSystem.Instance.OnSwipe -= OnSwipe;
			}
			else if (is2Stage)
			{
				Debug.Log("SWIPE_2");
				is2Stage = false;
				OnDiableTutorial(3);
				InputSystem.Instance.OnSwipe -= OnSwipe;
			}
		}

		private void OnAssembleStage()
		{
			InputSystem.Instance.OnTouch += OnTouch;
			OnShowTutorial(2);
			isAssembleStageInput = true;
		}
		
		private void OnLevelStart(int obj)
		{
			InputSystem.Instance.OnTouch += OnTouch;
			isFirstInput = true;
			isSecondInput = true;
			isAssembleStageInput = false;
		}

		private void OnTouch()
		{
			if (isFirstInput)
			{
				isFirstInput = false;
				OnDiableTutorial(0);
				GameC.Instance.OnFirstInput?.Invoke();
				this.WaitAndDoCoroutine(1f,() => InputSystem.Instance.OnSwipe += OnSwipe);
				InputSystem.Instance.OnTouch -= OnTouch;
			}
			else if (isAssembleStageInput)
			{
				Debug.Log("Assemble");
				isAssembleStageInput = false;
				OnDiableTutorial(2);
				InputSystem.Instance.OnTouch -= OnTouch;
			}
		}

		private void OnDiableTutorial(int numOfTutorial)
		{
			tutorialItems[numOfTutorial]?.Hide();
		}

		private void ShowTutorial(int levelNumber)
		{
			is2Stage = false;
			isFirstInput = true;
			isSecondInput = true;

			currentTutorialNumber = 0;
				
			tutorialItems[currentTutorialNumber]?.Play();
		}

		private void OnShowTutorial(int numTutor)
		{
			foreach (var tutorialItem in tutorialItems)
				tutorialItem.SetInactive();
			
			if (numTutor == 3)
			{
				this.WaitAndDoCoroutine(0.5f, () =>
				{
					tutorialItems[numTutor]?.Play();
					is2Stage = true;
					InputSystem.Instance.OnSwipe += OnSwipe;
				});
			}
			else
			{
				tutorialItems[numTutor]?.Play();
			}
		}

		private void Pass(bool _)
		{
			is2Stage = false;
			isFirstInput = true;
			isSecondInput = true;
			
			//GameC.Instance.OnLevelEnd -= Pass;
			SLS.Data.TutorialData.Passed.Value.Add(currentTutorialNumber);
			SLS.Save();
		}
	}
}