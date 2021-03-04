using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsUI : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Sprites")]
    [SerializeField] private Sprite soundOn;
    [SerializeField] private Sprite soundOff;
    [SerializeField] private Sprite vibrationOn;
    [SerializeField] private Sprite vibrationOff;

    [Space(30)]
    [SerializeField] private Button soundButton;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Image underlayImage;

    [Space(30)]
    [SerializeField] private Image settings;
    [SerializeField] private Image sound;
    [SerializeField] private Image vibration;
#pragma warning restore 0649

    private bool settingsOpened = false;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        sound.sprite = SLS.Data.Settings.SoundEnabled.Value ? soundOn : soundOff;
        vibration.sprite = SLS.Data.Settings.VibrationEnabled.Value ? vibrationOn : vibrationOff;
    }

    public void ShowSettingsButton()
    {
        settings.gameObject.SetActive(true);
    }

    public void HideSettingsButton()
    {
        settings.gameObject.SetActive(false);

        StopAllCoroutines();
        StartCoroutine(OpenClose(false));
    }

    public void OnClickOnSettings()
    {
        settingsOpened = !settingsOpened;

        StopAllCoroutines();
        StartCoroutine(OpenClose(settingsOpened));
    }

    public void OnSoundClick()
    {
        var soundEnable = !SLS.Data.Settings.SoundEnabled.Value;
        SLS.Data.Settings.SoundEnabled.Value = soundEnable;

        sound.sprite = soundEnable ? soundOn : soundOff;

        // todo: togle sound
    }

    public void OnVibrationClick()
    {
        var vibrationEnable = !SLS.Data.Settings.VibrationEnabled.Value;
        SLS.Data.Settings.VibrationEnabled.Value = vibrationEnable;

        vibration.sprite = vibrationEnable ? vibrationOn : vibrationOff;

        Taptic.tapticOn = vibrationEnable;
    }

    private IEnumerator OpenClose(bool open)
    {
        var time = 0.2f;
        var t = 0f;

        var alpha = 0f;

        if (open)
        {
            // soundButton.gameObject.SetActive(true);
            vibrationButton.gameObject.SetActive(true);
        }

        while (t < time)
        {
            t += Time.deltaTime;
            alpha = open ? (t / time) : 1 - (t / time);
            sound.color = sound.color.WithAlpha(alpha);
            vibration.color = vibration.color.WithAlpha(alpha);
            soundButton.image.color = soundButton.image.color.WithAlpha(alpha);
            vibrationButton.image.color = vibrationButton.image.color.WithAlpha(alpha);
            underlayImage.color = underlayImage.color.WithAlpha(alpha);
            yield return null;
        }

        alpha = open ? 1 : 0;
        sound.color = sound.color.WithAlpha(alpha);
        vibration.color = vibration.color.WithAlpha(alpha);
        soundButton.image.color = soundButton.image.color.WithAlpha(alpha);
        vibrationButton.image.color = vibrationButton.image.color.WithAlpha(alpha);
        underlayImage.color = underlayImage.color.WithAlpha(alpha);

        if (!open)
        {
            // soundButton.gameObject.SetActive(false);
            vibrationButton.gameObject.SetActive(false);
        }
    }
}