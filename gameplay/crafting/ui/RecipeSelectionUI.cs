using Godot;
using System;
using System.Linq;

public partial class RecipeSelectionUI : PanelContainer
{
	[Signal] public delegate void RecipeSelectedEventHandler(RecipeData recipe);

	[Export] RecipeCollectionData recipeCollectionData;
	[Export] PackedScene recipePreviewScene;
	[Export] Container previewContainer;
	[Export] RecipeDetailView recipeDetailView;

	RecipeMachine machine;

	public override void _Ready()
	{
		recipeDetailView.RecipeSelected += SelectRecipe;


	}

	private void SelectRecipe(RecipeData recipe)
	{
		machine.SetRecipe(recipe);
		recipeDetailView.Hide();
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

			recipePreview.Pressed += () => ShowRecipe(recipe);
		}
	}

	private void ShowRecipe(RecipeData recipe)
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

	public void SetRecipeMachine(RecipeMachine machine)
	{
		this.machine = machine;
		ShowFilteredRecipes(machine.GetRecipePredicate());

		Show();
		recipeDetailView.Hide();
	}

	private new void Hide()
	{
		ClearRecipes();
		base.Hide();
	}
}
