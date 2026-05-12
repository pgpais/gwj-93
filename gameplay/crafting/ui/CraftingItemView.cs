
using Godot;

public partial class CraftingItemView : Control
{
    [Export] Label itemName;
    [Export] TextureRect icon;
    [Export] Label currentAmount;
    [Export] Label maxAmount;
    [Export] Label requiredAmount;

    public void SetResource(GameResourceData itemData, int current, int max, int required)
    {
        itemName.Text = itemData.Name;
        icon.Texture = itemData.Icon;
        currentAmount.Text = current.ToString();
        maxAmount.Text = max.ToString();
        requiredAmount.Text = required.ToString();
    }

    public void SetQuantity(int current)
    {
        currentAmount.Text = current.ToString();
    }
}