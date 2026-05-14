using System.Collections.Generic;
using Godot;

public partial class Enemy : Actor
{
    public static List<Enemy> enemies = new List<Enemy>();

    [Export] Node3D target;
    // [Export] AICharacterInput input;
    [Export] HealthComponent health;
    [Export] CharacterController controller;

    public override void _EnterTree()
    {
        base._EnterTree();

        enemies.Add(this);
    }

    public override void _Ready()
    {
        base._Ready();

        // input.SetTarget(target);

        health.HealthDepleted += QueueFree;
    }

    public void Initialize(Node3D attackTarget)
    {
        target = attackTarget;
        // input.SetTarget(attackTarget);
    }

    public void SetTarget(Node3D target)
    {
        this.target = target;
        // input.SetTarget(target);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        enemies.Remove(this);
    }

    public void MoveTowards(Vector2 moveDir)
    {
        controller.MovInput = moveDir;
    }
}