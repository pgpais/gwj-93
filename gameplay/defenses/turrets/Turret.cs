using Godot;
using System;

public partial class Turret : Building
{
	[Export] PackedScene projectile;
	[Export] Node3D shootingPoint;
	[Export] float fireRate = 0.5f;
	[Export] float projectileSpeed = 10f;
	[Export] Node3D Anchor;
	[Export] Timer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer.WaitTime = fireRate;
		timer.Timeout += Shoot;
	}

	public void Shoot()
	{
		var projectileInstance = projectile.Instantiate<Projectile>();
		GetTree().CurrentScene.AddChild(projectileInstance);

		projectileInstance.GlobalPosition = shootingPoint.GlobalPosition;
		projectileInstance.GlobalRotation = shootingPoint.GlobalRotation;
		projectileInstance.LinearVelocity = -shootingPoint.GlobalTransform.Basis.Z * projectileSpeed;
	}
}
