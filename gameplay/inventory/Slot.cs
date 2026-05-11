using Godot;

public partial class SlotInventory
{
    [GlobalClass]
    public partial class Slot : Resource
    {
        public GameResourceData item;
        public int amount;
    }
}
