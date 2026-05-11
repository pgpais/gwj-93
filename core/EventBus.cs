using Godot;
using System;

public partial class EventBus : Node
{
	public static EventBus Instance { get; private set; }

	[Signal] public delegate void RecipeMachineInteractionStartedEventHandler(RecipeMachine machine);
	[Signal] public delegate void RecipeMachineInteractionStoppedEventHandler(RecipeMachine machine);

	public override void _Ready()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}
}
