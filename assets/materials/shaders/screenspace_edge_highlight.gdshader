shader_type spatial;

render_mode unshaded, ambient_light_disabled;

uniform float edge_brightness = 2.0;
uniform float sample_distance = 20.0;
uniform float depth_threshold = 0.1;
uniform float depth_artifact_correction_coef = 2;

uniform sampler2D color_texture : hint_screen_texture;
uniform sampler2D normal_texture : hint_normal_roughness_texture;
uniform sampler2D depth_texture : hint_depth_texture;

void vertex() {
	POSITION = vec4(VERTEX.xy, 1.0, 1.0);
}

void fragment() {
	float multiplier = 1.0;
	
	vec3 fragment_color = texture(color_texture, SCREEN_UV).rgb;
	vec3 fragment_normal = texture(normal_texture, SCREEN_UV).xyz;
	float fragment_depth = texture(depth_texture, SCREEN_UV).r;
	fragment_depth = PROJECTION_MATRIX[3][2] / (fragment_depth + PROJECTION_MATRIX[2][2]);
	
	float recalculated_sample_distance = sample_distance * (1.0 / fragment_depth);
	float highest_angle_difference = 0.0;
	float highest_depth_difference = 0.0;
	
	float lowest_depth = fragment_depth;
	vec2 lowest_depth_UV = SCREEN_UV;
	
	for (int x = -1; x < 2; x++){
		for (int y = -1; y < 2; y++){
			if (abs(x) > 0 || abs(y) > 0){
				float x_test = SCREEN_UV.x * (recalculated_sample_distance / VIEWPORT_SIZE.x) * float(x);
				float y_test = SCREEN_UV.y * (recalculated_sample_distance / VIEWPORT_SIZE.y) * float(y);
				vec2 offset_uv = SCREEN_UV + vec2(x_test, y_test);
				
				vec3 kernel_normal = texture(normal_texture, offset_uv).xyz;
				float kernel_depth = texture(depth_texture, offset_uv).r;
				kernel_depth = PROJECTION_MATRIX[3][2] / (kernel_depth + PROJECTION_MATRIX[2][2]);
				
				// get UV coords for nearest fragment
				if (kernel_depth < lowest_depth){
					lowest_depth = kernel_depth;
					lowest_depth_UV = offset_uv;
				}
				
				// find largest depth difference in kernel
				float depth_difference = abs(kernel_depth - fragment_depth);
				if (depth_difference > highest_depth_difference){
					highest_depth_difference = depth_difference;
				}
				
				// find largest angle difference in kernel
				float angle_difference = length(kernel_normal - fragment_normal);
				if (angle_difference >= highest_angle_difference){
					highest_angle_difference = angle_difference;
				}
			}
			
		}
	}
	
	float angle = 1.0 - dot(normalize(fragment_normal-vec3(0.5)),  vec3(0.0,0.0,1.0));
	
	// >= 15 degrees between surfaces
	if ((highest_angle_difference) >= PI/24.0 || highest_depth_difference >= depth_threshold + angle * depth_artifact_correction_coef){
		multiplier *= edge_brightness;
		fragment_color = texture(color_texture, lowest_depth_UV).rgb;
	} else {
		discard;
	}
	
	ALBEDO = fragment_color * multiplier;
}