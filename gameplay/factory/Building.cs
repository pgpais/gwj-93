
using Godot;
using static DirectionUtils;

public partial class Building : Node3D
{
    public Vector3I GridPosition;
    public Direction direction; // 0–3 (90° steps)

    public virtual void OnPlaced() { }
}