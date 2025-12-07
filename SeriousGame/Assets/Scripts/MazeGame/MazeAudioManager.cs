using System;
using UnityEngine;

public class MazeAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip beginAudio;
    [SerializeField] private AudioClip fogTrigger;
    [SerializeField] private AudioClip rebuildTrigger;
    
    public static MazeAudioManager Instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(beginAudio);
    }

    public void PlayFogTrigger()
    {
        audioSource.PlayOneShot(fogTrigger);
    }

    public void PlayRebuildTrigger()
    {
        audioSource.PlayOneShot(rebuildTrigger);
    }
}
