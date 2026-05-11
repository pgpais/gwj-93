using Godot;
using Godot.Collections;
using System;

public partial class ActiveMachineView : PanelContainer
{
	[Export] Label machineNameLabel;
	[Export] Container ingredientsContainer;
	[Export] Container resultsContainer;
	[Export] RecipePreview recipePreview;
	[Export] ProgressBar craftProgress;
	[Export] PackedScene ItemPreviewScene;
	[Export] Button stopButton;

	RecipeMachine machine;

	public override void _Ready()
	{
		base._Ready();

		stopButton.Pressed += OnStopButtonPressed;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (machine != null && machine.IsCrafting)
		{
			var craftProgress = machine.CraftingTime / machine.CurrentRecipe.CraftTime;
			this.craftProgress.Value = craftProgress;
		}
	}

	private void OnStopButtonPressed()
	{
		//TODO: stop machine craft
		GD.Print("Stop button pressed");
		machine.ClearRecipe();
	}

	public void ClearMachine()
	{
		this.machine = null;
		machineNameLabel.Text = "";

		foreach (Node child in ingredientsContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (Node child in resultsContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	public void SetRecipeMachine(RecipeMachine recipeMachine)
	{
		ClearMachine();

		this.machine = recipeMachine;

		machineNameLabel.Text = recipeMachine.GetType().Name;
		SetRecipe(machine.CurrentRecipe);
	}

	public new void Hide()
	{
		ClearMachine();
		base.Hide();
	}

	public void SetRecipe(RecipeData recipe)
	{
		recipePreview.SetRecipe(recipe);

		SetIngredients(recipe.Input);
		SetResults(recipe.Output);

		craftProgress.Value = 0;
		craftProgress.MaxValue = recipe.CraftTime;
	}

	private void SetIngredients(Dictionary<GameResourceData, int> input)
	{
		foreach (var resourceAmount in input)
		{
			ingredientsContainer.AddChild(CreateItemPreview(resourceAmount.Key, resourceAmount.Value));
		}
	}

	private void SetResults(Dictionary<GameResourceData, int> output)
	{
		foreach (var resourceAmount in output)
		{
			resultsContainer.AddChild(CreateItemPreview(resourceAmount.Key, resourceAmount.Value));
		}
	}

	private Node CreateItemPreview(GameResourceData key, int value)
	{
		var itemPreview = ItemPreviewScene.Instantiate<ItemPreview>();
		itemPreview.SetResource(key, value);
		return itemPreview;
	}
}
