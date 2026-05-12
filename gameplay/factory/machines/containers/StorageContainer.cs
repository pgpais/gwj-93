using Godot;

public partial class StorageContainer : Building, IItemInput
{
	[Export] ItemTransport input;

	[Export] StorageInventory storage;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (storage == null)
		{
			storage = new StorageInventory();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PullItemsFromInputs();
		GD.Print($"[{Name}] Storage: {storage.ToString()}");
	}

	private void PullItemsFromInputs()
	{
		if (input.HasItem() && storage.HasCapacity(input.GetCurrentItem().Data))
		{
			ReceiveItem(input.TakeItem());
		}
	}

	private void ReceiveItem(GameResource gameResource)
	{
		GD.Print($"[{Name}] Received {gameResource.Name}");
		storage.TryAddItem(gameResource.Data);
		gameResource.QueueFree();
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		if (this.direction != direction) return null;

		var inputPortGridPos = FactoryGrid.Instance.WorldToGrid(input.GlobalPosition);
		if (gridPos != inputPortGridPos) return null;

		return input;
	}
}
