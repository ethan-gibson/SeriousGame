using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RhythmGameManager : MonoBehaviour
{
	public static RhythmGameManager Instance;

	private int totalNotes;
	private int triggerAtNote;
	private int points;

	[SerializeField] private NoteSpawner noteSpawner;
	[SerializeField] private BeatScroller beatScroller;
	[SerializeField] private GameObject startButton;
	[SerializeField] private GameObject tutorialText;
	[SerializeField] private TextMeshProUGUI scoreText;
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
		triggerAtNote = totalNotes - playAudioTriggerBeatCount;
	}

	public void HitNote()
	{
		totalNotes--;
		
		points+=100;
		scoreText.text = points.ToString();

		if (totalNotes == triggerAtNote) { RhythmGameAudioManager.Instance.PlayTriggerSound(); }
		if (totalNotes == 0) { SceneManager.LoadScene("MainMenu"); }
	}

	public void StartGame()
	{
		beatScroller.StartBeatScroller();
		startButton.SetActive(false);
		tutorialText.SetActive(false);
		Conductor.Instance.StartTrack();
	}
}