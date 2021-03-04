using TMPro;
using UnityEngine;

public class StartForm : MonoBehaviour
{
	[SerializeField] private Transform levelPanel;
	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private float textShowTime;

	protected void InitInternal()
	{
		levelPanel.gameObject.SetActive(true);
	}

	protected void ShowInternal()
	{
		levelText.text = SLS.Data.Game.Level.ToString();

		this.WaitAndDoCoroutine(
			time: textShowTime,
			action: () => levelPanel.gameObject.SetActive(false)
		);
	}
}