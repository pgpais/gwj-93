using Godot;
using System;
using System.Collections.Generic;

public partial class StorageHUDView : PanelContainer
{
	[Export] StorageInventory inventory = new();
	[Export] PackedScene itemViewScene;
	[Export] Container viewsContainer;

	Dictionary<GameResourceData, ItemPreview> itemViews = new Dictionary<GameResourceData, ItemPreview>();

	public override void _Ready()
	{
		base._Ready();

		ClearViews();

		inventory.InventoryChanged += RefreshUI;
		RefreshUI();
	}

	public void ClearViews()
	{
		foreach (var child in viewsContainer.GetChildren())
		{
			child.QueueFree();
		}
		itemViews.Clear();
	}

	public void RefreshUI()
	{
		foreach (var item in inventory.Items)
		{
			if (itemViews.ContainsKey(item.Key))
			{
				itemViews[item.Key].SetResource(item.Key, item.Value);
				continue;
			}

			AddNewItemRow(item.Key, item.Value);
		}
	}

	public void AddNewItemRow(GameResourceData item, int quantity)
	{
		var itemView = itemViewScene.Instantiate<ItemPreview>();
		itemView.SetResource(item, quantity);

		viewsContainer.AddChild(itemView);
		itemViews.Add(item, itemView);
	}
}
