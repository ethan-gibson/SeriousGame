using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace MazeGame
{
	//[BurstCompile]
	public struct GenerateMazeJob : IJob
	{
		public Maze Maze;
		public int Seed;

		public void Execute()
		{
			var _random = new Random((uint)Seed);
			var _scratchpad = new NativeArray<(int, MazeFlags, MazeFlags)>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			var _activeIndices = new NativeArray<int>(Maze.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int _firstActiveIndex = 0, _lastActiveIndex = 0;
			_activeIndices[_firstActiveIndex] = _random.NextInt(Maze.Length);

			while (_firstActiveIndex <= _lastActiveIndex)
			{
				int _index = _activeIndices[_lastActiveIndex];

				int _availablePassageCount = findAvailablePassages(_index, _scratchpad);
				if (_availablePassageCount <= 1)
				{
					_lastActiveIndex -= 1;
				}
				if (_availablePassageCount > 0)
				{
					(int, MazeFlags, MazeFlags) _passage = _scratchpad[_random.NextInt(0, _availablePassageCount)];
					Maze.Set(_index, _passage.Item2);
					Maze[_passage.Item1] = _passage.Item3;
					_activeIndices[++_lastActiveIndex] = _passage.Item1;
				}
			}
		}

		int findAvailablePassages (
			int _index, NativeArray<(int, MazeFlags, MazeFlags)> _scratchpad
		)
		{
			int2 _coordinates = Maze.IndexToCoordinates(_index);
			int _count = 0;
			if (_coordinates.x + 1 < Maze.SizeEW)
			{
				int _i = _index + Maze.StepE;
				if (Maze[_i] == MazeFlags.Empty)
				{
					_scratchpad[_count++] = (_i, MazeFlags.PassageE, MazeFlags.PassageW);
				}
			}
			if (_coordinates.x > 0)
			{
				int _i = _index + Maze.StepW;
				if (Maze[_i] == MazeFlags.Empty)
				{
					_scratchpad[_count++] = (_i, MazeFlags.PassageW, MazeFlags.PassageE);
				}
			}
			if (_coordinates.y + 1 < Maze.SizeNS)
			{
				int _i = _index + Maze.StepN;
				if (Maze[_i] == MazeFlags.Empty)
				{
					_scratchpad[_count++] = (_i, MazeFlags.PassageN, MazeFlags.PassageS);
				}
			}
			if (_coordinates.y > 0)
			{
				int _i = _index + Maze.StepS;
				if (Maze[_i] == MazeFlags.Empty)
				{
					_scratchpad[_count++] = (_i, MazeFlags.PassageS, MazeFlags.PassageN);
				}
			}
			return _count;
		}
	}
}