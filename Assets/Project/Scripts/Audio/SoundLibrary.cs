using UnityEngine;
using System.Collections.Generic;

public class SoundLibrary : MonoBehaviour
{   
    [SerializeField] private SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> soundDictionary = new Dictionary<string, AudioClip[]>();

    private void Awake()
    {
        //Popular o dictionary
        foreach(SoundGroup group in soundGroups)
        {
            soundDictionary.Add(group.groupName, group.groupClips);
        }
    }
    
    public AudioClip GetAudioClipFromGroup(string _groupName)
    {
        if(soundDictionary.TryGetValue(_groupName, out AudioClip[] _clips))
        {
            if(_clips.Length == 0)
            {
                Debug.LogWarning("this sound group is empty: " + _groupName);
                return null;
            }

            return _clips[Random.Range(0, _clips.Length)];

        }

        Debug.LogWarning($"No group found with {_groupName}.");
        return null;
    }

}

[System.Serializable]
public struct SoundGroup
{
    public string groupName;
    public AudioClip[] groupClips;
}