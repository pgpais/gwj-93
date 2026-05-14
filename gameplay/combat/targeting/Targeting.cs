
using Godot;

public abstract partial class Targeting : Node
{
    [Signal] public delegate void TargetChangedEventHandler(Node3D newTarget);

    [Export] public Node3D CurrentTarget { get; protected set; }

    protected void ChangeTarget(Node3D newTarget)
    {
        if (CurrentTarget != null)
        {
            CurrentTarget.TreeExited -= RemoveTarget;
        }

        CurrentTarget = newTarget;

        if (CurrentTarget != null)
        {
            CurrentTarget.TreeExited += RemoveTarget;
        }
    }

    protected void RemoveTarget()
    {
        CurrentTarget = null;
    }


}
