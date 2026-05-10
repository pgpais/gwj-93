using Godot;
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

	bool isBuilding;

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
		if (isBuilding)
		{
			UpdateGhost();
			var nextBuilding = FactoryGrid.Instance.GetBuilding(_currentGridPos + _direction.GetDirectionVector());
			if (nextBuilding != null)
			{
				// GD.Print($"Next building: {nextBuilding.Name}");
				//TODO: highlight next building
			}

			var prevBuilding = FactoryGrid.Instance.GetBuilding(_currentGridPos - _direction.GetDirectionVector());
			if (prevBuilding != null)
			{
				// GD.Print($"Previous building: {prevBuilding.Name}");
				//TODO: highlight previous building
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (isBuilding)
		{
			if (@event.IsActionPressed("rotate_building"))
			{
				Rotate();
			}
		}

		if (@event.IsActionPressed("ui_cancel"))
		{
			CancelBuilding();
		}
	}

	private void CancelBuilding()
	{
		_ghost.Hide();
		isBuilding = false;
	}

	public void StartBuildingPreview(BuildingData buildingData)
	{
		_buildingData = buildingData;
		_ghost.SetMesh(buildingData.Mesh);
		isBuilding = true;
		_ghost.Show();
	}

	public void TryPlaceBuilding()
	{
		if (!FactoryGrid.Instance.IsPlacementValid(_currentGridPos, _direction, _buildingData.Footprint))
			return;

		FactoryGrid.Instance.PlaceBuilding(_buildingData, _currentGridPos, _direction, _buildingData.Footprint);
	}

	private void UpdateGhost()
	{
		var snappedPos = FactoryGrid.Instance.GridToWorld(_currentGridPos);

		_ghost.GlobalPosition = snappedPos;

		_ghost.RotationDegrees = new Vector3(0, _direction.GetDirectionAngle(), 0);

		bool valid = FactoryGrid.Instance.IsPlacementValid(_currentGridPos, _direction, _buildingData.Footprint);
		_ghost.SetValid(valid);
	}

	private void Rotate()
	{
		_direction = _direction.RotateRight();
		_ghost.RotationDegrees = new Vector3(0, _direction.GetDirectionAngle(), 0);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if (isBuilding)
		{
			var worldPos = GetMouseWorldPosition();
			_currentGridPos = FactoryGrid.Instance.WorldToGrid(worldPos);

			if (@event.IsActionPressed("place_building"))
			{
				TryPlaceBuilding();
			}
		}
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
