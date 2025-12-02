using System;
using System.Collections;
using MazeGame;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeManager : MonoBehaviour
{
	[SerializeField] private MazeVisualization visualization;
	[SerializeField] private int2 mazeSize = new (20, 20);

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
		spawnPickupInQuadrant(-19, -10, -19, -10);
		spawnPickupInQuadrant(10, 19, -19, -10);  
		spawnPickupInQuadrant(-19, -10, 10, 19);  
		spawnPickupInQuadrant(10, 19, 10, 19);    
	}

	private void spawnPickupInQuadrant(float _minX, float _maxX, float _minZ, float _maxZ)
	{
		const int _maxAttempts = 50;
    
		for (int _i = 0; _i < _maxAttempts; _i++)
		{
			float _x = Random.Range(_minX, _maxX);
			float _z = Random.Range(_minZ, _maxZ);
        
			Vector3 _checkPosition = new Vector3(_x, 0.2f, _z);
        
			if (Physics.Raycast(_checkPosition + Vector3.up * 50f, Vector3.down, 100f, floorLayerMask))
			{
				Instantiate(pickupPrefab, _checkPosition, Quaternion.identity);
				return;
			}
		}
    
		Debug.LogWarning($"Failed to find valid floor position in quadrant: X({_minX},{_maxX}), Z({_minZ},{_maxZ})");
	}

	public void Pickup()
	{
		++pickupCount;

		switch (pickupCount)
		{
			//thresholds
			//2 -> fog
			case 2:
			{
				if (Camera.main != null) { Camera.main.farClipPlane = 5; }
				break;
			}
			//3 -> regenerate
			case 3:
				var _mazeObjects = GameObject.FindGameObjectsWithTag("MazeObject");
				var _pointObjects = GameObject.FindGameObjectsWithTag("PickUp");
				foreach (var _variable in _mazeObjects)
				{
					_variable.SetActive(false);
				}
				foreach (var _variable in _pointObjects)
				{
					_variable.SetActive(false);
				}
				maze = new Maze(mazeSize);
				new GenerateMazeJob
				{
					Maze = maze,
					Seed = Random.Range(1, int.MaxValue),
					PickLastProbability = pickLastProbability,
					OpenDeadEndProbability = openRandomProbability,
				}.Schedule().Complete();
				visualization.Visualize(maze);
				spawnPickupInQuadrant(-19, 19, -19, 19);
				break;
			case 4:
				Debug.Log("Maze is complete");
				break;
		}
	}
}