using Godot;

public partial class ResourceExtractionLevel : Node3D
{

	[Export] float timeToExtract = 30f;
	[Export] PackedScene nextLevel;

	SceneTreeTimer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetTree().CreateTimer(timeToExtract);
		timer.Timeout += ChangeLevel;
	}

	public void ChangeLevel()
	{
		GetTree().ChangeSceneToPacked(nextLevel);
	}

	public double GetTimeToExtract()
	{
		if (timer == null) return 0;

		return timer.TimeLeft;
	}
}
