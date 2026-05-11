using Godot;

public partial class ConveyorBelt : Building, IItemInput, IItemOutput
{
	[Export] ItemTransport itemTransport;

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		if (this.direction == direction)
		{
			return itemTransport;
		}
		return null;
	}

	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		if (this.direction == direction)
		{
			return itemTransport;
		}
		return null;
	}

	public override void OnPlaced()
	{
		base.OnPlaced();

		// Check for building connections
		var offset = direction.GetDirectionVector();
		var nextBuilding = FactoryGrid.Instance.GetBuilding(GridPosition + offset);
		GD.Print($"Next building at {GridPosition + direction.GetDirectionVector()}: {nextBuilding?.Name}");
		if (nextBuilding is IItemInput nextItemInput)
		{
			var port = nextItemInput.GetInputPort(this.direction, GridPosition + offset);
			itemTransport.ConnectTo(port);
		}

		var prevBuilding = FactoryGrid.Instance.GetBuilding(GridPosition - offset);
		GD.Print($"Prev building at {GridPosition - direction.GetDirectionVector()}: {prevBuilding?.Name}");
		if (prevBuilding is IItemOutput prevItemOutput)
		{
			var port = prevItemOutput.GetOutputPort(this.direction, GridPosition - offset);
			port.ConnectTo(itemTransport);
		}
	}
}
