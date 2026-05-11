using Godot;
using System;
using System.Linq;

public partial class RecipeSelectionUI : PanelContainer
{
	[Signal] public delegate void RecipeSelectedEventHandler(int recipeId);

	[Export] RecipeCollectionData recipeCollectionData;
	[Export] PackedScene recipePreviewScene;
	[Export] Container previewContainer;
	[Export] RecipeDetailView recipeDetailView;

	public override void _Ready()
	{
		//TODO: move this into an orchestrator class

		EventBus.Instance.RecipeMachineInteractionStarted += OnRecipeMachineInteractionStarted;
		EventBus.Instance.RecipeMachineInteractionStopped += OnRecipeMachineInteractionStopped;
	}

	public void ShowFilteredRecipes(Func<RecipeData, bool> predicate)
	{
		ClearRecipes();

		var recipes = recipeCollectionData.Recipes.Where(predicate).ToArray();

		foreach (var recipe in recipes)
		{
			var recipePreview = recipePreviewScene.Instantiate<RecipePreview>();
			recipePreview.SetRecipe(recipe);
			previewContainer.AddChild(recipePreview);

			recipePreview.Pressed += () => SelectRecipe(recipe);
		}
	}

	private void SelectRecipe(RecipeData recipe)
	{
		recipeDetailView.SetRecipe(recipe);
	}

	private void ClearRecipes()
	{
		foreach (var child in previewContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void OnRecipeMachineInteractionStarted(RecipeMachine machine)
	{
		ShowFilteredRecipes(machine.GetRecipePredicate());
		Show();
		recipeDetailView.Hide();
	}

	private void OnRecipeMachineInteractionStopped(RecipeMachine machine)
	{
		ClearRecipes();
		Hide();
	}
}
