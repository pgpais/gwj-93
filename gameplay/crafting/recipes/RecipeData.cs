using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RecipeData : Resource
{
    [Export] public Dictionary<GameResourceData, int> Input { get; private set; } = [];
    [Export] public Dictionary<GameResourceData, int> Output { get; private set; } = [];
    [Export] public float CraftTime { get; private set; } = 1f;

    [ExportGroup("Metadata")]
    [Export] public string Name { get; private set; }
    [Export] public string Description { get; private set; }
    [Export] public Texture2D Icon { get; private set; }
}
