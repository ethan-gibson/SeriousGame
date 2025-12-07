using System;
using UnityEngine;

public class RhythmGameAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip startClip;
    [SerializeField] private AudioClip triggerClip;
    
    private AudioSource audioSource;
    
    public static RhythmGameAudioManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(startClip);
    }

    public void PlayTriggerSound()
    {
        audioSource.PlayOneShot(triggerClip);
    }
}
