using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MazeGame;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MazeManager : MonoBehaviour
{
	[SerializeField] private MazeVisualization visualization;
	[SerializeField] private int2 mazeSize = new(20, 20);

	[SerializeField, Tooltip("Use 0 for a random seed")]
	private int seed = 0;

	[SerializeField, Range(0, 1)] private float pickLastProbability = 0.5f;
	[SerializeField, Range(0, 1)] private float openRandomProbability = 0.5f;

	private Maze maze;

	public static MazeManager Instance { get; private set; }
	private int pickupCount;

	[SerializeField] private GameObject pickupPrefab;
	[SerializeField] private LayerMask floorLayerMask;

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		Application.targetFrameRate = 60;
		Instance = this;

		maze = new Maze(mazeSize);
		new GenerateMazeJob
		{
			Maze = maze,
			Seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
			PickLastProbability = pickLastProbability,
			OpenDeadEndProbability = openRandomProbability,
		}.Schedule().Complete();
		visualization.Visualize(maze);
		StartCoroutine(spawnPickupsDelayed());
	}

	private IEnumerator spawnPickupsDelayed()
	{
		yield return new WaitForFixedUpdate();

		spawnPickups();
	}

	private void spawnPickups()
	{
		List<Vector3> _floorCenters = getFloorTileCenters();
		List<Vector3> _quadrant1Positions = _floorCenters.Where(_p => _p is { x: < -10, z: < -10 }).ToList();
		List<Vector3> _quadrant2Positions = _floorCenters.Where(_p => _p is { x: >= 10, z: < -10 }).ToList();
		List<Vector3> _quadrant3Positions = _floorCenters.Where(_p => _p is { x: < -10, z: >= 10 }).ToList();
		List<Vector3> _quadrant4Positions = _floorCenters.Where(_p => _p is { x: >= 10, z: >= 10 }).ToList();

		// Spawn one pickup in each quadrant if available
		spawnInRandomPosition(_quadrant1Positions);
		spawnInRandomPosition(_quadrant2Positions);
		spawnInRandomPosition(_quadrant3Positions);
		spawnInRandomPosition(_quadrant4Positions);
	}

	private void spawnInRandomPosition(List<Vector3> _positions)
	{
		if (_positions == null || _positions.Count == 0)
		{
			Debug.LogWarning("No valid spawn positions in this quadrant!");
			return;
		}

		Vector3 _spawnPos = _positions[Random.Range(0, _positions.Count)];
		Instantiate(pickupPrefab, _spawnPos, Quaternion.identity);
	}

	private List<Vector3> getFloorTileCenters()
	{
		List<Vector3> _floorCenters = new List<Vector3>();

		for (int _x = 0; _x < mazeSize.x; _x++)
		{
			for (int _z = 0; _z < mazeSize.y; _z++)
			{
				float _worldX = (_x * 2) - mazeSize.x + 1;
				float _worldZ = (_z * 2) - mazeSize.y + 1;

				_floorCenters.Add(new Vector3(_worldX, 0.2f, _worldZ));
			}
		}

		return _floorCenters;
	}

	public void Pickup()
	{
		++pickupCount;

		switch (pickupCount)
		{
			//2 -> fog
			case 2:
			{
				if (Camera.main != null) { Camera.main.farClipPlane = 5; }
				MazeAudioManager.Instance.PlayFogTrigger();
				break;
			}
			//3 -> regenerate
			case 3:
				var _mazeObjects = GameObject.FindGameObjectsWithTag("MazeObject");
				var _pointObjects = GameObject.FindGameObjectsWithTag("PickUp");
				foreach (var _variable in _mazeObjects) { _variable.SetActive(false); }
				foreach (var _variable in _pointObjects) { _variable.SetActive(false); }
				maze = new Maze(mazeSize);
				new GenerateMazeJob
				{
					Maze = maze,
					Seed = Random.Range(1, int.MaxValue),
					PickLastProbability = pickLastProbability,
					OpenDeadEndProbability = openRandomProbability,
				}.Schedule().Complete();
				visualization.Visualize(maze);
				MazeAudioManager.Instance.PlayRebuildTrigger();
				break;
			//4 -> maze done
			case 4:
				SceneManager.LoadScene("MainMenu");
				break;
		}
	}
}