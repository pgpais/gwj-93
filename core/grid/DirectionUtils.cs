
using Godot;

public static class DirectionUtils
{
    public enum Direction
    {
        East = 0,
        South = 1,
        West = 2,
        North = 3
    }

    public static Vector3I GetDirectionVector(this Direction direction)
    {
        return direction switch
        {
            Direction.North => new Vector3I(0, 0, -1),
            Direction.East => new Vector3I(1, 0, 0),
            Direction.South => new Vector3I(0, 0, 1),
            Direction.West => new Vector3I(-1, 0, 0),
            _ => new Vector3I(0, 0, 0)
        };
    }

    public static float GetDirectionAngle(this Direction direction)
    {
        return direction switch
        {
            Direction.East => 0,
            Direction.North => 90,
            Direction.West => 180,
            Direction.South => 270,
            _ => 0
        };
    }

    public static Direction RotateRight(this Direction direction)
    {
        return (Direction)(((int)direction + 1) % 4);
    }

    public static Direction RotateLeft(this Direction direction)
    {
        return (Direction)(((int)direction + 3) % 4);
    }
}
