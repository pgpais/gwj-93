using Godot;
using System;
using System.Threading.Tasks;

public partial class ResourceCreator : Building, IItemOutput
{
	[Export] GameResourceData data;

	[Export] ItemTransport itemTransport;

	[ExportGroup("Spawning Settings")]
	[Export] float spawnRate = 0.5f;

	public void SetResource(GameResourceData itemData) => data = itemData;

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
			while (itemTransport.IsFull())
			{
				await ToSignal(itemTransport, ItemTransport.SignalName.HasCapacity);
			}
			CallDeferred(MethodName.SpawnResource);
		}
	}

	public void SpawnResource()
	{
		GameResource item = GameResource.Instantiate(data);
		AddChild(item);
		itemTransport.ReceiveItem(item);
	}

	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return itemTransport;
	}
}
