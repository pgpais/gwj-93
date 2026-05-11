
using System;
using Godot;

public abstract partial class RecipeMachine : Building
{
    [ExportGroup("References")]
    [Export] Interactable interactable;

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

        interactable.InteractionStarted += OnInteractionStarted;
        interactable.InteractionStopped += OnInteractionStopped;
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
        // GD.Print($"[{nameof(Constructor)} {Name}] crafting time: {craftingTime}");
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

    protected virtual void EndCraft()
    {
        if (!CanCraftRecipe())
        {
            isCrafting = false;
            return;
        }
        else
        {
            StartCrafting();
        }
    }

    public abstract Func<RecipeData, bool> GetRecipePredicate();
    protected abstract bool CanCraftRecipe();
    protected abstract bool CanAcceptRecipe(RecipeData recipe);
    protected abstract void ApplyRecipe(RecipeData recipe);

    private void OnInteractionStarted(Interactor interactor)
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.RecipeMachineInteractionStarted, this);
    }

    private void OnInteractionStopped(Interactor interactor)
    {
        EventBus.Instance.EmitSignal(EventBus.SignalName.RecipeMachineInteractionStopped, this);
    }
}
