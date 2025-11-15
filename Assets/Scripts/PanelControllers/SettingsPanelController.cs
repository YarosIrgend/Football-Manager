using System;
using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    private GameObject _currentPanel;
    public GameObject GameSettingsPanel;
    public GameObject SoundSettingsPanel;
    public GameObject VisualSettingsPanel;

    public void Start()
    {
        _currentPanel = GameSettingsPanel;
        GameSettingsPanel.SetActive(true);
    }

    public void GoInGameSettings()
    {
        _currentPanel.SetActive(false);
        _currentPanel = GameSettingsPanel;
        GameSettingsPanel.SetActive(true);
    }

    public void GoInSoundSettings()
    {
        _currentPanel.SetActive(false);
        _currentPanel = SoundSettingsPanel;
        SoundSettingsPanel.SetActive(true);
    }

    public void GoInVisualSettings()
    {
        _currentPanel.SetActive(false);
        _currentPanel = VisualSettingsPanel;
        VisualSettingsPanel.SetActive(true);
    }
}
