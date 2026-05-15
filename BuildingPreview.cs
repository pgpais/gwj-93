using Godot;

public partial class BuildingPreview : PanelContainer
{
	[Signal] public delegate void PressedEventHandler();

	[Export] Button button;
	[Export] TextureRect textureRect;
	[Export] Label label;

	[Export] PackedScene tooltipScene;

	BuildingData buildingData;

	public override void _Ready()
	{
		button.Pressed += () => EmitSignal(SignalName.Pressed);
	}

	public void SetBuildingData(BuildingData buildingData)
	{
		this.buildingData = buildingData;
		textureRect.Texture = buildingData.Preview;
		label.Text = buildingData.Name;
	}

	public void SetEnabled(bool enabled)
	{
		button.Disabled = !enabled;
	}

	public GodotObject GetTooltip(string forText)
	{
		if (buildingData.Cost.Count == 0) return null;

		BuildingTooltip tooltip = tooltipScene.Instantiate<BuildingTooltip>();
		tooltip.SetBuildingData(buildingData);
		return tooltip;
	}
}
