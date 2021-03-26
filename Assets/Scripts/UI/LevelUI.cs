using TMPro;
using UnityEngine;

namespace TFPlay.UI
{
	public class LevelUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI levelText;
		[SerializeField] private string levelFormat = "Level {0}";

		private void Awake()
		{
			GameC.Instance.OnInitComplite += Init;
		}

		private void Init()
		{
			GameC.Instance.OnLevelUIUpdate += SetLevel;
		}

		private void SetLevel(int levelNumber)
		{
			//Debug.Log("Level Number" + levelNumber);
			levelText.text = string.Format(levelFormat, levelNumber);

		}

		public void Restart()
		{
			GameC.Instance.RestartLevel();
		}
	}
}