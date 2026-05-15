using Godot;
using Godot.Collections;

[GlobalClass]
public partial class StorageInventory : Inventory
{
    [Signal] public delegate void InventoryChangedEventHandler();

    public Dictionary<GameResourceData, int> Items => items;
    [Export] Dictionary<GameResourceData, int> items;

    public StorageInventory()
    {
        items = new Dictionary<GameResourceData, int>();
    }

    public override int GetItemCount(GameResourceData item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }

    public override bool HasCapacity(GameResourceData item, int amount = 1)
    {
        return true;
    }

    public override bool HasItemAmount(GameResourceData item, int amount = 1)
    {
        return GetItemCount(item) >= amount;
    }

    public override bool TryAddItem(GameResourceData item, int amount = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += amount;
        }
        else
        {
            items.Add(item, amount);
        }
        EmitSignal(SignalName.InventoryChanged);
        return true;
    }

    public override bool TryRemoveItem(GameResourceData item, int amount = 1)
    {
        if (HasCapacity(item, amount))
        {
            items[item] -= amount;
            if (items[item] == 0) items.Remove(item);

            EmitSignal(SignalName.InventoryChanged);
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        string s = "";
        foreach (var item in items)
        {
            s += $"{item.Key.Name}: {item.Value}\n";
        }
        return s;
    }
}
