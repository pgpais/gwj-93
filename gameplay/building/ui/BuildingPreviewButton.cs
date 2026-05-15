using Godot;
using System;

public partial class BuildingPreviewButton : Button
{
	BuildingPreview buildingPreview;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buildingPreview = GetOwner<BuildingPreview>();
	}

	public override GodotObject _MakeCustomTooltip(string forText)
	{
		return buildingPreview.GetTooltip(forText);
	}
}
