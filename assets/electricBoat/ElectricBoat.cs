using Godot;
using System;

public partial class ElectricBoat : BaddieBase 
{
	[Export]
	public PackedScene PCB_projectile { get; set; }

    [Export]
	public int CrashDamage { get; set; } = 10; 

	public CharacterBody2D player_instance {set; get;}

	public Node current_level {set; get;}
    private AudioStreamPlayer explodeSfx {get; set;}
    private AudioStreamPlayer firePCBSfx {get; set;}
    private bool is_ready { get;  set; } = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		this.SPEED = new Vector2(0, 0.5f);
		this.alive = true;

		var pcbTimer = this.GetNode<Timer>("FirePcbTimer"); 
        this.explodeSfx = this.GetNode<AudioStreamPlayer>("sounds/explode"); 
        this.firePCBSfx = this.GetNode<AudioStreamPlayer>("sounds/firePcbSound"); 
        
		pcbTimer.Start();
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
                this.updateHealthValues(cannon_Ball.Damage);				
			}
			cannon_Ball.currentSpeed = this.SPEED;
			cannon_Ball.Explode();
		}

        if(body is a2d_pcb)
		{
			var pcb = body as a2d_pcb;
            if(pcb.originBoat != this)
            {
                if(this.alive)
                {
                    this.updateHealthValues(pcb.Damage);
                }
                pcb.Explode(); 
            }
		}

		if(body is car_bdy_2d_player_ship)
		{
            var player = body as car_bdy_2d_player_ship;
            player.UpdateHealth(this.CrashDamage);
			this.SelfDestruct(); 
		}
	}

    private void updateHealthValues(int damage)
    {
        this.health -= damage; 
        this.updateAnimationToHealthStatus(); 
    }

    public void ExplosionComplete()
	{
        this.QueueFree();
	}

	public void FireThePcb()
	{
		Timer pcbTimer = GetNode<Timer>("FirePcbTimer"); 
		var pcb = this.PCB_projectile.Instantiate<a2d_pcb>(); 
		var pcbMarker = GetNode<Marker2D>("pcbFireMarker"); 

		// probably need one more number here so its the same speed no matter how close 
		// player is. 
		pcb.Position = new Vector2 (pcbMarker.GlobalPosition.X, pcbMarker.GlobalPosition.Y); 
        pcb.originBoat = this;
		pcb.ZIndex = this.ZIndex+1;
		Vector2 velocityForTarget = pcb.GlobalPosition - this.player_instance.GlobalPosition;
		pcb.targetNormal = velocityForTarget.Normalized();
		
		this.current_level.AddChild(pcb);
        this.firePCBSfx.Play();
		pcbTimer.Start();
	}
	

	public override void SelfDestruct()
    {
		GetNode<CollisionShape2D>("electricShipCollision").SetDeferred(CollisionShape2D.PropertyName.Disabled, true); 
		var boatSprite = GetNode<AnimatedSprite2D>("electricShipSprite");
		boatSprite.Play("kill");
        GetNode<Timer>("FirePcbTimer").Stop() ; 
        this.explodeSfx.Play();
    }

	private void updateAnimationToHealthStatus()
    {
		var boatSprite = GetNode<AnimatedSprite2D>("electricShipSprite");
        if(this.alive){
			if(this.health>30)
			{
				boatSprite.Play("normal");
			}
			else if(this.health>0)
			{
				boatSprite.Play("damaged"); 
			}
			if(this.health<=0){
				this.SelfDestruct();
			}
		}
	}
}
