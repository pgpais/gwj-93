using Godot;

public partial class Enemy : Actor
{
    [Export] Node3D target;
    [Export] AICharacterInput input;
    [Export] HealthComponent health;

    public override void _Ready()
    {
        base._Ready();

        input.SetTarget(target);

        health.HealthDepleted += QueueFree;
    }

    public void Initialize(Node3D attackTarget)
    {
        target = attackTarget;
        input.SetTarget(attackTarget);
    }

    public void SetTarget(Node3D target)
    {
        this.target = target;
        input.SetTarget(target);
    }
}