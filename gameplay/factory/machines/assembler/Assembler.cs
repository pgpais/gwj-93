using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Assembler : RecipeMachine, IItemInput, IItemOutput
{
	[Export] ItemTransport[] inputs = new ItemTransport[2];
	[Export] ItemTransport output;

	Dictionary<GameResourceData, int> inputInventory = new Dictionary<GameResourceData, int>();
	Dictionary<GameResourceData, int> outputInventory = new Dictionary<GameResourceData, int>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	override public void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.o
	public ItemTransport GetOutputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		return output;
	}

	public ItemTransport GetInputPort(DirectionUtils.Direction direction, Vector3I gridPos)
	{
		foreach (var inputPort in inputs)
		{
			var transportGridPos = FactoryGrid.Instance.WorldToGrid(inputPort.GlobalPosition);

			if (transportGridPos != gridPos) continue;
			if (this.direction != direction) continue;

			return inputPort;
		}
		return null;
	}

	private void PushItemsToOutputs()
	{
		foreach (var item in outputInventory)
		{
			if (output.IsFull()) return;
			if (item.Value <= 0) continue;

			var gameResource = GameResource.Instantiate(item.Key);
			AddChild(gameResource);
			output.ReceiveItem(gameResource);

			outputInventory[item.Key]--;
		}
	}

	private void PullItemsFromInputs()
	{
		foreach (var input in inputs)
		{
			if (input.HasItem())
			{
				ReceiveItem(input.GetItem());
			}
		}
	}

	private void ReceiveItem(GameResource gameResource)
	{
		inputInventory[gameResource.Data]++;
	}

	protected override bool CanAcceptRecipe(RecipeData recipe)
	{
		return recipe.Input.Count == inputs.Length && recipe.Output.Count == 1;
	}

	protected override void ApplyRecipe(RecipeData recipe)
	{
		for (int i = 0; i < inputs.Length; i++)
		{
			inputInventory.Clear();
		}
		outputInventory.Clear();

		foreach (var input in recipe.Input)
		{
			inputInventory.Add(input.Key, 0);
		}

		foreach (var output in recipe.Output)
		{
			outputInventory.Add(output.Key, 0);
		}

		foreach (var input in inputs)
		{
			input.SetFilter(new Array<GameResourceData>(recipe.Input.Keys.ToArray()), true);
		}

		output.SetFilter(new Array<GameResourceData>(recipe.Output.Keys.ToArray()), true);

	}

	private void TryToStartCraft()
	{
		if (!CanCraftRecipe())
		{
			return;
		}

		StartCrafting();
	}

	private bool CanCraftRecipe()
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
