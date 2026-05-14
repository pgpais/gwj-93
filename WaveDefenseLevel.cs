using Godot;

public partial class WaveDefenseLevel : Node3D
{
	public float PreparationTime => currentPreparationTime;
	[Export] float startPreparationTime = 300f;

	float currentPreparationTime = 0f;

	[Export] EnemySpawningManager enemySpawningManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentPreparationTime = startPreparationTime;
		enemySpawningManager.StopSpawning();
		GetTree().CreateTimer(startPreparationTime).Timeout += () => enemySpawningManager.StartSpawning();
	}

	public override void _Process(double delta)
	{
		currentPreparationTime -= (float)delta;
	}
}
