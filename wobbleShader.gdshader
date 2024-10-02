shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

uniform float amplitude = 0.014;
uniform float frequency = 25.0;
uniform float speed = 5.0;
uniform float amplitude_vertical = 0.0;
uniform float frequency_vertical = 0.0;
uniform float speed_vertical = 0.0;
uniform vec2 scroll_direction = vec2(0.0, 0.0);
uniform float scrolling_speed = 0.08;
uniform bool enable_palette_cycling = false;
uniform sampler2D palette;
uniform float palette_speed = 0.1;


void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float diff_x = amplitude * sin((frequency * SCREEN_UV.y) + (speed * TIME));
	float diff_y = amplitude_vertical * sin((frequency_vertical * SCREEN_UV.y) + (speed_vertical * TIME));
	vec2 scroll = scroll_direction * TIME * scrolling_speed;
	vec2 distortion =  vec2(SCREEN_UV.x + diff_x, SCREEN_UV.y + diff_y) + scroll;
	vec4 screenColor = texture(screen_texture, distortion);
	COLOR = screenColor;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
