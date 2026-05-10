using Godot;
using Godot.Collections;
using System.Linq;

public partial class Constructor : RecipeMachine, IItemInput, IItemOutput
{
	[Export] ItemTransport input;
	[Export] ItemTransport output;

	Dictionary<GameResourceData, int> inputInventory = new();

	Dictionary<GameResourceData, int> outputInventory = new();

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (currentRecipe == null) return;

		if (input.HasItem())
		{
			PullItemFromInput();
		}

		if (!isCrafting)
		{
			TryToStartCraft();
		}
		else
		{
			CraftingTick(delta);
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

	protected override void EndCraft()
	{
		base.EndCraft();
		foreach (var output in currentRecipe.Output)
		{
			outputInventory[output.Key] += output.Value;
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

	protected override bool CanAcceptRecipe(RecipeData recipe)
	{
		return recipe.Input.Count == 1 && recipe.Output.Count == 1;
	}

	protected override void ApplyRecipe(RecipeData recipe)
	{
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

		input.SetFilter(new Array<GameResourceData>(recipe.Input.Keys.ToArray()), true);
		output.SetFilter(new Array<GameResourceData>(recipe.Output.Keys.ToArray()), true);
	}
}
