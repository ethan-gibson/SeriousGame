using System;
using MazeGame;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeManager : MonoBehaviour
{
	[SerializeField] private MazeVisualization visualization;
	[SerializeField] private int2 mazeSize = new int2(20, 20);

	[SerializeField, Tooltip("Use 0 for a random seed")]
	private int seed = 0;

	private Maze maze;

	private void Awake()
	{
		maze = new Maze(mazeSize);
		new GenerateMazeJob
		{
			Maze = maze,
			Seed = seed != 0 ? seed : Random.Range(1, int.MaxValue),
		}.Schedule().Complete();
		visualization.Visualize(maze);
	}
}