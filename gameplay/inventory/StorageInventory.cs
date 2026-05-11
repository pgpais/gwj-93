using Godot;

[GlobalClass]
public partial class StorageInventory : Inventory
{
    public override int GetItemCount(GameResourceData item)
    {
        throw new System.NotImplementedException();
    }

    public override bool HasCapacity(GameResourceData item, int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public override bool HasItemAmount(GameResourceData item, int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryAddItem(GameResourceData item, int amount = 1)
    {
        throw new System.NotImplementedException();
    }

    public override bool TryRemoveItem(GameResourceData item, int amount = 1)
    {
        throw new System.NotImplementedException();
    }
}
