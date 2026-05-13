shader_type spatial;
render_mode world_vertex_coords;

// for terrain tiles

uniform vec3 mix_from_tint : source_color = vec3(1.,1.,1.);
uniform sampler2D mix_from_albedo : source_color, hint_default_black;
uniform vec3 mix_to_tint : source_color = vec3(1.,1.,1.);
uniform sampler2D mix_to_albedo : source_color, hint_default_white;
uniform sampler2D mix_mask : hint_default_black;
uniform float mix_amount : hint_range(0.0, 1.0) = 1.0;

uniform float from_tiling_scale = 1.0;
uniform float to_tiling_scale = 1.0;

varying vec3 world_pos;
varying vec3 world_normal;

vec3 sample_triplanar(sampler2D sample_texture, vec3 pos, vec3 normal, float tiling_scale) {
    vec3 blend_weights = abs(normal);
    blend_weights = normalize(max(blend_weights, 0.00001));
    
    vec2 uv_x = pos.yz * tiling_scale;
    vec2 uv_y = pos.xz * tiling_scale;
    vec2 uv_z = pos.xy * tiling_scale;
    
    vec3 sample_x = texture(sample_texture, uv_x).rgb;
    vec3 sample_y = texture(sample_texture, uv_y).rgb;
    vec3 sample_z = texture(sample_texture, uv_z).rgb;
    
    return (sample_x * blend_weights.x)
        + (sample_y * blend_weights.y)
        + (sample_z * blend_weights.z);
}

void vertex() {
	world_pos = VERTEX;
	world_normal = NORMAL;
}

void fragment() {
	vec3 albedo_from = sample_triplanar(mix_from_albedo, world_pos, world_normal, from_tiling_scale);//texture(mix_from_albedo, UV).rgb;
	vec3 albedo_to = sample_triplanar(mix_to_albedo, world_pos, world_normal, to_tiling_scale);//texture(mix_to_albedo, UV).rgb;
	float mask = texture(mix_mask, UV).x;
	
	ALBEDO = mix(albedo_from * mix_from_tint, albedo_to * mix_to_tint, mask * mix_amount);
}
