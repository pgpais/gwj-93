using Godot;
using System;

public partial class LevelEndMenu : PanelContainer
{
	[Signal] public delegate void ContinuePressedEventHandler();

	[Export] string extractionTitle;
	[Export] string extractionDescription;
	[Export] string lastExtractionTitle;
	[Export] string lastExtractionDescription;

	[ExportGroup("References")]
	[Export] Label titleLabel;
	[Export] Label descriptionLabel;
	[Export] Button continueButton;

	public override void _Ready()
	{
		continueButton.Pressed += () => EmitSignal(SignalName.ContinuePressed);
	}

	public void ShowLastExtraction()
	{
		titleLabel.Text = lastExtractionTitle;
		descriptionLabel.Text = lastExtractionDescription;
		Show();
	}

	public void ShowExtraction()
	{
		titleLabel.Text = extractionTitle;
		descriptionLabel.Text = extractionDescription;
		Show();
	}
}
