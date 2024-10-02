using Godot;
using Vector2 = Godot.Vector2;

public partial class MotorBoat : BaddieBase
{
    public int CrashDamage { get; set; } = 5;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		this.SPEED = new Vector2(0, 2.5f);
		this.health = 20;
		this.alive = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void SelfDestruct()
    {
		GetNode<CollisionShape2D>("BoatCollision").SetDeferred(CollisionShape2D.PropertyName.Disabled, true); 
		var boatSprite = GetNode<AnimatedSprite2D>("BoatSprite");
		boatSprite.Play("boatSplosion");
        this.GetNode<AudioStreamPlayer>("sounds/killExplode").Play(); 
        
    }

	public void ExplosionComplete()
	{
		this.QueueFree();
	}

    public override void _PhysicsProcess(double delta)
	{
		Vector2 position = this.Position; 

		position += this.SPEED + ((float)delta * this.SPEED);
		this.Position = position; 
	}

	public void on_body_entered(Node2D body)
	{
		if(body is a_2d_cannon_ball)
		{
			var cannon_Ball = body as a_2d_cannon_ball;
			if(this.alive)
			{
                this.updateHealthValues(cannon_Ball.Damage);
				
			}
			cannon_Ball.currentSpeed = this.SPEED;
			cannon_Ball.Explode();
		}

		if(body is car_bdy_2d_player_ship)
		{
            var player = body as car_bdy_2d_player_ship;
            player.UpdateHealth(this.CrashDamage);
			this.SelfDestruct(); 
		}

        if(body is a2d_pcb)
		{
			var pcb = body as a2d_pcb;
            if(this.alive)
			{
                this.updateHealthValues(pcb.Damage);
            }
			pcb.Explode(); 
		}

        if(body is ElectricBoat)
		{
            this.SPEED = Vector2.Zero;
			this.SelfDestruct(); 
		}
	}

    private void updateHealthValues(int damageAmount)
    {
        this.health -= damageAmount; 
        this.updateAnimationToHealthStatus(); 
    }

    private void updateAnimationToHealthStatus()
    {
         if(this.alive){
			if(this.health<=0){
				this.alive = false;
				this.SelfDestruct(); 
			}
		 }
    }

    private void OnScreenNotifierExited()
	{
		this.QueueFree();
	}

}
