using Godot;

public partial class BuildingPreview : PanelContainer
{
	[Signal] public delegate void PressedEventHandler();

	[Export] Button button;
	[Export] TextureRect textureRect;
	[Export] Label label;

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
}
