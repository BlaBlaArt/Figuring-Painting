using UnityEngine;

public class CustomLevelsProvider : MonoBehaviour, ILevelProvider
{
    public int LevelsCount => 0;

    public void LoadLevel(int number)
    {
        MenuUI.Instance.restartButton.interactable = true;
    }

    public void Reload()
    {

    }
}