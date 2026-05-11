using Godot;
using System;

public partial class ItemPreview : Control
{
	[Export] TextureRect resourceTexture;
	[Export] Label resourceNameLabel;

	internal void SetResource(GameResourceData itemData, int amount = 1)
	{
		resourceTexture.Texture = itemData.Icon;
		resourceNameLabel.Text = itemData.Name;
	}
}
