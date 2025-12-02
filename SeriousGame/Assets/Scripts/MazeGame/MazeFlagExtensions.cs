namespace MazeGame
{
	public static class MazeFlagExtensions
	{
		public static bool Has(this MazeFlags _flags, MazeFlags _mask) =>
			(_flags & _mask) == _mask;

		public static bool HasAny(this MazeFlags _flags, MazeFlags _mask) =>
			(_flags & _mask) != 0;

		public static bool HasNot(this MazeFlags _flags, MazeFlags _mask) =>
			(_flags & _mask) != _mask;

		public static bool HasExactlyOne(this MazeFlags _flags) =>
			_flags != 0 && (_flags & (_flags - 1)) == 0;

		public static MazeFlags With(this MazeFlags _flags, MazeFlags _mask) =>
			_flags | _mask;

		public static MazeFlags Without(this MazeFlags _flags, MazeFlags _mask) =>
			_flags & ~_mask;
	}
	
	[System.Flags]
	public enum MazeFlags
	{
		Empty = 0,
	
		PassageN = 0b0001,
		PassageE = 0b0010,
		PassageS = 0b0100,
		PassageW = 0b1000,

		PassageAll = 0b1111
	}
}