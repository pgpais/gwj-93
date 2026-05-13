shader_type spatial;

uniform sampler2D albedo_texture : hint_default_white, filter_linear_mipmap;
uniform sampler2D orm_texture : hint_default_white, filter_linear_mipmap;
uniform sampler2D normal_texture : hint_normal, filter_linear_mipmap;

uniform vec3 tint_color : source_color = vec3(1.,1.,1.);
uniform float metallic : hint_range(0.0, 1.0) = 0.0;
uniform float roughness : hint_range(0.0, 1.0) = 1.0;

uniform float belt_speed = 1.0;
uniform vec2 belt_direction = vec2(-1.0, 0.0);

void fragment() {
	vec2 shifted_UV = UV + (belt_direction * TIME * belt_speed);
	
	vec4 color = texture(albedo_texture, shifted_UV);
	vec3 normal = texture(normal_texture, shifted_UV).xyz;
	vec3 orm = texture(orm_texture, shifted_UV).xyz;
	
	ALBEDO = color.rgb * tint_color;
	NORMAL_MAP = normal;
	AO = orm.x;
	ROUGHNESS = orm.y * roughness;
	METALLIC = orm.z * metallic;
}
