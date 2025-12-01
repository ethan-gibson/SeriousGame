using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace MazeGame
{
	public struct Maze
	{
		[NativeDisableParallelForRestriction]
		private NativeArray<MazeFlags> cells;
		
		private int2 size;
		
		public int Length => size.x * size.y;

		public Maze(int2 _size)
		{
			size = _size;
			cells = new NativeArray<MazeFlags>(size.x * size.y, Allocator.Persistent);
		}

		public int2 IndexToCoordinates(int _index)
		{
			int2 _coords;
			_coords.y = _index / size.x;
			_coords.x = _index % size.x;
			return _coords;
		}

		public Vector3 CoordinatesToWorldPos(int2 _coords, float _y = 0f) =>
			new Vector3(2f * _coords.x + 1 - size.x, _y, 2f * _coords.y + 1 - size.y);
		
		public Vector3 IndexToWorldPosition (int _index, float _y = 0f) =>
			CoordinatesToWorldPos(IndexToCoordinates(_index), _y);

		public MazeFlags this[int _index]
		{
			get => cells[_index];
			set => cells[_index] = value;
		}

		public MazeFlags Set(int _index, MazeFlags _mask) =>
			cells[_index] = cells[_index].With(_mask);

		public MazeFlags Unset(int _index, MazeFlags _mask) =>
			cells[_index] = cells[_index].Without(_mask);
		
		public int SizeEW => size.x;

		public int SizeNS => size.y;

		public int StepN => size.x;

		public int StepE => 1;

		public int StepS => -size.x;

		public int StepW => -1;

		public void OnDestroy()
		{
			cells.Dispose();
		}
		
		public void Dispose()
		{
			if (cells.IsCreated)
			{
				cells.Dispose();
			}
		}
	}
}