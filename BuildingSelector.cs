using Godot;

public partial class BuildingSelector : PanelContainer
{
	[Export] BuildingController buildingController;
	[Export] BuildingDataCollection buildingDataCollection;
	[Export] Container previewsContainer;
	[Export] PackedScene previewScene;

	public override void _Ready()
	{
		base._Ready();

		if (buildingDataCollection != null)
		{
			Init(buildingDataCollection);
		}
	}

	public void Init(BuildingDataCollection buildingDataCollection)
	{
		//remove container children
		foreach (Node node in previewsContainer.GetChildren())
		{
			node.QueueFree();
		}

		foreach (BuildingData buildingData in buildingDataCollection.Buildings)
		{
			BuildingPreview preview = previewScene.Instantiate<BuildingPreview>();
			preview.SetBuildingData(buildingData);
			previewsContainer.AddChild(preview);

			preview.Pressed += () => SelectBuilding(buildingData);
		}
	}

	public void SelectBuilding(BuildingData buildingData)
	{
		buildingController.StartBuildingPreview(buildingData);
	}
}
