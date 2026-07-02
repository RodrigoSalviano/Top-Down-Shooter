using UnityEngine;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    public enum AudioChannel
    {
        Music,
        SFX,
        Master
    }

    [Header ("Volume Settings")]
    [SerializeField] private float _masterVolumePerct = 1f;
    [SerializeField] private float _musicVolumePerct  = 1f;
    [SerializeField] private float _sfxVolumePerct    = 1f;

    public float MasterVolume => _masterVolumePerct;
    public float MusicVolume => _musicVolumePerct;
    public float SfxVolume => _sfxVolumePerct;

    private AudioSource _2DAudioSource;
    private AudioSource[] _audioSources;
    private SoundLibrary _soundLibrary;
    private int _activeAudioSourcesIndex;

    private Transform _audioListener_T;
    private Player _player;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;

        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _soundLibrary = GetComponent<SoundLibrary>();

        _audioSources = new AudioSource[2];

        LoadPlayerPrefs();

        for(int i = 0; i < _audioSources.Length; i++)
        {
            GameObject newAudioSource = new GameObject("Audio Source" + (i + 1));
            _audioSources[i] = newAudioSource.AddComponent<AudioSource>();
            _audioSources[i].loop = true;
            _audioSources[i].volume = _musicVolumePerct * _masterVolumePerct;
            _audioSources[i].transform.parent = transform;
        }

        GameObject new2DSource = new GameObject("2D Audio Source");
        _2DAudioSource = new2DSource.AddComponent<AudioSource>();
        _2DAudioSource.transform.parent = transform;

        _audioListener_T = GetComponentInChildren<AudioListener>().transform;

        SetAudioListener();
    }

    private void Update()
    {
        if(_audioListener_T != null && _player != null)
        {
            _audioListener_T.position = _player.transform.position;
        }
    }

    public void SetVolume(float _volumePerct, AudioChannel _channel)
    {
        switch (_channel)
        {
            case AudioChannel.Master:
                _masterVolumePerct =  _volumePerct;
                break;
            case AudioChannel.Music:
                _musicVolumePerct = _volumePerct;
                break;
            case AudioChannel.SFX:
                _sfxVolumePerct =  _volumePerct;
                break;
            
        }

        _audioSources[0].volume = _musicVolumePerct * _masterVolumePerct;
        _audioSources[1].volume = _musicVolumePerct * _masterVolumePerct;

        SavePlayerPrefs();
    }

    public void PlaySoundFX(AudioClip _clip, Vector3 _pos)
    {
        if(_clip != null)
        {
            AudioSource.PlayClipAtPoint(_clip, _pos, _sfxVolumePerct * _masterVolumePerct);
        }
    }

    public void PlaySoundFX(string _groupName, Vector3 _pos)
    {
        PlaySoundFX(_soundLibrary.GetAudioClipFromGroup(_groupName), _pos);
    }

    public void PlaySound2D(string _groupName)
    {
        _2DAudioSource.PlayOneShot(_soundLibrary.GetAudioClipFromGroup(_groupName), _sfxVolumePerct * _masterVolumePerct);
    }

    public void PlayMusic(AudioClip _clip, float _fadeDuration = 1f)
    {
        _activeAudioSourcesIndex = 1 - _activeAudioSourcesIndex;
        _audioSources[_activeAudioSourcesIndex].clip = _clip;
        _audioSources[_activeAudioSourcesIndex].Play();

        StartCoroutine(AnimateMusicCrossFade(_fadeDuration));
    }

    IEnumerator AnimateMusicCrossFade(float duration)
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;

            _audioSources[_activeAudioSourcesIndex].volume = Mathf.Lerp(0, _musicVolumePerct * _masterVolumePerct, percent);
            _audioSources[1 - _activeAudioSourcesIndex].volume = Mathf.Lerp( _musicVolumePerct * _masterVolumePerct, 0, percent);

            yield return null;
        }
    }

    private void PlayGameOverMusic()
    {
        PlayMusic(_soundLibrary.GetAudioClipFromGroup("Game Over"));
    }

    public void SetAudioListener()
    {
        if(FindFirstObjectByType<Player>() != null)
        {
           _player = FindFirstObjectByType<Player>();
           _player.OnDeath += PlayGameOverMusic; 
        }
    }

    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetFloat("Master Volume", _masterVolumePerct);
        PlayerPrefs.SetFloat("Music Volume", _musicVolumePerct);
        PlayerPrefs.SetFloat("SFX Volume", _sfxVolumePerct);
        PlayerPrefs.Save();
    }

    private void LoadPlayerPrefs()
    {
        if(PlayerPrefs.HasKey("Master Volume"))
        {
            _masterVolumePerct = PlayerPrefs.GetFloat("Master Volume");
        }
        else
            _masterVolumePerct = .5f;
        if(PlayerPrefs.HasKey("Music Volume"))
        {
            _musicVolumePerct = PlayerPrefs.GetFloat("Music Volume");
        }
        else
            _musicVolumePerct = .5f;
        if(PlayerPrefs.HasKey("SFX Volume"))
        {
            _sfxVolumePerct = PlayerPrefs.GetFloat("SFX Volume");
        }
        else
            _sfxVolumePerct = .5f;

    }

}