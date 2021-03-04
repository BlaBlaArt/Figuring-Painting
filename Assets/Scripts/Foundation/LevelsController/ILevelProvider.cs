public interface ILevelProvider
{
	void LoadLevel(int number);
	void Reload();
	int LevelsCount { get; }
}