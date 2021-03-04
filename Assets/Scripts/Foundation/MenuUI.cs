using System;
using TFPlay.UI;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private SettingsUI settingsUI;

    [SerializeField] private WinUI winUI;
    [SerializeField] private LoseUI loseUI;
    [SerializeField] private LevelUI levelUI;

    [SerializeField] private Transform coinsUI;

    [SerializeField] private Image blackImage;
    [SerializeField] public Button restartButton;

	public static MenuUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        GameC.Instance.OnInitComplite += Init;
    }

    public void Init()
    {
        GameC.Instance.OnLevelEnd += OnLevelEnd;
    }

    public void OnLevelEnd(bool playerWon)
    {
        if (playerWon)
            winUI.Show();
        else
            loseUI.Show();
    }

    public void BlackScreenFadeOut(Action onEnd = null)
    {
        this.ChangeAlpha(
            time: 0.2f,
            graphic: blackImage,
            to: 0,
            onEnd: onEnd
        );
    }

    public void BlackScreenFadeIn(Action onEnd = null)
    {
        this.ChangeAlpha(
            time: 0.2f,
            graphic: blackImage,
            to: 1,
            onEnd: onEnd
        );
    }
}