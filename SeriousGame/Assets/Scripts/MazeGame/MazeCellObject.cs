using System.Collections.Generic;
using UnityEngine;

namespace MazeGame
{
	public class MazeCellObject : MonoBehaviour
	{
#if UNITY_EDITOR
		static List<Stack<MazeCellObject>> pools;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void clearPools()
		{
			if (pools == null) { pools = new(); }
			else
			{
				for (int _i = 0; _i < pools.Count; _i++) { pools[_i].Clear(); }
			}
		}
#endif

		[System.NonSerialized] Stack<MazeCellObject> pool;

		public MazeCellObject GetInstance()
		{
			if (pool == null)
			{
				pool = new();
#if UNITY_EDITOR
				pools.Add(pool);
#endif
			}
			if (pool.TryPop(out MazeCellObject _instance))
			{
				_instance.gameObject.SetActive(true);
			}
			else
			{
				_instance = Instantiate(this);
				_instance.pool = pool;
			}
			return _instance;
		}

		public void Recycle()
		{
			pool.Push(this);
			gameObject.SetActive(false);
		}
	}
}