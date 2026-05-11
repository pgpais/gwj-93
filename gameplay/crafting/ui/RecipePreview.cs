using Godot;
using System;

public partial class RecipePreview : PanelContainer
{
	[Signal] public delegate void PressedEventHandler();

	[Export] TextureRect previewRect;
	[Export] Label recipeName;
	[Export] Button recipeButton;

	RecipeData recipe;

	public override void _Ready()
	{
		recipeButton.Pressed += () => EmitSignal(SignalName.Pressed);
	}


	public void SetRecipe(RecipeData recipe)
	{
		this.recipe = recipe;
		previewRect.Texture = recipe.Icon;
		recipeName.Text = recipe.Name;
	}
}
