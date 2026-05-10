using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Constructor : Building, IItemInput, IItemOutput
{
	[Export] ItemTransport input;
	[Export] ItemTransport output;

	[ExportGroup("Debug")]
	[Export] bool debug = false;
	[Export] RecipeData debug_startingRecipe;

	RecipeData recipe;

	Dictionary<GameResourceData, int> inputInventory = new();

	Dictionary<GameResourceData, int> outputInventory = new();

	bool isCrafting = false;
	float craftingTime = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (debug)
		{
			SetRecipe(debug_startingRecipe);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (input.HasItem())
		{
			PullItemFromInput();
		}

		if (!isCrafting)
		{
			TryToCraftRecipe();
		}
		else
		{
			CraftingTick(delta);
			if (craftingTime >= recipe.CraftTime)
			{
				EndCraft();
			}
		}

		foreach (var output in outputInventory)
		{
			if (output.Value > 0)
			{
				OutputItem(output.Key);
			}
		}

	}

	private void PullItemFromInput()
	{
		ReceiveItem(input.GetItem());
	}

	public void SetRecipe(RecipeData recipe)
	{
		this.recipe = recipe;
		inputInventory.Clear();
		outputInventory.Clear();

		foreach (var input in recipe.Input)
		{
			inputInventory.Add(input.Key, 0);
		}

		foreach (var output in recipe.Output)
		{
			outputInventory.Add(output.Key, 0);
		}
	}

	//TODO: receive items in input, craft using recipe, output the result

	public void ReceiveItem(GameResource item, int amount = 1)
	{
		if (inputInventory.ContainsKey(item.Data))
		{
			inputInventory[item.Data] += amount;
		}
		else
		{
			GD.Print($"Item {item.Name} not in recipe");
		}

		item.QueueFree();
	}

	private void TryToCraftRecipe()
	{
		if (!CanCraftRecipe())
		{
			return;
		}

		StartCrafting();
	}

	private bool CanCraftRecipe()
	{
		foreach (var input in recipe.Input)
		{
			if (!InventoryHasItem(input.Key, input.Value))
			{
				return false;
			}
		}

		return true;
	}

	private void OutputItem(GameResourceData itemData)
	{
		if (output.IsFull()) return;
		GD.Print($"Outputting {itemData.Name}");

		var item = GameResource.Instantiate(itemData);
		AddChild(item);

		InventorySpendItem(item.Data, 1);
		output.ReceiveItem(item);
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return input;
	}

	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return output;
	}

	private void StartCrafting()
	{
		isCrafting = true;
		craftingTime = 0f;
	}

	public void CraftingTick(double delta)
	{
		craftingTime += (float)delta;
		GD.Print($"[{nameof(Constructor)} {Name}] crafting time: {craftingTime}");
	}

	private void EndCraft()
	{
		foreach (var output in recipe.Output)
		{
			outputInventory[output.Key] += output.Value;
		}

		if (!CanCraftRecipe())
		{
			isCrafting = false;
			return;
		}
		else
		{
			StartCrafting();
		}
	}

	private bool InventoryHasItem(GameResourceData item, int amount = 1)
	{
		if (inputInventory.ContainsKey(item))
		{
			return inputInventory[item] >= amount;
		}
		return false;
	}

	private void InventorySpendItem(GameResourceData item, int amount = 1)
	{
		if (inputInventory.ContainsKey(item))
		{
			inputInventory[item] -= amount;
		}
	}
}
