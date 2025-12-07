using UnityEngine;

public class BeatScroller : MonoBehaviour
{
	private float tempo;
	private bool hasStarted;

	private void Start()
	{
		tempo = Conductor.Instance.GetSongBpm() / 5;
	}

	private void Update()
	{
		if (hasStarted) { transform.position -= new Vector3(tempo * Time.deltaTime, 0f, 0f); }
	}

	public void StartBeatScroller()
	{
		Debug.Log("StartBeatScroller");
		hasStarted = true;
	}
}