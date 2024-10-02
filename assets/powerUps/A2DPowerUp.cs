using Godot;
using System;

public partial class A2DPowerUp : Area2D
{
    [Export]
	public Vector2 SPEED {get; set; } = new Vector2(0, 3.5f);

    [Signal]
    public delegate void PowerUpCollectedEventHandler();

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
		Vector2 position = this.Position; 

		position += this.SPEED + ((float)delta * this.SPEED);
		this.Position = position; 
	}

    public void on_body_entered(Node2D body)
	{
        if(body is car_bdy_2d_player_ship)
		{
			this.triggerPowerUp(); 
		}
    }

    private void triggerPowerUp()
    {
        GetNode<CollisionShape2D>("SkullCollision2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        this.EmitSignal(SignalName.PowerUpCollected);
        this.GetNode<AudioStreamPlayer>("sounds/evilLaugh").Play(); 
        this.Visible=false;
    }

    private void OnScreenNotifierExited()
	{
		this.QueueFree();
	}
}
