using Godot;

[GlobalClass]
public partial class PlayerCharacterInput : Node
{
	[Export] CharacterController controller;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleMovementInput();
	}

	private void HandleMovementInput()
	{
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		controller.MovInput = inputDir;
	}
}
