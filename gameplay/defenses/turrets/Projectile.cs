using Godot;

public partial class Projectile : RigidBody3D
{
    [Export] float timeToLive = 1;
    [Export] float speed = 10;
    [Export] float damage = 1;

    SceneTreeTimer timer;

    public override void _Ready()
    {
        base._Ready();

        timer = GetTree().CreateTimer(timeToLive);
        timer.Timeout += QueueFree;
    }
}
