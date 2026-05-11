
using Godot;
using static DirectionUtils;

public partial class Building : Node3D
{
    public Vector3I GridPosition;
    public Direction direction; // 0–3 (90° steps)

    [Export] Node3D componentsParent;

    public virtual void OnPlaced() { }

    public T GetComponent<T>() where T : Node
    {
        foreach (Node child in componentsParent.GetChildren())
        {
            if (child is T component)
            {
                return component;
            }
        }
        return null;
    }
}