using Godot;
using System;
using static DirectionUtils;

[GlobalClass]
public partial class BuildingController : Node3D
{
	[Export] public Camera3D Camera;
	[Export] public PackedScene GhostScene;
	[Export] public float TileSize = 1.0f;

	private BuildingData _buildingData;
	private GhostBuilding _ghost;
	private Direction _direction = Direction.North;
	private Vector3I _currentGridPos;

	#region Debug
	[ExportGroup("Debug")]
	[Export] bool _debug = false;
	[Export] BuildingData debug_buildingData;
	#endregion

	public override void _Ready()
	{
		_ghost = GhostScene.Instantiate<GhostBuilding>();
		AddChild(_ghost);

		if (_debug)
		{
			StartBuildingPreview(debug_buildingData);
		}
	}

	public override void _Process(double delta)
	{
		UpdateGhost();
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("rotate_building"))
		{
			Rotate();
		}

		if (@event.IsActionPressed("place_building"))
		{
			TryPlaceBuilding();
		}
	}

	public void StartBuildingPreview(BuildingData buildingData)
	{
		_buildingData = buildingData;
		_ghost.SetMesh(buildingData.Mesh);
	}

	public void TryPlaceBuilding()
	{
		if (!FactoryGrid.Instance.IsPlacementValid(_currentGridPos))
			return;

		FactoryGrid.Instance.PlaceBuilding(_buildingData, _currentGridPos, _direction);
	}

	private void UpdateGhost()
	{
		var worldPos = GetMouseWorldPosition();
		_currentGridPos = FactoryGrid.Instance.WorldToGrid(worldPos);
		var snappedPos = FactoryGrid.Instance.GridToWorld(_currentGridPos);

		_ghost.GlobalPosition = snappedPos;

		_ghost.RotationDegrees = new Vector3(0, _direction.GetDirectionAngle(), 0);

		bool valid = FactoryGrid.Instance.IsPlacementValid(_currentGridPos);
		_ghost.SetValid(valid);
	}

	private void Rotate()
	{
		_direction = _direction.RotateRight();
		_ghost.RotationDegrees = new Vector3(0, _direction.GetDirectionAngle(), 0);
	}

	private Vector3 GetMouseWorldPosition()
	{
		var mousePos = GetViewport().GetMousePosition();

		var from = Camera.ProjectRayOrigin(mousePos);
		var to = from + Camera.ProjectRayNormal(mousePos) * 1000f;

		var space = GetWorld3D().DirectSpaceState;

		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = space.IntersectRay(query);

		if (result.Count > 0)
			return (Vector3)result["position"];

		return Vector3.Zero;
	}
}
