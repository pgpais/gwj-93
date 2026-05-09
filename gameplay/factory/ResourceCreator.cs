using Godot;
using System;
using System.Threading.Tasks;

public partial class ResourceCreator : Building
{
	[Export] GameResourceData data;
	[Export] ConveyorBelt belt;

	[ExportGroup("Spawning Settings")]
	[Export] float spawnRate = 0.5f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_ = SpawnResources();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async Task SpawnResources()
	{
		while (true)
		{
			await Task.Delay(TimeSpan.FromSeconds(spawnRate));
			while (belt.IsFull())
			{
				await ToSignal(belt, ConveyorBelt.SignalName.HasCapacity);
			}
			GameResource item = GameResource.Instantiate(data);
			AddChild(item);
			belt.ReceiveItem(item);
		}
	}
}
