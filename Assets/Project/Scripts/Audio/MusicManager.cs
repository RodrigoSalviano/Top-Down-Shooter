using UnityEngine;
using UnityEngine.InputSystem;

public class MusicManager : MonoBehaviour, IDebugUser
{
    [SerializeField] private AudioClip mainTheme;
    [SerializeField] private AudioClip menuTheme;

    private AudioManager _audiomanager;

    private void Start()
    {
        InitializeInputActionAsset();
        CacheDebugInput();

        _audiomanager = GetComponent<AudioManager>();
        _audiomanager.PlayMusic(menuTheme);
    }

    private void Update()
    {
        //TODO: Debug - REMOVER
        if (_debugPlayMainTheme.WasPressedThisFrame())
        {
            _audiomanager.PlayMusic(mainTheme);
        } 
    }

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