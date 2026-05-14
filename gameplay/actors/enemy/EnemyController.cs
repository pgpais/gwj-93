using Godot;

public partial class EnemyController : Node
{
	[Export] AStarGrid aStarGrid;
	[Export] Player player;
	[Export] Node3D pathVisualizer;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		CallDeferred(MethodName.ControlEnemies);
	}

	private void ControlEnemies()
	{
		//TODO: target should be base "core"
		//TODO: If enemies have player nearby, chase player
		foreach (var enemy in Enemy.enemies)
		{
			var path = aStarGrid.GetPathToPosition(enemy.GlobalPosition, player.GlobalPosition);

			if (path.Length < 2) continue;

			var dir = new Vector2(enemy.GlobalPosition.X, enemy.GlobalPosition.Z).DirectionTo(path[1]);
			enemy.MoveTowards(dir);
			pathVisualizer.GlobalPosition = new Vector3(path[1].X, enemy.GlobalPosition.Y, path[1].Y);
		}
	}
}
