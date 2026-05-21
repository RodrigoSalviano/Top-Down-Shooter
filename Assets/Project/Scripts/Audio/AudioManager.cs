using UnityEngine;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    [Header ("Volume Settings")]
    [SerializeField] private float _masterVolumePerct = 1f;
    [SerializeField] private float _musicVolumePerct  = 1f;
    [SerializeField] private float _sfxVolumePerct    = 1f;

    private AudioSource[] _audioSources;
    private int _activeAudioSourcesIndex;

    private Transform _audioListener_T;
    private Transform _player_T;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;

        }

        Instance = this;

        _audioSources = new AudioSource[2];

        for(int i = 0; i < _audioSources.Length; i++)
        {
            GameObject newAudioSource = new GameObject("Audio Source" + (i + 1));
            _audioSources[i] = newAudioSource.AddComponent<AudioSource>();
            _audioSources[i].loop = true;
            _audioSources[i].volume = _musicVolumePerct * _masterVolumePerct;
            _audioSources[i].transform.parent = transform;
        }

        _audioListener_T = GetComponentInChildren<AudioListener>().transform;
        _player_T = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        if(_audioListener_T != null && _player_T != null)
        {
            _audioListener_T.position = _player_T.position;
        }
    }

    public void PlaySoundFX(AudioClip _clip, Vector3 _pos)
    {
        if(_clip != null)
        {
            AudioSource.PlayClipAtPoint(_clip, _pos, _sfxVolumePerct * _masterVolumePerct);
        }
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
}