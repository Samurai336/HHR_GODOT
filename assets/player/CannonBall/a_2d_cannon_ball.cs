using Godot;
using System;

public partial class a_2d_cannon_ball : Area2D
{
	[Export]
	Vector2 MAX_SPEED = new Vector2(0, 800.0f);

	[Export]
	public Vector2 currentSpeed {get;set;}
	
	[Export]
	public int Damage {get;set;} = 10; 

	private bool hit = false;

	public bool BallDirection { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.currentSpeed=this.MAX_SPEED;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnScreenNotifierExited()
	{
		this.QueueFree();
	}

	public void Explode()
	{
		// turn off collision here so we don't need the if loop. 
		if(this.hit == false){
			this.hit=true;
			GetNode<CollisionShape2D>("CannonBallCollision").SetDeferred(CollisionShape2D.PropertyName.Disabled, true); 
			var cannonBallSprite = GetNode<AnimatedSprite2D>("CannonBallSprite");
			cannonBallSprite.Play("ballHit");
            this.GetNode<AudioStreamPlayer>("sounds/explode").Play(); 
		}
	}

	public void ExplosionComplete()
	{
		// see whats coming in here. 
		this.QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
			if(hit){
				this.Position += this.currentSpeed +  ((float)delta * this.currentSpeed);
			}
			else
			{
				if(this.BallDirection)
				{
					this.Position += this.Transform.X  * this.currentSpeed.Y * (float)delta;
				}
				else 
				{
					this.Position -= this.Transform.X  * this.currentSpeed.Y * (float)delta;

				}
			}
	}
}

