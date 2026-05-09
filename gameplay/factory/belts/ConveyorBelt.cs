using System.Threading.Tasks;
using Godot;

public partial class ConveyorBelt : Node3D
{
	[Signal] public delegate void FullEventHandler();
	[Signal] public delegate void HasCapacityEventHandler();

	[Export] float beltSpeed = 0.5f;

	[Export] Node3D startPos;
	[Export] Node3D endPos;

	[Export] ConveyorBelt nextBelt;

	[ExportGroup("Debug")]
	[Export] bool Debug = false;
	[Export] GameResourceData debug_startingResource;

	GameResource currentItem;
	float currentItemPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Debug)
		{
			currentItemPos = 0;
			currentItem = GameResource.Instantiate(debug_startingResource);
			AddChild(currentItem);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentItem != null)
		{
			if (currentItemPos < 1)
			{
				currentItemPos += beltSpeed * (float)delta;
				currentItem.Position = startPos.Position.Lerp(endPos.Position, currentItemPos);
			}
			else
			{
				if (nextBelt != null)
				{
					SendItem();
				}
			}
		}
	}

	private void SendItem()
	{
		if (!nextBelt.IsFull())
		{
			nextBelt.ReceiveItem(currentItem);
			ClearItem();
		}
	}

	public void ReceiveItem(GameResource item)
	{
		if (IsFull())
		{
			GD.Print($"Belt {Name} tried to receive {item.Name} but was full");
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
}
