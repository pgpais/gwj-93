using Godot;

public partial class Actor : CharacterBody3D
{
	[Export] Node3D _componentsParent;

	public T GetComponent<T>() where T : Node
	{
		foreach (Node child in _componentsParent.GetChildren())
		{
			if (child is T component)
			{
				return component;
			}
		}
		return null;
	}
}
