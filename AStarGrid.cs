using Godot;
using Godot.Collections;
using System;

public partial class AStarGrid : Node
{
	[Export] FactoryGrid factoryGrid;
	[Export] Rect2I region = new Rect2I(0, 0, 1000, 1000);
	AStarGrid2D aStarGrid;

	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		aStarGrid = new AStarGrid2D
		{
			Region = region,
			CellSize = new Vector2I(1, 1),
			DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never
		};
		aStarGrid.Update();

		player = GetTree().GetFirstNodeInGroup("player") as Player;

		factoryGrid.BuildingPlaced += OnBuildingPlaced;
		factoryGrid.BuildingRemoved += OnBuildingRemoved;
	}

	private void OnBuildingPlaced(Building building, Array<Vector3I> gridPositions)
	{
		foreach (Vector3I gridPos in gridPositions)
		{
			aStarGrid.SetPointSolid(new Vector2I((int)gridPos.X, (int)gridPos.Z), true);
		}
	}

	private void OnBuildingRemoved(Building building, Array<Vector3I> gridPositions)
	{
		foreach (Vector3I gridPos in gridPositions)
		{
			aStarGrid.SetPointSolid(new Vector2I((int)gridPos.X, (int)gridPos.Z), false);
		}
	}

	public Vector2[] GetPathToPosition(Vector3 from, Vector3 to)
	{
		return aStarGrid.GetPointPath(new Vector2I(Mathf.RoundToInt(from.X), Mathf.RoundToInt(from.Z)), new Vector2I(Mathf.RoundToInt(to.X), Mathf.RoundToInt(to.Z)));
	}
}
