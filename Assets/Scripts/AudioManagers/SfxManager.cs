using UnityEngine;

public class SfxManager : MonoBehaviour
{
    private AudioSource sfxSource;

    public AudioClip diceFalling;
    public AudioClip diceRolling;

    private float sfxVolume = 1f;

    private void Start()
    {
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
    }

    public void PlaySfx(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Vector3.zero, sfxVolume);
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SfxVolume", volume);
        PlayerPrefs.Save();
    }

    public float GetSfxVolume()
    {
        return sfxVolume;
    }
} 
