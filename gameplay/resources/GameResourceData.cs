using Godot;

[GlobalClass]
public partial class GameResourceData : Resource
{
    [Export] public string Name { get; private set; }
    [Export] public Mesh Mesh { get; private set; }
}
