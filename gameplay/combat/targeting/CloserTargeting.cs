using Godot;
using System;

[GlobalClass]
public partial class CloserTargeting : Targeting
{
	[Export] Area3D targetingArea;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targetingArea.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node3D body)
	{
		ChangeTarget(GetClosestBody());
		EmitSignal(nameof(TargetChanged), CurrentTarget);
	}

	private Node3D GetClosestBody()
	{
		Node3D closestBody = null;
		float closestDistance = float.MaxValue;
		foreach (Node3D body in targetingArea.GetOverlappingBodies())
		{
			float distance = targetingArea.GlobalPosition.DistanceTo(body.GlobalPosition);
			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestBody = body;
			}
		}
		return closestBody;
	}
}
