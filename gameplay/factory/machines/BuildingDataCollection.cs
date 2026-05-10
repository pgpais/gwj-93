using Godot;
using Godot.Collections;

[GlobalClass]
public partial class BuildingDataCollection : Resource
{
    [Export]
    public Array<BuildingData> Buildings { get; set; }
}
