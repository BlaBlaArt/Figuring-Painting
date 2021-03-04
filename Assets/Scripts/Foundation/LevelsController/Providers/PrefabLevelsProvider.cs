using System.Collections.Generic;
using UnityEngine;

public class PrefabLevelsProvider : MonoBehaviour, ILevelProvider
{
    [SerializeField] private List<GameObject> levels = new List<GameObject>();

    private GameObject currentLevel;
    private int currentLevelIndex;

    public int LevelsCount => levels.Count;

    public void LoadLevel(int number)
    {
        currentLevelIndex = number;
        Load(currentLevelIndex);
    }

    public void Reload()
    {
        Load(currentLevelIndex);
    }

    public void Load(int index)
    {
        if (currentLevel != null)
            Destroy(currentLevel);

        currentLevel = Instantiate(levels[index]);
        MenuUI.Instance.restartButton.interactable = true;
    }
}