using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer mainMixer;
    public void UpdateSoundVolume(float volume)
    {
        mainMixer.SetFloat("SoundsVolume", volume);
    }

    public void UpdateMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", volume);
    }
}
