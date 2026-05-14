using Godot;
using System;

public partial class Healthbar3D : Node3D
{
	[Export] HealthComponent health;
	[Export] ProgressBar bar;

	Camera3D cam;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cam = GetViewport().GetCamera3D();
		bar.Position = cam.UnprojectPosition(GlobalPosition);

		bar.MaxValue = health.MaxHealth;
		bar.Value = health.CurrentHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CallDeferred(MethodName.PositionHealthBar);
	}

	private void PositionHealthBar()
	{
		var unprojectedPos = cam.UnprojectPosition(GlobalPosition);
		var targetPos = unprojectedPos;
		bar.Position = targetPos;

		bar.Value = health.CurrentHealth;
	}
}
