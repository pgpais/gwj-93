using Godot;
using System;

public partial class BuildingTooltip : PanelContainer
{
	[Export] PackedScene itemPreviewScene;
	[Export] Container ItemPreviewContainer;

	public void SetBuildingData(BuildingData buildingData)
	{
		// remove container children
		foreach (Node node in ItemPreviewContainer.GetChildren())
		{
			node.QueueFree();
		}

		foreach (var Cost in buildingData.Cost)
		{
			ItemPreview preview = itemPreviewScene.Instantiate<ItemPreview>();
			preview.SetResource(Cost.Key, Cost.Value);
			ItemPreviewContainer.AddChild(preview);
		}
	}
}
