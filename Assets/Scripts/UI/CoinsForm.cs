using TMPro;
using UnityEngine;

public class CoinsForm : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI coinsCount;

	private void Start()
	{
		GameC.Instance.OnInitComplite += Init;
	}

	private void Init()
	{
		SLS.Data.Game.Coins.OnValueChanged += SetConsCount;
		SetConsCount(SLS.Data.Game.Coins.Value);
	}

	private void Show()
	{
		SetConsCount(SLS.Data.Game.Coins.Value);
	}

	private void SetConsCount(int count)
	{
		coinsCount.SetText($"{count}");
	}
}