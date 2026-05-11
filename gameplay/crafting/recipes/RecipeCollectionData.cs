using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RecipeCollectionData : Resource
{
    [Export] public Array<RecipeData> Recipes = new Array<RecipeData>();
}
