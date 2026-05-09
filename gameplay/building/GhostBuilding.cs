using Godot;
using System;

public partial class GhostBuilding : Node3D
{
	[Export] MeshInstance3D _mesh;

	[ExportGroup("Colors")]
	[Export] Color _validColor = new Color(0, 1, 0, 0.5f);
	[Export] Color _invalidColor = new Color(1, 0, 0, 0.5f);

	public void SetMesh(Mesh mesh)
	{
		_mesh.Mesh = mesh;
	}

	public void SetValid(bool valid)
	{
		var mat = _mesh.GetActiveMaterial(0) as StandardMaterial3D;
		if (mat == null) return;

		mat = mat.Duplicate() as StandardMaterial3D;
		_mesh.SetSurfaceOverrideMaterial(0, mat);

		mat.AlbedoColor = valid ? _validColor : _invalidColor;
		mat.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
	}
}
