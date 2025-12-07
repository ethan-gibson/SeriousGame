using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RhythmGameManager : MonoBehaviour
{
	public static RhythmGameManager Instance;

	private int totalNotes;
	private int triggerAtNote;

	[SerializeField] private NoteSpawner noteSpawner;
	[SerializeField] private BeatScroller beatScroller;
	[SerializeField] private GameObject startButton;
	[SerializeField] private int playAudioTriggerBeatCount = 6;

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		totalNotes = noteSpawner.GetTotalNotes();
		triggerAtNote = totalNotes - triggerAtNote;
	}

	public void HitNote()
	{
		totalNotes--;

		if (totalNotes == triggerAtNote) { RhythmGameAudioManager.Instance.PlayTriggerSound(); }
		if (totalNotes == 0) { SceneManager.LoadScene("MainMenu"); }
	}

	public void StartGame()
	{
		beatScroller.StartBeatScroller();
		startButton.SetActive(false);
	}
}