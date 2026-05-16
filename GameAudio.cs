using Godot;
using System;

public partial class GameAudio : AudioStreamPlayer
{
	public static GameAudio Instance { get; private set; }
	// Called when the node enters the scene tree for the first time.
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

	public void PlayAudio(AudioStream stream, float minPitch = 1.0f, float maxPitch = 1.0f)
	{
		Stream = stream;
		PitchScale = (float)GD.RandRange(minPitch, maxPitch);
		Play();
	}
}
