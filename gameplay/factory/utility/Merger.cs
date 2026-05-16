using Godot;
using Godot.Collections;

public partial class Merger : Building, IItemInput, IItemOutput
{
	[Export] Array<ItemTransport> inputs;
	[Export] ItemTransport output;
	[Export] SlotInventory inventory;

	int currentInput = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		inventory = new SlotInventory(1, 1);
	}

	public override void OnPlaced()
	{
		base.OnPlaced();

		foreach (var input in inputs)
		{
			input.ConnectToNeighboursOutput(direction.RotateRight().RotateRight());
		}

		output.ConnectToNeighboursInput(direction);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PullItemsFromInputs();

		PushItemsToOutput();
	}

	private void PullItemsFromInputs()
	{
		for (int i = 0; i < inputs.Count; i++)
		{
			var input = inputs[currentInput];
			currentInput = (currentInput + 1) % inputs.Count;
			if (!input.HasItem()) continue;
			if (!inventory.HasCapacity(input.GetCurrentItem().Data)) continue;

			ReceiveItem(input.TakeItem());
		}
	}

	private void PushItemsToOutput()
	{
		var item = inventory.GetSlot(0).item;
		if (item == null) return;

		if (output.IsFull()) return;

		var itemInstance = GameResource.Instantiate(item);
		if (!output.CanReceiveItem(itemInstance))
		{
			itemInstance.QueueFree();
			return;
		}

		AddChild(itemInstance);
		output.ReceiveItem(itemInstance);

		inventory.TryRemoveItem(item, 1);
	}

	private void ReceiveItem(GameResource gameResource)
	{
		GD.Print($"[{Name}] Received {gameResource.Name}");
		inventory.TryAddItem(gameResource.Data);
		gameResource.QueueFree();
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		foreach (var inputPort in inputs)
		{
			var transportGridPos = FactoryGrid.Instance.WorldToGrid(inputPort.GlobalPosition);

			if (transportGridPos != gridPos) continue;
			if (this.direction != direction) continue;

			return inputPort;
		}
		return null;
	}

	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		if (this.direction != direction) return null;

		return output;
	}
}
