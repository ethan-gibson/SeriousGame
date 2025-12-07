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

    private bool isPlaying;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;

        secPerBeat = 60f / songBpm;
    }

    private void Update()
    {
        if (!isPlaying) { return; }
        songTime += Time.deltaTime;
        timeInBeats = songTime / secPerBeat;
        Debug.Log(timeInBeats);
    }

    public void StartTrack()
    {
        isPlaying = true;
    }

    public float GetSongBpm()
    {
        return songBpm;
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
