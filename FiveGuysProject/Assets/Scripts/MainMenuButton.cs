using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Animator mAnimator;
    Resolution[] resolutions;
    public TMPro.TMP_Dropdown resolutionDropdown;
    float newVolume;
    [SerializeField] GameObject Audio;
    [SerializeField] GameObject PlayerAudio;
    [SerializeField] GameObject gunHitAudio;
    [SerializeField] SavedSettings SavedSettings;
    [SerializeField] Slider VolSlider;
    [SerializeField] Slider MusicVolumeSlider;
    [SerializeField] Slider SoundEffectSlider;
    [SerializeField] Slider SensSlider;

    private void Start()
    {
        MusicVolumeSlider.value = SavedSettings.MusicVolume;
        SoundEffectSlider.value = SavedSettings.SoundEffectVoulume;
        VolSlider.value = SavedSettings.masterVolume;
        SensSlider.value = SavedSettings.Sensitivity;
        AudioListener.volume = SavedSettings.masterVolume;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        
        int currentResolution = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolution = i;
            }
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Settings()
    {
        mAnimator.SetTrigger("isSettings");
        //mAnimator.SetBool("isSetting", true);
    }
    public void Back()
    {
        mAnimator.SetTrigger("isBack");
        
    }

    public void SetQuality(int qualityIndex)
    {
        Debug.Log(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
        
    }
    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
    public void SetResolution(int resolitionIndex)
    {
        Debug.Log(resolitionIndex);
        Resolution resolution = resolutions[resolitionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetAudio(float volume)
    {
        AudioListener.volume = volume;
    }
    public void ChangeVolume(float value)
    {
        SavedSettings.masterVolume = value;
        AudioListener.volume = value;
        newVolume = value;
    }
    public void ChangeSenitivity(float value)
    {
        SavedSettings.Sensitivity = value;

    }
    public void ChangeMusicVolume(float value)
    {
 
        Audio = GameObject.FindGameObjectWithTag("AudioManager");
        SavedSettings.MusicVolume = value;
        if (Audio != null)
        {
            Audio.GetComponent<AudioManager>().SetMusicAudio(value / 2);
        }

    }
    public void ChangeEffectsVolume(float value)
    {
        Audio = GameObject.FindGameObjectWithTag("AudioManager");
        PlayerAudio = GameObject.FindGameObjectWithTag("Player");
        SavedSettings.SoundEffectVoulume = value;
        if (Audio != null)
        {
            Audio.GetComponent<AudioManager>().SetEffectsAudio(value);

        }
        PlayerAudio.GetComponent<PlayerController>().SetAudio(value);
    }
}
