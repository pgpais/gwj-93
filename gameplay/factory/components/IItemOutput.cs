using Godot;
using static DirectionUtils;

public interface IItemOutput
{
    public ItemTransport GetOutputPort(Direction direction, Vector3I gridPos);
}
