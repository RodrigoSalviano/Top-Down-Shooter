using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour, IDebugUser
{
    [SerializeField] private AudioClip mainTheme;
    [SerializeField] private AudioClip menuTheme;

    private AudioManager _audiomanager;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;    
    }

    private void OnDisable()
    {
       SceneManager.sceneLoaded -= OnSceneLoaded; 
       //_debugPlayerControls.Debug.Disable();     
    }

    private void Start()
    {
       //InitializeInputActionAsset();
       //CacheDebugInput();

        _audiomanager = GetComponent<AudioManager>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(_audiomanager == null)
        {
            _audiomanager = GetComponent<AudioManager>();
        }
        
        _audiomanager.SetAudioListener();

        if(scene.buildIndex == 0)
        {
            _audiomanager.PlayMusic(menuTheme);
        }
        else
        {
            _audiomanager.PlayMusic(mainTheme);
        }
    }

    //private void Update()
    //{
    //    //TODO: Debug - REMOVER
    //    if (_debugPlayMainTheme.WasPressedThisFrame())
    //    {
    //        _audiomanager.PlayMusic(mainTheme);
    //    } 
    //}

    #region Debug Section

    private PlayerControls _debugPlayerControls;
    private InputAction _debugPlayMainTheme;

    public void InitializeInputActionAsset()
    {
       _debugPlayerControls = new PlayerControls();
       _debugPlayerControls.Debug.Enable(); 
    }

    public void CacheDebugInput()
    {
        _debugPlayMainTheme = _debugPlayerControls.Debug.Get().FindAction("Debug_play_Main_Theme_Music");
    }

    #endregion
}