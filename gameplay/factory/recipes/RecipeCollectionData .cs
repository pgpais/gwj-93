using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RecipeCollectionData : Resource
{
    [Export] public Array<RecipeData> recipes = new Array<RecipeData>();
}
