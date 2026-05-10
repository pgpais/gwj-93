using Godot;

[GlobalClass]
public partial class BuildingData : Resource
{
    [Export] public string Name { get; set; }
    [Export] public PackedScene Scene { get; set; }
    [Export] public Texture2D Preview { get; private set; }



    // Used for presenting the building in the ghost view
    [Export] public Mesh Mesh { get; private set; } //TODO: I'm wondering if we don't have a better way to handle this


}
