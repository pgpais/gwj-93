using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SlotInventory : Inventory
{
    [Export] Array<Slot> slots;
    [Export] int capacity;

    public SlotInventory(int numberOfSlots = 1)
    {
        slots = new Array<Slot>();
        for (int i = 0; i < numberOfSlots; i++)
        {
            slots.Add(new Slot());
        }

        capacity = numberOfSlots;
    }

    public override int GetItemCount(GameResourceData item)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            bool isSlotOfItem = slot.item != null && slot.item == item;
            if (isSlotOfItem)
            {
                count += slot.amount;
            }
        }

        return count;
    }

    public override bool HasCapacity(GameResourceData item, int amount = 1)
    {
        if (slots.Count < capacity)
        {
            return true;
        }

        if ((GetItemCount(item) + amount) % item.MaxStack > 0)
        {
            return true;
        }

        return false;
    }

    public override bool HasItemAmount(GameResourceData item, int amount = 1)
    {
        return GetItemCount(item) >= amount;
    }

    public override bool TryAddItem(GameResourceData item, int amount = 1)
    {
        if (!HasCapacity(item, amount)) return false;

        var remainingAmount = amount;
        for (int i = 0; remainingAmount > 0; i++)
        {
            var slot = slots[i];

            if (slot.item == null)
            {
                slot.item = item;
                int spaceAvailable = Mathf.Min(remainingAmount, item.MaxStack);
                slot.amount = spaceAvailable;
                remainingAmount -= spaceAvailable;
            }
            else if (slot.item == item && slot.amount < item.MaxStack)
            {
                slot.amount++;
                int slotSpaceLeft = item.MaxStack - slot.amount;
                remainingAmount -= Mathf.Min(remainingAmount, slotSpaceLeft);
            }
        }

        return true;
    }

    public override bool TryRemoveItem(GameResourceData item, int amount = 1)
    {
        if (!HasItemAmount(item, amount)) return false;

        var remainingAmount = amount;
        for (int i = 0; remainingAmount > 0; i++)
        {
            var slot = slots[i];

            if (slot.item == item && slot.amount > 0)
            {
                var removedAmount = Mathf.Min(remainingAmount, slot.amount);
                slot.amount -= removedAmount;
                if (slot.amount <= 0) slot.item = null;

                remainingAmount -= Mathf.Min(remainingAmount, removedAmount);
            }
        }

        return true;
    }
}
