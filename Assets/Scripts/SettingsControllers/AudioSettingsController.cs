using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsController : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Managers")]
    public MusicManager musicManager;
    public SfxManager sfxManager;

    private void Start()
    {
        musicSlider.value = musicManager.GetMusicVolume() * 100f;
        sfxSlider.value = sfxManager.GetSfxVolume() * 100f;
    }

    public void OnMusicSliderChanged(float value)
    {
        float normalized = value / 100f;
        musicManager.SetMusicVolume(normalized);
    }

    public void OnSfxSliderChanged(float value)
    {
        float normalized = value / 100f;
        sfxManager.SetSfxVolume(normalized);
    }
}