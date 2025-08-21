namespace LowPolySurvival.Game.Core.Persistence
{
	public interface ISavable
	{
		string SaveToJson();
		void LoadFromJson(string json);
	}
}
