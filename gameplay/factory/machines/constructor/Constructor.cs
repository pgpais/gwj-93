using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Constructor : RecipeMachine, IItemInput, IItemOutput
{
	[Export] ItemTransport input;
	[Export] ItemTransport output;

	[Export] SlotInventory inputInventory;
	[Export] SlotInventory outputInventory;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (currentRecipe == null) return;

		PullItemsFromInputs();

		if (!isCrafting)
		{
			TryToStartCraft();
		}
		else
		{
			CraftingTick(delta);
		}

		PushItemsToOutputs();
	}

	private void PullItemsFromInputs()
	{
		if (input.HasItem() && inputInventory.HasCapacity(input.GetCurrentItem().Data))
		{
			ReceiveItem(input.TakeItem());
		}
	}

	private void PushItemsToOutputs()
	{
		foreach (var result in currentRecipe.Output)
		{
			if (output.IsFull()) return;
			if (!outputInventory.HasItemAmount(result.Key, result.Value)) continue;

			var item = GameResource.Instantiate(result.Key);
			AddChild(item);
			output.ReceiveItem(item);

			outputInventory.TryRemoveItem(result.Key, result.Value);
		}
	}

	public override Func<RecipeData, bool> GetRecipePredicate()
	{
		return recipe => recipe.Input.Count == 1 && recipe.Output.Count == 1;
	}

	public void ReceiveItem(GameResource gameResource, int amount = 1)
	{
		GD.Print($"[{Name}] Received {gameResource.Name}");
		inputInventory.TryAddItem(gameResource.Data);
		gameResource.QueueFree();
	}

	private void TryToStartCraft()
	{
		if (!CanCraftRecipe())
		{
			return;
		}

		StartCrafting();
	}

	protected override bool CanCraftRecipe()
	{
		foreach (var input in currentRecipe.Input)
		{
			if (!inputInventory.HasItemAmount(input.Key, input.Value))
			{
				return false;
			}
		}

		foreach (var output in currentRecipe.Output)
		{
			if (!outputInventory.HasCapacity(output.Key, output.Value))
			{
				return false;
			}
		}

		return true;
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return input;
	}

	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return output;
	}

	protected override void EndCraft()
	{
		base.EndCraft();
		foreach (var output in currentRecipe.Output)
		{
			if (!outputInventory.TryAddItem(output.Key, output.Value))
			{
				GD.PushError($"Failed to add {output.Key.Name} to output inventory");
			}
			{
				GD.Print($"[{Name}] Added {output.Key.Name} to output inventory, outputInventory: {outputInventory.GetItemCount(output.Key)}");
			}
		}
	}

	protected override bool CanAcceptRecipe(RecipeData recipe)
	{
		return recipe.Input.Count == 1 && recipe.Output.Count == 1;
	}

	protected override void ApplyRecipe(RecipeData recipe)
	{
		inputInventory = new SlotInventory();
		outputInventory = new SlotInventory();

		if (recipe == null) return;

		input.SetFilter(new Array<GameResourceData>(recipe.Input.Keys.ToArray()), true);
		output.SetFilter(new Array<GameResourceData>(recipe.Output.Keys.ToArray()), true);
	}

	public override int GetItemQuantity(GameResourceData resource)
	{
		return inputInventory.GetItemCount(resource);
	}
}
