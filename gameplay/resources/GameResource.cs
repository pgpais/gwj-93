using Godot;

public partial class GameResource : Node3D
{
	const string GAME_RESOURCE_SCENE_UID = "uid://c5ub8plrwk3ij";
	public string ResourceName => data.Name;

	[Export] GameResourceData data;

	[Export] MeshInstance3D _meshInstance;

	public void Initialize(GameResourceData data)
	{
		this.data = data;
		_meshInstance.Mesh = data.Mesh;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}


	public static GameResource Instantiate(GameResourceData data)
	{
		var scene = GD.Load<PackedScene>(GAME_RESOURCE_SCENE_UID);
		var resource = scene.Instantiate<GameResource>();
		resource.Initialize(data);
		return resource;
	}
}
