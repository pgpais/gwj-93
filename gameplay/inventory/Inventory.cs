using Godot;

[GlobalClass]
public abstract partial class Inventory : Resource
{
    public abstract int GetItemCount(GameResourceData item);
    public abstract bool HasCapacity(GameResourceData item, int amount = 1);
    public abstract bool TryAddItem(GameResourceData item, int amount = 1);
    public abstract bool HasItemAmount(GameResourceData item, int amount = 1);
    public abstract bool TryRemoveItem(GameResourceData item, int amount = 1);
}