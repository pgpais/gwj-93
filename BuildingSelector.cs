using System.Collections.Generic;
using Godot;

public partial class BuildingSelector : PanelContainer
{
	[Export] StorageInventory inventory;
	[Export] BuildingController buildingController;
	[Export] BuildingDataCollection buildingDataCollection;
	[Export] Container previewsContainer;
	[Export] PackedScene previewScene;

	Dictionary<BuildingData, BuildingPreview> buildings = new Dictionary<BuildingData, BuildingPreview>();

	public override void _Ready()
	{
		base._Ready();

		if (buildingDataCollection != null)
		{
			Init(buildingDataCollection);
			DisableCostlyBuildings();
			inventory.InventoryChanged += OnInventoryChanged;
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

			buildings.Add(buildingData, preview);

			preview.Pressed += () => SelectBuilding(buildingData);
		}
	}

	public void SelectBuilding(BuildingData buildingData)
	{
		buildingController.StartBuildingPreview(buildingData);
	}

	public void OnInventoryChanged()
	{
		DisableCostlyBuildings();
	}

	private void DisableCostlyBuildings()
	{
		foreach (var building in buildings)
		{
			if (building.Key.Cost.Count <= 0) continue;

			foreach (var buildingCost in building.Key.Cost)
			{
				if (!inventory.HasItemAmount(buildingCost.Key, buildingCost.Value))
				{
					building.Value.SetEnabled(false);
				}
			}
		}
	}
}
