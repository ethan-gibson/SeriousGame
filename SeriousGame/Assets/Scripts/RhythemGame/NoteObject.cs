using System;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    private LaneController targetLane;
    private bool canBeHit;

    public void NoteHit()
    {
        gameObject.SetActive(false);
        RhythmGameManager.Instance.HitNote();
    }

    
    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.gameObject.CompareTag("RhythmTrigger")) { return; }
        canBeHit = true;
        targetLane?.RegisterNote(this);
    }

    private void OnTriggerExit(Collider _other)
    {
        if (!_other.CompareTag("RhythmTrigger")) { return; }
        canBeHit = false;
        targetLane?.DeregisterNote(this);

        if (!gameObject.activeSelf) { return; }
        RhythmGameManager.Instance.HitNote();
        gameObject.SetActive(false);
    }

    public void SetTargetLane(LaneController _targetLane)
    {
        targetLane = _targetLane;
    }
    
    public bool CanBeHit()
    {
        return canBeHit;
    }
}
