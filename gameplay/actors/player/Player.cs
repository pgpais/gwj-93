using System;
using Godot;

public partial class Player : Actor
{
    [Export] CharacterController characterController;
    [Export] Interactor interactor;

    public override void _Ready()
    {
        base._Ready();

        interactor.StartedInteracting += OnInteractionStarted;
        interactor.StoppedInteracting += OnInteractionStopped;
    }

    private void OnInteractionStarted(Interactable interactable)
    {
        characterController.MovementEnabled = false;
    }

    private void OnInteractionStopped(Interactable interactable)
    {
        characterController.MovementEnabled = true;
    }
}
