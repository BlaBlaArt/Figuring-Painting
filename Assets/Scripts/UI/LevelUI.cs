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
			GameC.Instance.OnLevelStart += SetLevel;
		}

		private void SetLevel(int levelNumber)
		{
			Debug.Log("Level Number" + levelNumber);
			
			if (levelNumber == 0)
			{
				levelText.text = string.Format(levelFormat, levelNumber + 1);
			}
			else if(levelNumber == 2)
			{
				levelText.text = string.Format(levelFormat, levelNumber);
			}
			else
			{
				levelText.text = string.Format(levelFormat, levelNumber - 1);
			}
		}

		public void Restart()
		{
			GameC.Instance.RestartLevel();
		}
	}
}