@tool

class_name SpinModifier3D
extends SkeletonModifier3D

var skeleton: Skeleton3D
var bone_id: int
var bone_id_parent: int

enum Axis {
	X_Plus, Y_Plus, Z_Plus, X_Minus, Y_Minus, Z_Minus
}

@export_enum(" ") var bone_name: String:
	set(name):
		bone_name = name
		var _skeleton: Skeleton3D = get_skeleton()
		if _skeleton:
			bone_id = _skeleton.find_bone(bone_name)
			bone_id_parent = _skeleton.get_bone_parent(bone_id)

@export var spin_rpm: float = 600.0
@export var spin_axis: Axis = Axis.Z_Minus

var elapsed_time := 0.0

func _validate_property(property: Dictionary) -> void:
	if property.name == "bone_name":
		var _skeleton: Skeleton3D = get_skeleton()
		if _skeleton:
			property.hint = PROPERTY_HINT_ENUM
			property.hint_string = _skeleton.get_concatenated_bone_names()

func validate_skeleton() -> void:
	skeleton = get_skeleton()
	if skeleton:
		bone_id = skeleton.find_bone(bone_name)
		bone_id_parent = skeleton.get_bone_parent(bone_id)

func _process_modification() -> void:
	if !skeleton or skeleton != get_skeleton():
		validate_skeleton()
	
	if !skeleton or !is_inside_tree():
		return
	
	elapsed_time += get_process_delta_time() * (spin_rpm / 60.0)
	var spin = fmod(elapsed_time, TAU)
	var pose_transform = skeleton.get_bone_global_pose(bone_id)
	var transformed_pose = pose_transform.rotated_local(get_spin_axis_local(), spin)
	
	skeleton.set_bone_global_pose(bone_id, transformed_pose)
	

func get_spin_axis_local() -> Vector3:
	match spin_axis:
		Axis.X_Plus: return Vector3(1,0,0)
		Axis.Y_Plus: return Vector3(0,1,0)
		Axis.Z_Plus: return Vector3(0,0,1)
		Axis.X_Minus: return Vector3(-1,0,0)
		Axis.Y_Minus: return Vector3(0,-1,0)
		_, Axis.Z_Minus: return Vector3(0,0,-1)
