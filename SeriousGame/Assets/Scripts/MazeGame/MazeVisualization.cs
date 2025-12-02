using UnityEngine;

namespace MazeGame
{
	[CreateAssetMenu]
	public class MazeVisualization : ScriptableObject
	{
		[SerializeField] private MazeCellObject end, straight, corner, tJunction, xJunction;
		
		private static Quaternion[] rotations =
		{
			Quaternion.identity,
			Quaternion.Euler(0, 90, 0),
			Quaternion.Euler(0, 180, 0),
			Quaternion.Euler(0, 270, 0),
		};

		public void Visualize(Maze _maze)
		{
			for (int _i = 0; _i < _maze.Length; _i++)
			{
				(MazeCellObject, int) _prefabWithRotation = getPrefab(_maze[_i]);
				MazeCellObject _instance = _prefabWithRotation.Item1.GetInstance();
				_instance.transform.SetPositionAndRotation(_maze.IndexToWorldPosition(_i), rotations[_prefabWithRotation.Item2]);
			}
		}

		private (MazeCellObject, int) getPrefab(MazeFlags _flags) => _flags switch
		{
			MazeFlags.PassageN => (end, 0),
			MazeFlags.PassageE => (end, 1),
			MazeFlags.PassageS => (end, 2),
			MazeFlags.PassageW => (end, 3),

			MazeFlags.PassageN | MazeFlags.PassageS => (straight, 0),
			MazeFlags.PassageE | MazeFlags.PassageW => (straight, 1),

			MazeFlags.PassageN | MazeFlags.PassageE => (corner, 0),
			MazeFlags.PassageE | MazeFlags.PassageS => (corner, 1),
			MazeFlags.PassageS | MazeFlags.PassageW => (corner, 2),
			MazeFlags.PassageW | MazeFlags.PassageN => (corner, 3),

			MazeFlags.PassageAll & ~MazeFlags.PassageW => (tJunction, 0),
			MazeFlags.PassageAll & ~MazeFlags.PassageN => (tJunction, 1),
			MazeFlags.PassageAll & ~MazeFlags.PassageE => (tJunction, 2),
			MazeFlags.PassageAll & ~MazeFlags.PassageS => (tJunction, 3),

			_ => (xJunction, 0)
		};


	}
}