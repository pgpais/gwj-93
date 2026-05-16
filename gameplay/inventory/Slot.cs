using Godot;

public partial class SlotInventory
{
    [GlobalClass]
    public partial class Slot : Resource
    {
        [Export] public GameResourceData item;
        [Export] public int amount;
        [Export] public int stackSize = -1;
    }
}
