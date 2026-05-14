using Godot;
using System;

public partial class Turret : Node3D
{
	[Export] PackedScene projectile;
	[Export] Node3D shootingPoint;
	[Export] float fireRate = 0.5f;
	[Export] float projectileSpeed = 10f;
	[Export] Node3D Anchor;

	[Export] Targeting targeting;

	[Export] Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer.WaitTime = fireRate;
		timer.Timeout += Shoot;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (targeting.CurrentTarget != null)
		{
			Anchor.LookAt(targeting.CurrentTarget.GlobalPosition, Vector3.Up);
			Anchor.RotationDegrees = new Vector3(0, Anchor.RotationDegrees.Y, 0);
		}
	}

	public void Shoot()
	{
		if (targeting.CurrentTarget == null) return;

		var projectileInstance = projectile.Instantiate<Projectile>();
		GetTree().Root.AddChild(projectileInstance);

		projectileInstance.GlobalPosition = shootingPoint.GlobalPosition;
		projectileInstance.GlobalRotation = shootingPoint.GlobalRotation;
		projectileInstance.LinearVelocity = -shootingPoint.GlobalTransform.Basis.Z * projectileSpeed;
	}
}
