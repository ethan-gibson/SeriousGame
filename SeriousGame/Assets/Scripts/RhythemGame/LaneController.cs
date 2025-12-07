using UnityEngine;
using UnityEngine.InputSystem;

public class LaneController : MonoBehaviour
{
	[SerializeField] private InputActionReference laneInputAction; // Assign in Inspector

	private NoteObject currentNoteInZone;

	private void OnEnable()
	{
		if (laneInputAction != null) { laneInputAction.action.performed += OnInputPerformed; }
	}

	private void OnDisable()
	{
		if (laneInputAction != null) { laneInputAction.action.performed -= OnInputPerformed; }
	}

	private void OnInputPerformed(InputAction.CallbackContext _context)
	{
		attemptHit();
	}

	private void attemptHit()
	{
		if (currentNoteInZone == null || !currentNoteInZone.CanBeHit()) { return; }
		currentNoteInZone.NoteHit();
		currentNoteInZone = null;
	}

	public void RegisterNote(NoteObject _note)
	{
		Debug.Log("Registering note: " + _note);
		currentNoteInZone = _note;
	}

	public void DeregisterNote(NoteObject _note)
	{
		Debug.Log("Deregistering note: " + _note);
		if (currentNoteInZone == _note) { currentNoteInZone = null; }
	}
}