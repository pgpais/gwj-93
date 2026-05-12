using Godot;

public partial class RecipeDetailView : PanelContainer
{
	[Signal] public delegate void RecipeSelectedEventHandler(RecipeData recipe);

	[Export] PackedScene ItemPreviewScene;
	[Export] Label recipeNameLabel;
	[Export] Container ingredientsContainer;
	[Export] Container resultsContainer;
	[Export] Button selectButton;

	RecipeData recipe;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectButton.Pressed += () => EmitSignal(SignalName.RecipeSelected, recipe);
		Hide();
	}

	public void SetRecipe(RecipeData recipe)
	{
		this.recipe = recipe;

		recipeNameLabel.Text = recipe.Name;

		RemoveChildren();

		CreateIngredients();
		CreateResults();

		Show();
	}

	private void RemoveChildren()
	{
		foreach (Node child in ingredientsContainer.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var child in resultsContainer.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void CreateIngredients()
	{
		foreach (var ingredient in recipe.Input)
		{
			ingredientsContainer.AddChild(CreateItemPreview(ingredient.Key, ingredient.Value));
		}
	}

	private void CreateResults()
	{
		foreach (var result in recipe.Output)
		{
			resultsContainer.AddChild(CreateItemPreview(result.Key, result.Value));
		}
	}

	private ItemPreview CreateItemPreview(GameResourceData itemData, int amount)
	{
		var itemPreview = ItemPreviewScene.Instantiate<ItemPreview>();
		itemPreview.SetResource(itemData, amount);
		return itemPreview;
	}
}
