using Godot;
using Godot.Collections;

public partial class Splitter : Building, IItemInput, IItemOutput
{
	[Export] ItemTransport input;
	[Export] Array<ItemTransport> outputs;
	[Export] SlotInventory inventory;

	int currentOutput = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		inventory = new SlotInventory(1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PullItemsFromInputs();

		PushItemsToOutput();
	}

	private void PullItemsFromInputs()
	{
		if (input.HasItem() && inventory.HasCapacity(input.GetCurrentItem().Data))
		{
			ReceiveItem(input.TakeItem());
		}
	}

	private void PushItemsToOutput()
	{
		var item = inventory.GetSlot(0).item;
		if (item == null) return;

		for (int i = 0; i < outputs.Count; i++)
		{
			var output = outputs[currentOutput];
			currentOutput = (currentOutput + 1) % outputs.Count;

			if (output.IsFull()) continue;

			var itemInstance = GameResource.Instantiate(item);
			AddChild(itemInstance);
			output.ReceiveItem(itemInstance);

			inventory.TryRemoveItem(item, 1);
			break;
		}

	}

	private void ReceiveItem(GameResource gameResource)
	{
		GD.Print($"[{Name}] Received {gameResource.Name}");
		inventory.TryAddItem(gameResource.Data);
		gameResource.QueueFree();
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		if (direction == this.direction)
		{
			return input;
		}

		return null;
	}

	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		foreach (var outputPort in outputs)
		{
			var transportGridPos = FactoryGrid.Instance.WorldToGrid(outputPort.GlobalPosition);

			if (transportGridPos != gridPos) continue;
			if (this.direction != direction) continue;

			return outputPort;
		}
		return null;
	}
}
