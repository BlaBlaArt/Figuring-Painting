using TFPlay.UI;
using UnityEngine;

public class StageDemoForm : MonoBehaviour
{
	[SerializeField] public ProgressBar ProgressBar;

	protected void ShowInternal()
	{
		ProgressBar.Reset();
	}
}