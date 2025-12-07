using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
	[SerializeField] private GameObject noteGameObject;
	[SerializeField] private GameObject beatScroller;
	[SerializeField] private List<NoteData> noteData;
	[SerializeField] private int notesShownInAdvance;
	[SerializeField] private LaneController topTriggerPoint;
	[SerializeField] private LaneController bottomTriggerPoint;
	[SerializeField] private Transform topSpawnPoint;
	[SerializeField] private Transform bottomSpawnPoint;
	
	private List<NoteData> runtimeNoteData;

	private void Awake()
	{
		runtimeNoteData = new List<NoteData>(noteData);
	}

	private void Update()
	{
		if (!Conductor.Instance) return;
		
		List<NoteData> _beatsToRemove = new List<NoteData>();
		float _currentTime = Conductor.Instance.GetTimeInBeats();
		
		foreach (var _beat in runtimeNoteData.Where(_beat => _beat.BeatTime > _currentTime && _beat.BeatTime < _currentTime + notesShownInAdvance))
		{
			spawnNoteInLane(_beat.LaneIndex);
			_beatsToRemove.Add(_beat);
		}
		
		// Remove from runtime list only
		foreach (var _beatToRemove in _beatsToRemove)
		{
			runtimeNoteData.Remove(_beatToRemove);
		}
	}

	private void spawnNoteInLane(int _lane)
	{
		Debug.Log("Spawning note in lane: " + _lane);
		var _spawnPos = _lane == 0 ? topSpawnPoint.position : bottomSpawnPoint.position;
		GameObject _note = Instantiate(noteGameObject, _spawnPos, Quaternion.identity, beatScroller.transform);

		NoteObject _noteObj = _note.GetComponent<NoteObject>();
		if (_noteObj != null)
		{
			_noteObj.SetTargetLane(_lane == 0 ? topTriggerPoint : bottomTriggerPoint);
		}
	}

	public int GetTotalNotes()
	{
		return runtimeNoteData.Count; // Return runtime count
	}
}