using Godot;
using Godot.Collections;

public partial class ResourceExtractionLevel : Node3D
{

	[Export] float timeToExtract = 30f;
	[Export] PackedScene nextLevel;
	[Export] Node3D storageSpawnPoint;
	[Export] BuildingData storageBuildingData;
	[Export] Array<Node3D> resourceCreatorSpawnPoints;
	[Export] BuildingData resourceCreatorBuildingData;
	[Export] Array<GameResourceData> startingResourdces;

	SceneTreeTimer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetTree().CreateTimer(timeToExtract);
		timer.Timeout += ChangeLevel;

		PlaceStartingBuildings();
	}

	private void PlaceStartingBuildings()
	{
		FactoryGrid.Instance.PlaceBuilding(storageBuildingData, (Vector3I)storageSpawnPoint.GlobalPosition);

		for (int i = 0; i < resourceCreatorSpawnPoints.Count; i++)
		{
			var resourceCreator = FactoryGrid.Instance.PlaceBuilding(resourceCreatorBuildingData, (Vector3I)resourceCreatorSpawnPoints[i].GlobalPosition) as ResourceCreator;
			resourceCreator.SetResource(startingResourdces[i % startingResourdces.Count]);
		}
	}

	public void ChangeLevel()
	{
		GetTree().ChangeSceneToPacked(nextLevel);
	}

	public double GetTimeToExtract()
	{
		if (timer == null) return 0;

		return timer.TimeLeft;
	}
}
