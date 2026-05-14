using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class EnemySpawningManager : Node
{
    [Export] float timeBetweenSpawns = 1f;
    [Export] float timeBetweenSpawnsVariance = 0.5f;

    Player player;


    Array<EnemySpawner> spawners = new Array<EnemySpawner>();

    Timer spawnTimer;

    public override void _Ready()
    {
        base._Ready();
        var nodesInSpawners = GetTree().GetNodesInGroup("spawners");
        player = GetTree().GetFirstNodeInGroup("player") as Player;

        foreach (var node in nodesInSpawners)
        {
            if (node is EnemySpawner spawner)
            {
                spawners.Add(spawner);
            }
        }

        spawnTimer = new Timer();
        AddChild(spawnTimer);
        spawnTimer.WaitTime = timeBetweenSpawns;

        spawnTimer.Timeout += SpawnEnemyInRandomSpawner;
        spawnTimer.Start();
    }

    public void SpawnEnemyInRandomSpawner()
    {
        var spawner = spawners[GD.RandRange(0, spawners.Count - 1)];
        var enemy = spawner.SpawnEnemy();

        enemy.SetTarget(player);
    }
}