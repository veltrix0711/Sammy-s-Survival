namespace LowPolySurvival.Game.Core.Persistence
{
	public interface IIdentifiedSavable : ISavable
	{
		string GetSaveKey();
	}
}
