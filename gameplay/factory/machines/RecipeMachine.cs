
using Godot;

public abstract partial class RecipeMachine : Building
{
    [ExportGroup("Debug")]
    [Export] bool debug = false;
    [Export] RecipeData debug_startingRecipe;
    protected RecipeData currentRecipe;

    protected bool isCrafting = false;
    protected float craftingTime = 0f;

    public override void _Ready()
    {
        base._Ready();

        if (debug)
        {
            SetRecipe(debug_startingRecipe);
        }
    }

    public void SetRecipe(RecipeData recipe)
    {
        if (!CanAcceptRecipe(recipe))
        {
            GD.Print($"{Name} cannot use recipe {recipe.ResourceName}");
            return;
        }

        currentRecipe = recipe;

        ApplyRecipe(recipe);
    }

    public virtual void CraftingTick(double delta)
    {
        craftingTime += (float)delta;
        GD.Print($"[{nameof(Constructor)} {Name}] crafting time: {craftingTime}");
        if (craftingTime >= currentRecipe.CraftTime)
        {
            EndCraft();
        }
    }

    protected virtual void StartCrafting()
    {
        isCrafting = true;
        craftingTime = 0f;
    }
    protected virtual void EndCraft() { }
    protected abstract bool CanAcceptRecipe(RecipeData recipe);
    protected abstract void ApplyRecipe(RecipeData recipe);
}
