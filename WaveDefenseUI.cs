using Godot;

public partial class WaveDefenseUI : CanvasLayer
{
	[Export] WaveDefenseLevel waveDefenseLevel;
	[Export] Label timeLabel;

	public override void _Process(double delta)
	{
		base._Process(delta);

		// Format string
		if (waveDefenseLevel.PreparationTime >= 0)
		{
			timeLabel.Text = string.Format("Time: {0:00}:{1:00}", (int)(waveDefenseLevel.PreparationTime / 60), waveDefenseLevel.PreparationTime % 60);
		}
		else
		{
			timeLabel.Hide();
		}
	}
}
