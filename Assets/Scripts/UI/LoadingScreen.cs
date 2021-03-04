using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI loadingTMP;
	[SerializeField] private string loadingText = "Loading";
	[SerializeField] private float dotsSpeed = 1;
	
	private void Update()
	{
		if (!gameObject.activeSelf)
			return;

		loadingTMP.text = loadingText;

		for (int i = 0; i < Mathf.Floor((Time.time * dotsSpeed) % 4); i++)
			loadingTMP.text += ".";
	}
}
