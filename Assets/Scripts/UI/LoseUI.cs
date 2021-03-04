public class LoseUI : CanvasGroupUI
{
	public override void Hide()
	{
		base.Hide();
		GameC.Instance.RestartLevel();
	}
}