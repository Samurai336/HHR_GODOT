using Godot;
using System;

public partial class house : Area2D
{
	[Export]
	public Vector2 SPEED {get; set; } = new Vector2(0, 5.0f);
	// Called when the node enters the scene tree for the first time.

	[Export]
	public int health {get; set; } = 100;

	private bool alive {get; set; } = true;

	public override void _Ready()
	{
		var houseSprite = GetNode<AnimatedSprite2D>("houseSprite2D");        
		houseSprite.Play();

        var smokeParticles = GetNode<GpuParticles2D>("ChimneySmoke");
        smokeParticles.Emitting=true;
        
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 position = this.Position; 

		position += this.SPEED + ((float)delta * this.SPEED);
		this.Position = position; 
        
	}

	public void on_area_entered(Node2D body)
	{
		if(body is a_2d_cannon_ball)
		{
			
			var cannon_Ball = body as a_2d_cannon_ball;
			if(this.alive)
			{
				this.health -= cannon_Ball.Damage; 
				this.updateAnimationToHealthStatus(); 
			}
			cannon_Ball.currentSpeed = this.SPEED;
			cannon_Ball.Explode();
		}
	}

    public void SelfDestruct(){
        this.health = -1; 
        this.updateAnimationToHealthStatus();
    }

    private void updateAnimationToHealthStatus()
    {
		var houseSprite = GetNode<AnimatedSprite2D>("houseSprite2D");
        if(this.alive){
			if(this.health>50)
			{
				houseSprite.Play("normal");
			}
			else if(this.health>0)
			{
				houseSprite.Play("damaged"); 
			}
			if(this.health<=0){
				this.alive = false;
				houseSprite.Play("kill"); 
                this.GetNode<AudioStreamPlayer>("sounds/houseExplode").Play(); 
                var smokeParticles = GetNode<GpuParticles2D>("ChimneySmoke");
                smokeParticles.Emitting=false;
			}
		}
    }

	private void Animation2DCompleted()
	{
		var houseSprite = GetNode<AnimatedSprite2D>("houseSprite2D");
		
		if(houseSprite.Animation == "kill")
		{
			houseSprite.Play("dead"); 
		}
	}

    private void OnScreenNotifierExited()
	{
		this.QueueFree();
	}

}
