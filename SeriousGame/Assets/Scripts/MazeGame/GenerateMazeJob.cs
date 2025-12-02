using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace MazeGame
{
	[BurstCompile]
	public struct GenerateMazeJob : IJob
	{
		public Maze Maze;
		public int Seed;
		public float PickLastProbability, OpenDeadEndProbability;

		public void Execute()
		{
			Debug.Log("Execute");
			var _random = new Random((uint)Seed);
			var _scratchpad = new NativeArray<(int, MazeFlags, MazeFlags)>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

			var _activeIndices = new NativeArray<int>(Maze.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
			int _firstActiveIndex = 0, _lastActiveIndex = 0;
			_activeIndices[_firstActiveIndex] = _random.NextInt(Maze.Length);

			while (_firstActiveIndex <= _lastActiveIndex)
			{
				int _randomActiveIndex, _index;
				bool _pickLast = _random.NextFloat() < PickLastProbability;
				if (_pickLast)
				{
					_randomActiveIndex = 0;
					_index = _activeIndices[_lastActiveIndex];
				}
				else
				{
					_randomActiveIndex = _random.NextInt(_firstActiveIndex, _lastActiveIndex + 1);
					_index = _activeIndices[_randomActiveIndex];
				}

				int _availablePassageCount = findAvailablePassages(_index, _scratchpad);
				if (_availablePassageCount <= 1)
				{
					if (_pickLast) { _lastActiveIndex -= 1; }
					else { _activeIndices[_randomActiveIndex] = _activeIndices[_firstActiveIndex++]; }
				}
				if (_availablePassageCount > 0)
				{
					(int, MazeFlags, MazeFlags) _passage = _scratchpad[_random.NextInt(0, _availablePassageCount)];
					Maze.Set(_index, _passage.Item2);
					Maze[_passage.Item1] = _passage.Item3;
					_activeIndices[++_lastActiveIndex] = _passage.Item1;
				}
			}
			if (OpenDeadEndProbability > 0f)
			{
				openDeadEnds(_random, _scratchpad);
			}
		}

		private int findAvailablePassages(int _index, NativeArray<(int, MazeFlags, MazeFlags)> _scratchpad)
		{
			int2 _coordinates = Maze.IndexToCoordinates(_index);
			int _count = 0;
			if (_coordinates.x + 1 < Maze.SizeEW)
			{
				int _i = _index + Maze.StepE;
				if (Maze[_i] == MazeFlags.Empty) { _scratchpad[_count++] = (_i, MazeFlags.PassageE, MazeFlags.PassageW); }
			}
			if (_coordinates.x > 0)
			{
				int _i = _index + Maze.StepW;
				if (Maze[_i] == MazeFlags.Empty) { _scratchpad[_count++] = (_i, MazeFlags.PassageW, MazeFlags.PassageE); }
			}
			if (_coordinates.y + 1 < Maze.SizeNS)
			{
				int _i = _index + Maze.StepN;
				if (Maze[_i] == MazeFlags.Empty) { _scratchpad[_count++] = (_i, MazeFlags.PassageN, MazeFlags.PassageS); }
			}
			if (_coordinates.y > 0)
			{
				int _i = _index + Maze.StepS;
				if (Maze[_i] == MazeFlags.Empty) { _scratchpad[_count++] = (_i, MazeFlags.PassageS, MazeFlags.PassageN); }
			}
			return _count;
		}

		private int findClosedPassages(int _index, NativeArray<(int, MazeFlags, MazeFlags)> _scratchpad, MazeFlags _excludeFlags)
		{
			int2 _coordinates = Maze.IndexToCoordinates(_index);
			int _count = 0;

			if (_excludeFlags != MazeFlags.PassageE && _coordinates.x + 1 < Maze.SizeEW) { _scratchpad[_count++] = (Maze.StepE, MazeFlags.PassageE, MazeFlags.PassageW); }
			if (_excludeFlags != MazeFlags.PassageW && _coordinates.x > 0) { _scratchpad[_count++] = (Maze.StepW, MazeFlags.PassageW, MazeFlags.PassageE); }
			if (_excludeFlags != MazeFlags.PassageN && _coordinates.y + 1 < Maze.SizeNS) { _scratchpad[_count++] = (Maze.StepN, MazeFlags.PassageN, MazeFlags.PassageS); }
			if (_excludeFlags != MazeFlags.PassageS && _coordinates.y > 0) { _scratchpad[_count++] = (Maze.StepS, MazeFlags.PassageS, MazeFlags.PassageN); }
			return _count;
		}

		private Random openDeadEnds(Random _random, NativeArray<(int, MazeFlags, MazeFlags)> _scratchpad)
		{
			for (int _i = 0; _i < Maze.Length; _i++)
			{
				MazeFlags _flags = Maze[_i];
				if (_flags.HasExactlyOne() && _random.NextFloat() < OpenDeadEndProbability)
				{
					int _availablePassageCount = findClosedPassages(_i, _scratchpad, _flags);
					(int, MazeFlags, MazeFlags) _passage = _scratchpad[_random.NextInt(0, _availablePassageCount)];
					Maze[_i] = _flags.With(_passage.Item2);
					Maze.Set(_i + _passage.Item1, _passage.Item3);
				}
			}
			return _random;
		}
	}
}