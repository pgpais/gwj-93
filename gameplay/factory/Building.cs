
using Godot;
using static DirectionUtils;

public partial class Building : Node3D
{
    public BuildingData Data { get; private set; }

    public Vector3I GridPosition;
    public Direction direction; // 0–3 (90° steps)

    [Export] Node3D componentsParent;

    public virtual void OnPlaced() { }
    public virtual void OnRemoved() { QueueFree(); }

    public void SetData(BuildingData data)
    {
        Data = data;
    }

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