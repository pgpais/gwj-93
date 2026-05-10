using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemTransport : Node3D
{
	[Signal] public delegate void FullEventHandler();
	[Signal] public delegate void HasCapacityEventHandler();


	[Export] float beltSpeed = 2f;

	[Export] Node3D startPos;
	[Export] Node3D endPos;

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

	/// <summary>
	/// Connect this Transport to another
	/// </summary>
	/// <param name="nextPort">The next Transport</param>
	public void ConnectTo(ItemTransport nextPort)
	{
		this.nextPort = nextPort;
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
			GD.Print($"Belt {Name} tried to receive {item.Name} but was full");
			return;
		}

		if (!CanReceiveItem(item))
		{
			GD.Print($"Belt {Name} tried to receive {item.Name} but was filtered");
			return;
		}

		GD.Print($"Belt {Name} received {item.Name}");
		item.Reparent(this);
		currentItem = item;
		currentItemPos = 0;

		bool becameFull = IsFull();
		if (becameFull)
		{
			GD.Print($"Belt {Name} became full");
			EmitSignal(SignalName.Full);
		}
	}

	public GameResource GetItem()
	{
		//TODO: Only allow getting item if it reached the end
		var item = currentItem;
		ClearItem();
		return item;
	}

	public void ClearItem()
	{
		GD.Print($"Belt {Name} cleared");
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
		currentItem.Position = startPos.Position.Lerp(endPos.Position, currentItemPos);
	}

	private void SendItem()
	{
		if (!nextPort.IsFull())
		{
			nextPort.ReceiveItem(currentItem);
			ClearItem();
		}
	}
}
