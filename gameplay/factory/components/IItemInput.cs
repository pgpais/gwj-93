using Godot;
using static DirectionUtils;

public interface IItemInput
{
    public ItemTransport GetInputPort(Direction direction, Vector3I gridPos);
}
