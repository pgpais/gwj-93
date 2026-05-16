using Godot;
using Godot.Collections;
using System;
using System.Linq;
using static ConveyorBelt;

public partial class Assembler : RecipeMachine, IItemInput, IItemOutput
{
	[Export] ItemTransport[] inputs = new ItemTransport[2];
	[Export] ItemTransport output;

	[Export] SlotInventory inputInventory;
	[Export] SlotInventory outputInventory;

	override public void _PhysicsProcess(double delta)
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

	public override void OnPlaced()
	{
		base.OnPlaced();

		foreach (var input in inputs)
		{
			input.ConnectToNeighboursOutput(direction.RotateRight().RotateRight());
		}

		output.ConnectToNeighboursInput(direction);
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


	public override Func<RecipeData, bool> GetRecipePredicate()
	{
		return recipe => recipe.Input.Count == inputs.Length && recipe.Output.Count == 1;
	}

	private void PushItemsToOutputs()
	{
		foreach (var result in currentRecipe.Output)
		{
			if (output.IsFull()) return;
			if (!outputInventory.HasItemAmount(result.Key, result.Value)) continue;

			if (outputInventory.TryRemoveItem(result.Key, result.Value))
			{
				var item = GameResource.Instantiate(result.Key);
				if (!output.CanReceiveItem(item))
				{
					item.QueueFree();
					continue;
				}

				AddChild(item);
				output.ReceiveItem(item);
			}
		}
	}

	private void PullItemsFromInputs()
	{
		foreach (var input in inputs)
		{
			if (input.HasItem() && inputInventory.HasCapacity(input.GetCurrentItem().Data))
			{
				ReceiveItem(input.TakeItem());
			}
		}
	}

	private void ReceiveItem(GameResource gameResource)
	{
		GD.Print($"[{Name}] Received {gameResource.Name}");
		inputInventory.TryAddItem(gameResource.Data);
		gameResource.QueueFree();
	}

	protected override bool CanAcceptRecipe(RecipeData recipe)
	{
		return recipe.Input.Count == inputs.Length && recipe.Output.Count == 1;
	}

	protected override void ApplyRecipe(RecipeData recipe)
	{
		inputInventory = new SlotInventory(inputs.Length);
		outputInventory = new SlotInventory();

		if (recipe == null) return;

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

	protected override void EndCraft()
	{
		base.EndCraft();
		foreach (var output in currentRecipe.Output)
		{
			if (!outputInventory.TryAddItem(output.Key, output.Value))
			{
				GD.PushError($"Failed to add {output.Key.Name} to output inventory");
			}
		}
	}

	public override int GetItemQuantity(GameResourceData resource)
	{
		return inputInventory.GetItemCount(resource);
	}
}
