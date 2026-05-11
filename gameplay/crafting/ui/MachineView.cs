using Godot;
using System;

public partial class MachineView : PanelContainer
{
	[Export] ActiveMachineView activeMachineView;
	[Export] RecipeSelectionUI recipeSelectionUI;

	RecipeMachine machine;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EventBus.Instance.RecipeMachineInteractionStarted += OnRecipeMachineInteractionStarted;
		EventBus.Instance.RecipeMachineInteractionStopped += OnRecipeMachineInteractionStopped;
	}

	private void OnRecipeMachineInteractionStarted(RecipeMachine machine)
	{
		this.machine = machine;

		SetMenuOnRecipeChanged();

		machine.RecipeChanged += (recipe) => SetMenuOnRecipeChanged();

		Show();
	}

	private void SetMenuOnRecipeChanged()
	{

		if (machine.CurrentRecipe != null)
		{
			activeMachineView.SetRecipeMachine(machine);
			activeMachineView.Show();
			recipeSelectionUI.Hide();
		}
		else
		{
			recipeSelectionUI.SetRecipeMachine(machine);
			recipeSelectionUI.Show();
			activeMachineView.Hide();
		}
	}

	private void OnRecipeMachineInteractionStopped(RecipeMachine machine)
	{
		activeMachineView.Hide();
		recipeSelectionUI.Hide();
		Hide();
		activeMachineView.ClearMachine();

		this.machine.RecipeChanged -= (recipe) => SetMenuOnRecipeChanged();
		this.machine = null;
	}
}
