using Godot;

[GlobalClass]
public partial class EnemySpawner : Marker3D
{
	[Export] PackedScene _enemyScene;


	public override void _EnterTree()
	{
		base._EnterTree();
		AddToGroup("spawners");
	}

	public Enemy SpawnEnemy()
	{
		var enemy = _enemyScene.Instantiate<Enemy>();
		enemy.GlobalTransform = GlobalTransform;
		GetParent().AddChild(enemy);

		return enemy;
	}
}
