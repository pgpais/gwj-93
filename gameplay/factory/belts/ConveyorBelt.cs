using System;
using Godot;
using Godot.Collections;

public partial class ConveyorBelt : Building, IItemInput, IItemOutput
{
	[Export] ItemTransport itemTransport;
	[ExportGroup("Belt Arrangements")]
	[Export] Node3D straightBelt;
	[Export] Curve3D straightItemTransportCurve;
	[Export] Node3D curveRightBelt;
	[Export] Curve3D curveRightItemTransportCurve;
	[Export] Node3D curveLeftBelt;
	[Export] Curve3D curveLeftItemTransportCurve;


	Arrangement currentArrangement = Arrangement.Straight;

	public override void _Ready()
	{
		base._Ready();

		itemTransport.InputConnected += () => CheckPlacement();
		itemTransport.OutputConnected += () => CheckPlacement();
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return itemTransport;
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

		var neighbours = CheckPlacement();

		if (neighbours.FrontExists()) itemTransport.ConnectTo(neighbours.front);
		if (neighbours.LeftExists()) neighbours.left.ConnectTo(itemTransport);
		if (neighbours.RightExists()) neighbours.right.ConnectTo(itemTransport);
		if (neighbours.BehindExists()) neighbours.behind.ConnectTo(itemTransport);
	}

	private Neighbours CheckPlacement()
	{
		var neighbours = HandleNeighbours();

		PickArrangement(neighbours);

		return neighbours;
	}

	private Neighbours HandleNeighbours()
	{
		var currentDirection = direction;

		//Output
		var front = GetNeighbourInput(currentDirection);

		//Input
		var left = GetNeighbourOutput(currentDirection.RotateLeft());
		var right = GetNeighbourOutput(currentDirection.RotateRight());
		var behind = GetNeighbourOutput(currentDirection.RotateRight().RotateRight());


		return new Neighbours()
		{
			front = front,
			left = left,
			right = right,
			behind = behind
		};
	}

	private ItemTransport GetNeighbourInput(DirectionUtils.Direction direction)
	{
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

	private ItemTransport GetNeighbourOutput(DirectionUtils.Direction direction)
	{
		var offset = direction.GetDirectionVector();
		var nextBuilding = FactoryGrid.Instance.GetBuilding(GridPosition + offset);
		GD.Print($"{direction} Next building at {GridPosition + direction.GetDirectionVector()}: {nextBuilding?.Name}");

		if (nextBuilding == null) return null;
		if (nextBuilding.direction != direction.RotateRight().RotateRight()) return null;

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

	private void PickArrangement(Neighbours neighbours)
	{
		//TODO: Receive items from old active transport
		if (neighbours.LeftExists() && !neighbours.RightExists() && !neighbours.BehindExists())
		{
			currentArrangement = Arrangement.CurveLeft;

			curveLeftBelt.Visible = true;
			// itemTransport.RotationDegrees = new Vector3(0, 90, 0);
			straightBelt.Visible = false;
			curveRightBelt.Visible = false;

			itemTransport.SetCurve(curveLeftItemTransportCurve);
		}
		else if (neighbours.RightExists() && !neighbours.LeftExists() && !neighbours.BehindExists())
		{
			currentArrangement = Arrangement.CurveRight;

			curveLeftBelt.Visible = false;
			straightBelt.Visible = false;
			curveRightBelt.Visible = true;
			// itemTransport.RotationDegrees = new Vector3(0, -90, 0);

			itemTransport.SetCurve(curveRightItemTransportCurve);
		}
		else
		{
			currentArrangement = Arrangement.Straight;

			curveLeftBelt.Visible = false;
			straightBelt.Visible = true;
			// itemTransport.RotationDegrees = new Vector3(0, 0, 0);
			curveRightBelt.Visible = false;

			itemTransport.SetCurve(straightItemTransportCurve);
		}
	}

	private enum Arrangement
	{
		Straight,
		CurveRight,
		CurveLeft
	}

	public struct Neighbours
	{
		public ItemTransport front;
		public ItemTransport left;
		public ItemTransport right;
		public ItemTransport behind;

		public bool FrontExists() => front != null;
		public bool LeftExists() => left != null;
		public bool RightExists() => right != null;
		public bool BehindExists() => behind != null;
	}
}
