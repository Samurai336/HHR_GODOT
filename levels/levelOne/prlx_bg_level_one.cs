using Godot;
using System;

public partial class prlx_bg_level_one : ParallaxBackground
{
	[Export]
	public Vector2 Speed { get; set; } = new Vector2(0,2); 
	 

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

    public override void _PhysicsProcess(double delta)
    {
		Vector2 currentOffset = this.ScrollBaseOffset;
		currentOffset += this.Speed;
		this.ScrollBaseOffset = currentOffset; 
        base._PhysicsProcess(delta);
    }
}
