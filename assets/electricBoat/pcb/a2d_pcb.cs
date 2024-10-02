using Godot;
using System;

public partial class a2d_pcb : Area2D
{

	[Export]
	public int Damage {get;set;} = 20; 

	[Export]
	public float speed {get;set;} = 2f;

	[Export]
	public float rotationSpeed {get;set;} = 10.0f; 

	public Vector2 targetNormal {get;set;} 

    public ElectricBoat originBoat {get;set;}

	private bool hit = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Explode()
	{
		// turn off collision here so we don't need the if loop. 
		if(this.hit == false){
			this.hit=true;
			GetNode<CollisionPolygon2D>("PcbCollision").SetDeferred(CollisionPolygon2D.PropertyName.Disabled, true); 
			var cannonBallSprite = GetNode<AnimatedSprite2D>("PCBAnimatedSprite2D");
            var explodeSfx = GetNode<AudioStreamPlayer>("sounds/explodeSfx");
            explodeSfx.Play();
			cannonBallSprite.Play("pcbHit");
		}
	}



	public override void _PhysicsProcess(double delta)
	{
		if(this.hit == false){
			var positionUpdate = this.targetNormal*this.speed;
			this.Position -= positionUpdate +  ((float)delta * positionUpdate);
    		this.Rotation -= rotationSpeed + ((float)delta * rotationSpeed); 

		}

	}

	public void on_collision(Node2D body)
	{
		if(body is car_bdy_2d_player_ship)
		{
            var player = body as car_bdy_2d_player_ship;
            player.UpdateHealth(5);
			this.Explode(); 
		}
	}
}
