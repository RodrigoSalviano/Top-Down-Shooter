using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    
    [SerializeField] private int[] screenWidths;
    [Space(10)]
    [SerializeField] private Toggle[] resolutionToggles;
    [SerializeField] private Toggle fullScreen;
    [Space(10)]
    [SerializeField] private Slider[] volumeSliders;

    private int _activeScreenResolutionIndex;
    private bool _isFullsreen;

    private const float ASPECT_RATIO = 16f/9f;

    private const string SCREEN_RES_KEY = "Screen Resolution Index";

    private const string FULLSCREEN_KEY = "Fullscreen";

    private void Start()
    {
        Debug.Log($"Master: {AudioManager.Instance.MasterVolume}; Music: {AudioManager.Instance.MusicVolume}; SFX: {AudioManager.Instance.SfxVolume}");
        volumeSliders[0].value = AudioManager.Instance.MasterVolume;
        volumeSliders[1].value = AudioManager.Instance.MusicVolume;
        volumeSliders[2].value = AudioManager.Instance.SfxVolume;

        SetInitialScreenResolution();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainScene_Rodrigo");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    private void SetInitialScreenResolution()
    {
        if(PlayerPrefs.HasKey(SCREEN_RES_KEY) || PlayerPrefs.HasKey(FULLSCREEN_KEY))
        {
            _isFullsreen = PlayerPrefs.GetInt(FULLSCREEN_KEY) == 1;

            if (_isFullsreen)
            {
                SetFullscreenResolution(true);
                fullScreen.isOn = true;

                SetToggles(_disableAll : true);
            }
            else
            {
                _activeScreenResolutionIndex = PlayerPrefs.GetInt(SCREEN_RES_KEY);
                
                SetToggles(_activeScreenResolutionIndex);

                SetScreenResolution(_activeScreenResolutionIndex);
            }
        }
        else
        {
            SetToggles(_disableAll : true);
            
            SetFullscreenResolution(true);
            fullScreen.isOn = true;
        }
    }

    private void SetToggles(int _activeIndex = 0, bool _disableAll = false)
    {
        for(int i = 0; i < resolutionToggles.Length; i++)
                {
                    resolutionToggles[i].isOn = _disableAll ? false : i == _activeIndex;
                }

        if (_disableAll)
        {
            fullScreen.isOn = true;
        }
    }

    public void SetScreenResolution(int _index)
    {
        if (resolutionToggles[_index].isOn)
        {
            _activeScreenResolutionIndex = _index;
            Screen.SetResolution(screenWidths[_index], (int)(screenWidths[_index] / ASPECT_RATIO), false);
            SetToggles(_activeScreenResolutionIndex);
            PlayerPrefs.SetInt(SCREEN_RES_KEY, _activeScreenResolutionIndex);
            PlayerPrefs.SetInt(FULLSCREEN_KEY, 0);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreenResolution(bool _isFullsreen)
    {
        if (_isFullsreen)
        {
            Resolution[] allResolution = Screen.resolutions;
            Resolution maxResolution = allResolution[allResolution.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
            SetToggles(_disableAll : true);
        }
        else
        {
            SetScreenResolution(_activeScreenResolutionIndex);
        }

        PlayerPrefs.SetInt(FULLSCREEN_KEY, _isFullsreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float _value)
    {
        AudioManager.Instance.SetVolume(_value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float _value)
    {
       AudioManager.Instance.SetVolume(_value, AudioManager.AudioChannel.Music); 
    }

    public void SetSFXVolume(float _value)
    {
        AudioManager.Instance.SetVolume(_value, AudioManager.AudioChannel.SFX);
    }
}