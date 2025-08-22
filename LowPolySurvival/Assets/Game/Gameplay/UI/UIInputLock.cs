namespace LowPolySurvival.Game.Gameplay.UI
{
	public static class UIInputLock
	{
		private static int lockCount;
		public static bool IsLocked => lockCount > 0;

		public static void Push()
		{
			lockCount++;
		}

		public static void Pop()
		{
			if (lockCount > 0) lockCount--;
		}
	}
}


