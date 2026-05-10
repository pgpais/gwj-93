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
		var nextBuilding = FactoryGrid.Instance.GetBuilding(GridPosition + direction.GetDirectionVector());
		GD.Print($"Next building at {GridPosition + direction.GetDirectionVector()}: {nextBuilding}");
		if (nextBuilding is IItemInput nextItemInput)
		{
			var port = nextItemInput.GetInputPort(this.direction, GridPosition);
			itemTransport.ConnectTo(port);
		}

		var prevBuilding = FactoryGrid.Instance.GetBuilding(GridPosition - direction.GetDirectionVector());
		GD.Print($"Prev building at {GridPosition - direction.GetDirectionVector()}: {prevBuilding}");
		if (prevBuilding is IItemOutput prevItemOutput)
		{
			var port = prevItemOutput.GetOutputPort(this.direction, GridPosition);
			port.ConnectTo(itemTransport);
		}
	}
}
