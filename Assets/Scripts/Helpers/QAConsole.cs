using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QAConsole : MonoBehaviour
{
	[SerializeField] private Button showQAConsoleButton;
	[SerializeField] private GameObject qaPanel;
	[SerializeField] private InputField levelInputField;
	[SerializeField] private Button loadLevelButton;

	private bool isOpen = false;

	private void Awake()
	{
		loadLevelButton.onClick.AddListener(LoadLevel);
		showQAConsoleButton.onClick.AddListener(OpenConsole);
	}

	private void OpenConsole()
	{
		isOpen = !isOpen;
		qaPanel.SetActive(isOpen);
	}

	private void LoadLevel()
	{
		Int32.TryParse(levelInputField.text, out int levelNumber);

		qaPanel.SetActive(false);

		UnloadLogic(levelNumber);
		LoadLogic(levelNumber);
	}

	private void LoadLogic(int levelNumber)
	{
		SLS.Data.Game.Level.Value = levelNumber;

		// TODO: Переделать
		SceneManager.LoadSceneAsync(levelNumber, LoadSceneMode.Additive);
	}

	private void UnloadLogic(int levelNumber)
	{
		// TODO: Переделать
		SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(1));

		SLS.Data.Game.Level.Value = levelNumber;
	}
}