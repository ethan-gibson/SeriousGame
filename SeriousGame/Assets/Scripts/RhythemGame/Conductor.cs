using System;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    [SerializeField] private float songBpm;
    [SerializeField] private float firstNoteOffset;
    
    public static Conductor Instance;

    private float secPerBeat;
    private float songTime;
    private float timeInBeats;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        
        secPerBeat = songBpm / 60f;
    }

    private void Update()
    {
        songTime += Time.deltaTime;
        timeInBeats = songTime / secPerBeat;
    }

    public float GetSongBpm()
    {
        return secPerBeat;
    }

    public float GetSongTime()
    {
        return songTime;
    }

    public float GetTimeInBeats()
    {
        return timeInBeats;
    }
}
