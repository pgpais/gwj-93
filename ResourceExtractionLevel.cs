using Godot;
using Godot.Collections;

public partial class ResourceExtractionLevel : Node3D
{
	public int LevelNumber = 1;

	[Export] int maxLevels = 3;
	[Export] float timeToExtract = 30f;
	[Export] PackedScene extractionLevelScene;
	[Export] PackedScene lastLevel;
	[Export] Node3D storageSpawnPoint;
	[Export] BuildingData storageBuildingData;
	[Export] Array<Node3D> resourceCreatorSpawnPoints;
	[Export] BuildingData resourceCreatorBuildingData;
	[Export] Array<GameResourceData> startingResourdces;

	[ExportGroup("UI")]
	[Export] LevelEndMenu levelEndMenu;
	[Export] Array<Control> levelUIs;

	SceneTreeTimer timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = GetTree().CreateTimer(timeToExtract);
		timer.Timeout += ShowEndUI;

		levelEndMenu.ContinuePressed += ChangeLevel;

		PlaceStartingBuildings();
	}

	private void PlaceStartingBuildings()
	{
		FactoryGrid.Instance.PlaceBuilding(storageBuildingData, (Vector3I)storageSpawnPoint.GlobalPosition);

		for (int i = 0; i < resourceCreatorSpawnPoints.Count; i++)
		{
			var resourceCreator = FactoryGrid.Instance.PlaceBuilding(resourceCreatorBuildingData, (Vector3I)resourceCreatorSpawnPoints[i].GlobalPosition, DirectionUtils.Direction.South) as ResourceCreator;
			resourceCreator.SetResource(startingResourdces[i % startingResourdces.Count]);
		}
	}

	public void ShowEndUI()
	{
		foreach (var ui in levelUIs)
		{
			ui.Hide();
		}

		if (LevelNumber == maxLevels)
		{
			levelEndMenu.ShowLastExtraction();
		}
		else
		{
			levelEndMenu.ShowExtraction();
		}
	}

	public void ChangeLevel()
	{
		if (LevelNumber < maxLevels)
		{
			LevelNumber++;
			//Change to Extraction Level
			var extractionLevelScene = GD.Load<PackedScene>(SceneFilePath);
			var extractionLevelInstance = extractionLevelScene.Instantiate<ResourceExtractionLevel>();
			extractionLevelInstance.LevelNumber = LevelNumber;
			GD.Print($"Changing to level {LevelNumber}");
			GetTree().ChangeSceneToNode(extractionLevelInstance);
		}
		else
		{
			GetTree().ChangeSceneToPacked(lastLevel);
		}
	}

	public double GetTimeToExtract()
	{
		if (timer == null) return 0;

		return timer.TimeLeft;
	}
}
