using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{
    public GameObject soundSettingsPanel;
    public Button soundSettingsButton;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    [Header("Mute Toggle")]
    public Toggle muteToggle;

    private bool isOpen = false;

    public void Initialize()
    {
        soundSettingsButton.onClick.AddListener(SoundSettingsClicked);

        float savedMasterVolume = PlayerPrefs.GetFloat("MainVolume", 1.0f);
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        float savedUIVolume = PlayerPrefs.GetFloat("UIVolume", 1.0f);
        bool isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;

        AudioManager.Instance.audioMixer.GetFloat("MainVolume", out float masterVolume);// find current master volume
        AudioManager.Instance.audioMixer.GetFloat("MusicVolume", out float musicVolume);// find current music volume
        AudioManager.Instance.audioMixer.GetFloat("SFXVolume", out float sfxVolume);// find current sfx volume
        AudioManager.Instance.audioMixer.GetFloat("UIVolume", out float uiVolume);// find current ui volume

        masterSlider.value = Mathf.Pow(10, masterVolume / 20); // Convert dB back to linear for slider
        musicSlider.value = Mathf.Pow(10, musicVolume / 20);
        sfxSlider.value = Mathf.Pow(10, sfxVolume / 20); 
        uiSlider.value = Mathf.Pow(10, uiVolume / 20);

        AudioManager.Instance.SetVolume("MainVolume", savedMasterVolume);
        AudioManager.Instance.SetVolume("MusicVolume", savedMusicVolume);
        AudioManager.Instance.SetVolume("SFXVolume", savedSFXVolume);
        AudioManager.Instance.SetVolume("UIVolume", savedUIVolume);
        AudioManager.Instance.ToggleMute(isMuted);

        muteToggle.isOn = isMuted;

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        uiSlider.onValueChanged.AddListener(OnUIVolumeChanged);

        muteToggle.onValueChanged.AddListener(OnMuteToggleChanged);

        soundSettingsPanel.SetActive(false);
    }

    public void SoundSettingsClicked()
    {
        if (isOpen)
        {
            isOpen = false;
            AudioManager.Instance.PlayUISound("UI_Close");
            soundSettingsPanel.SetActive(false);
        }
        else
        {
            isOpen = true;
            AudioManager.Instance.PlayUISound("UI_Open");
            soundSettingsPanel.SetActive(true);
        }
    }

    // Called when the slider value changes
    private void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("MainVolume", value);
        //PlayerPrefs.SetFloat("MainVolume", value);  // Save the value
        //PlayerPrefs.Save();
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("MusicVolume", value);
        //PlayerPrefs.SetFloat("MainVolume", value);  // Save the value
        //PlayerPrefs.Save();
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("SFXVolume", value);
        //PlayerPrefs.SetFloat("MainVolume", value);  // Save the value
        //PlayerPrefs.Save();
    }

    private void OnUIVolumeChanged(float value)
    {
        AudioManager.Instance.SetVolume("UIVolume", value);
        //PlayerPrefs.SetFloat("MainVolume", value);  // Save the value
        //PlayerPrefs.Save();
    }

    // Called when the toggle is changed
    private void OnMuteToggleChanged(bool isMuted)
    {
        AudioManager.Instance.ToggleMute(isMuted);
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);  // Save the mute state
        PlayerPrefs.Save();
    }
}