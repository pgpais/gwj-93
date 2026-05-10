using Godot;
using Godot.Collections;
using static DirectionUtils;

[Tool]
[GlobalClass]
public partial class FactoryGrid : Node3D
{
	public static FactoryGrid Instance { get; private set; }

	[Export] public float TileSize { get; private set; } = 1.5f;

	[ExportGroup("Gizmo")]
	[Export] bool showGizmo = false;
	[Export] int gridSize = 20;
	[Export] Color GridColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

	private Dictionary<Vector3I, Building> grid = new();

	public override void _Ready()
	{
		if (!Engine.IsEditorHint())
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				QueueFree();
			}
		}
		else
		{
			if (showGizmo)
			{
				GenerateGrid();
			}
		}

	}

	public override void _Process(double delta)
	{
		HandleGizmo();
	}


	public bool IsOccupied(Vector3I pos)
	{
		return grid.ContainsKey(pos);
	}

	public Building GetBuilding(Vector3I pos)
	{
		if (!IsOccupied(pos)) return null;

		return grid[pos];
	}

	public bool IsPlacementValid(Vector3I pos)
	{
		//TODO: might need to make more checks eventually
		return !IsOccupied(pos);
	}

	public Building PlaceBuilding(BuildingData buildingData, Vector3I gridPos = default, Direction direction = default)
	{
		var building = buildingData.Scene.Instantiate<Building>();
		GetTree().Root.AddChild(building);

		building.GlobalPosition = GridToWorld(gridPos);
		building.RotationDegrees = new Vector3(0, direction.GetDirectionAngle(), 0);
		building.GridPosition = gridPos;
		building.direction = direction;

		building.OnPlaced();

		grid[gridPos] = building;

		GD.Print($"Placed {building.Name} at {gridPos}");

		return building;
	}

	public Building Get(Vector3I pos)
	{
		return grid.TryGetValue(pos, out var b) ? b : null;
	}

	public Vector3 GridToWorld(Vector3I gridPos)
	{
		return new Vector3(
			gridPos.X * TileSize,
			0,
			gridPos.Z * TileSize
		);
	}

	public Vector3I WorldToGrid(Vector3 worldPos)
	{
		return new Vector3I(
			Mathf.FloorToInt(worldPos.X / TileSize),
			0,
			Mathf.FloorToInt(worldPos.Z / TileSize)
		);
	}

	public Vector3 SnapToGrid(Vector3 worldPos, float cellSize)
	{
		return new Vector3(
			Mathf.Floor(worldPos.X / cellSize),
			0,
			Mathf.Floor(worldPos.Z / cellSize)
		);
	}



	private MeshInstance3D _meshInstance;
	private void HandleGizmo()
	{
		if (Engine.IsEditorHint())
		{
			// Optional: regenerate live if values change
			if (showGizmo)
			{
				if (_meshInstance == null)
					GenerateGrid();
			}
			else
			{
				if (_meshInstance != null)
				{
					RemoveChild(_meshInstance);
					_meshInstance.QueueFree();
					_meshInstance = null;
				}
			}
		}
	}
	private void GenerateGrid()
	{
		// Clean up old mesh
		if (_meshInstance != null)
		{
			RemoveChild(_meshInstance);
			_meshInstance.QueueFree();
		}

		_meshInstance = new MeshInstance3D();
		AddChild(_meshInstance);

		var mesh = new ImmediateMesh();
		var material = new StandardMaterial3D
		{
			ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
			AlbedoColor = GridColor,
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha
		};

		mesh.SurfaceBegin(Mesh.PrimitiveType.Lines, material);

		float half = gridSize * TileSize * 0.5f;

		// Draw lines along X
		for (int i = 0; i <= gridSize; i++)
		{
			float offset = -half + i * TileSize;

			mesh.SurfaceAddVertex(new Vector3(-half, 0, offset));
			mesh.SurfaceAddVertex(new Vector3(half, 0, offset));
		}

		// Draw lines along Z
		for (int i = 0; i <= gridSize; i++)
		{
			float offset = -half + i * TileSize;

			mesh.SurfaceAddVertex(new Vector3(offset, 0, -half));
			mesh.SurfaceAddVertex(new Vector3(offset, 0, half));
		}

		mesh.SurfaceEnd();

		_meshInstance.Mesh = mesh;
	}
}
