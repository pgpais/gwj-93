using System;
using Godot;
using Godot.Collections;
using static DirectionUtils;

[GlobalClass]
public partial class ItemTransport : Node3D
{
	[Signal] public delegate void InputConnectedEventHandler();
	[Signal] public delegate void OutputConnectedEventHandler();
	[Signal] public delegate void FullEventHandler();
	[Signal] public delegate void HasCapacityEventHandler();
	[Signal] public delegate void ItemTansportedEventHandler(GameResource item);


	[Export] float beltSpeed = 2f;

	[Export] Path3D path;
	[Export] PathFollow3D followPath;

	[Export] ItemTransport nextPort;

	[ExportGroup("Debug")]
	[Export] bool Debug = false;
	[Export] GameResourceData debug_startingResource;

	GameResource currentItem;
	float currentItemPos;

	Array<GameResourceData> filteredItems = new();
	bool isFilterWhitelist = false;

	public override void _Ready()
	{
		if (Debug)
		{
			currentItemPos = 0;
			currentItem = GameResource.Instantiate(debug_startingResource);
			AddChild(currentItem);
		}
	}

	public override void _Process(double delta)
	{
		if (currentItem != null)
		{
			bool hasSpaceToMove = currentItemPos < 1;
			if (hasSpaceToMove)
			{
				MoveItem(delta);
			}
			else
			{
				bool hasNextPort = nextPort != null;
				if (hasNextPort)
				{
					SendItem();
				}
			}
		}
	}

	public void SetCurve(Curve3D curve)
	{
		path.Curve = curve;
	}

	/// <summary>
	/// Connect this Transport to another
	/// </summary>
	/// <param name="nextPort">The next Transport</param>
	public void ConnectTo(ItemTransport nextPort)
	{
		this.nextPort = nextPort;

		EmitSignal(SignalName.OutputConnected);
	}

	public void SetFilter(Array<GameResourceData> items, bool isWhitelist)
	{
		filteredItems = items;
		isFilterWhitelist = isWhitelist;
	}

	public bool CanReceiveItem(GameResource item)
	{
		if (isFilterWhitelist)
		{
			return filteredItems.Contains(item.Data);
		}
		else
		{
			return !filteredItems.Contains(item.Data);
		}
	}

	public void ReceiveItem(GameResource item)
	{
		if (IsFull())
		{
			GD.Print($"[{Owner.Name}] Belt {Name} tried to receive {item.Name} but was full");
			return;
		}

		if (!CanReceiveItem(item))
		{
			GD.Print($"[{Owner.Name}] Belt {Name} tried to receive {item.Name} but was filtered");
			return;
		}

		GD.Print($"[{Owner.Name}] Belt {Name} received {item.Name}");
		item.Reparent(this);
		currentItem = item;
		currentItemPos = 0;

		bool becameFull = IsFull();
		if (becameFull)
		{
			// GD.Print($"Belt {Name} became full");
			EmitSignal(SignalName.Full);
		}
	}

	public GameResource GetCurrentItem()
	{
		return currentItem;
	}

	public GameResource TakeItem()
	{
		//TODO: Only allow getting item if it reached the end
		var item = currentItem;
		ClearItem();
		return item;
	}

	public void ClearItem()
	{
		GD.Print($"[{Owner.Name}] Belt {Name} cleared");
		currentItem = null;
		currentItemPos = 0;

		EmitSignal(SignalName.HasCapacity);
	}

	public bool HasItem()
	{
		return currentItem != null;
	}

	public bool IsFull()
	{
		//TODO: A belt may have more capacity than just one item
		return HasItem();
	}

	private void MoveItem(double delta)
	{
		currentItemPos += beltSpeed * (float)delta;

		followPath.ProgressRatio = Mathf.Min(1, currentItemPos);
		currentItem.GlobalPosition = followPath.GlobalPosition;
	}

	private void SendItem()
	{
		if (!nextPort.IsFull() && nextPort.CanReceiveItem(currentItem))
		{
			nextPort.ReceiveItem(currentItem);
			ClearItem();
		}
	}

	public void ConnectOutputToThis(ItemTransport otherOutputPort)
	{
		otherOutputPort.ConnectTo(this);
		EmitSignal(SignalName.InputConnected);
	}

	public void ConnectToNeighboursOutput(Direction neighbourDirection)
	{
		var neighbourPort = GetNeighbourOutput(neighbourDirection);
		if (neighbourPort != null)
		{
			neighbourPort.ConnectTo(this);
		}
	}

	public void ConnectToNeighboursInput(Direction neighbourDirection)
	{
		var neighbourPort = GetNeighbourInput(neighbourDirection);
		if (neighbourPort != null)
		{
			this.ConnectTo(neighbourPort);
		}
	}

	private ItemTransport GetNeighbourInput(Direction direction)
	{
		var GridPosition = FactoryGrid.Instance.WorldToGrid(GlobalPosition);
		var offset = direction.GetDirectionVector();
		var nextBuilding = FactoryGrid.Instance.GetBuilding(GridPosition + offset);
		GD.Print($"Next building at {GridPosition + direction.GetDirectionVector()}: {nextBuilding?.Name}");
		if (nextBuilding is IItemInput itemInput)
		{
			var otherInputPort = itemInput.GetInputPort(direction, GridPosition + offset);
			if (otherInputPort != null)
			{
				return otherInputPort;
			}
		}
		return null;
	}

	private ItemTransport GetNeighbourOutput(Direction direction)
	{
		var GridPosition = FactoryGrid.Instance.WorldToGrid(GlobalPosition);
		var offset = direction.GetDirectionVector();
		var nextBuilding = FactoryGrid.Instance.GetBuilding(GridPosition + offset);
		GD.Print($"{direction} Next building at {GridPosition + direction.GetDirectionVector()}: {nextBuilding?.Name}");
		if (nextBuilding is IItemOutput itemOutput)
		{
			var otherOutputPort = itemOutput.GetOutputPort(direction.RotateRight().RotateRight(), GridPosition + offset);
			if (otherOutputPort != null)
			{
				return otherOutputPort;
			}
		}
		return null;
	}
}
