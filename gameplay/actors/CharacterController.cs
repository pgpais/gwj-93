using Godot;

[GlobalClass]
public partial class CharacterController : Node
{
	[Export] public float Speed { get; private set; }

	public Vector2 MovInput { get; set; } = Vector2.Zero;

	CharacterBody3D _body;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_body = GetOwner<CharacterBody3D>(); // necessary because of Godot's bug
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Vector3 velocity = _body.Velocity;

		// Add the gravity.
		if (!_body.IsOnFloor())
		{
			velocity += _body.GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = MovInput;
		Vector3 direction = (_body.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(_body.Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(_body.Velocity.Z, 0, Speed);
		}

		_body.Velocity = velocity;
		_body.MoveAndSlide();
	}
}
