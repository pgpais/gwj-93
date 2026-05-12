using Godot;
using System;

public partial class ItemPreview : Control
{
	[Export] TextureRect resourceTexture;
	[Export] Label resourceNameLabel;
	[Export] Label resourceAmountLabel;

	internal void SetResource(GameResourceData itemData, int amount = -1)
	{
		resourceTexture.Texture = itemData.Icon;
		resourceNameLabel.Text = itemData.Name;

		if (amount == -1) resourceAmountLabel.Hide();
		else
		{
			resourceAmountLabel.Show();
			resourceAmountLabel.Text = amount.ToString();
		}
	}
}
