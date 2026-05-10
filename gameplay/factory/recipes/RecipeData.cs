using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RecipeData : Resource
{
    [Export] public Dictionary<GameResourceData, int> Input { get; private set; } = [];
    [Export] public Dictionary<GameResourceData, int> Output { get; private set; } = [];
    [Export] public float CraftTime { get; private set; } = 1f;


}
