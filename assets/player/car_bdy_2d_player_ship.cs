using Godot;
using System;

public partial class car_bdy_2d_player_ship : CharacterBody2D
{
    [Signal]
    public delegate void HealthChangedEventHandler(int currentHealth);

    [Signal]
    public delegate void PlayerDiedEventHandler();

	[Export]
	public PackedScene CannonBalls { get; set; }

    [Export]
	public PackedScene SmokeParticles { get; set; }

	[Export]
	float MAX_SPEED = 100.0f;

	[Export]
	float ROTATION_SPEED = 0.05f;
	
	[Export]
	float MAX_LEFT_ROTATION = -0.50f;
        
	[Export]
	float MAX_RIGHT_ROTATION = 0.50f;
	
	[Export]
	float ACCELERATION = 100.0f;

	[Export]
	float DECELERATION_DRIFT = 5.0f;

	[Export]
	float VERTICAL_CURRENT = 50.0f;

    [Export]
    public int health{ get; set; } = 15;

	private bool leftPressed = false; 

	private bool rightPressed = false; 

	private bool fireLeft = false; 

	private bool fireRight = false; 
    private AudioStreamPlayer cannonFireSfx {get; set;}

    public override void _Ready()
	{
        this.cannonFireSfx = GetNode<AudioStreamPlayer>("sounds/cannonFire");
    }

	public override void _Input(InputEvent @event)
	{
		// left movement 
		if(@event.IsActionPressed("player_left")){
			this.leftPressed = true;
		}
		if(@event.IsActionReleased("player_left")){
			this.leftPressed = false;
		}

		// right movement 
		if(@event.IsActionPressed("player_right")){
			this.rightPressed = true;
		}
		if(@event.IsActionReleased("player_right")){
			this.rightPressed = false;
		}

		// fire left 
		if(@event.IsActionPressed("fire_left")){
			this.fireLeft = true;
		}
		if(@event.IsActionReleased("fire_left")){
			this.fireLeft = false;
		}
		
		// fire right
		if(@event.IsActionPressed("fire_right")){
			this.fireRight = true;
		}
		if(@event.IsActionReleased("fire_right")){
			this.fireRight = false;
		}
	}

	public override void _Process(double delta)
	{
		if(this.fireRight){
			this.FireCannon("tmr_rightCannonCoolDown", "mkr_cannonRight", true);
		}

		if(this.fireLeft){
			this.FireCannon("tmr_leftCannonCoolDown", "mkr_cannonLeft", false);
		}

        var total_smoke = this.Owner.GetChildCount();
	}

    private void FireCannon(string cannonTimerName, string cannonLocationMarkerName, bool ballDirection=false)
    {

        Timer cannonTimer = GetNode<Timer>(cannonTimerName); 
			if(cannonTimer.TimeLeft <= 0){
                this.cannonFireSfx.Play();
				var cannonBall = this.CannonBalls.Instantiate<a_2d_cannon_ball>(); 
				cannonBall.BallDirection=ballDirection;
				var cannonMarker = GetNode<Marker2D>(cannonLocationMarkerName); 
				cannonBall.Transform = cannonMarker.GlobalTransform;
				this.Owner.AddChild(cannonBall);
                this.MakeSmoke(cannonMarker);				
				cannonTimer.Start(); 
              

			}
    }

    private void MakeSmoke(Marker2D cannonLocationMarker)
    {
        if(cannonLocationMarker.Name == "mkr_cannonLeft"){
            var leftSmoke = this.SmokeParticles.Instantiate<GpuParticles2D>();
            leftSmoke.GlobalPosition = cannonLocationMarker.GlobalPosition;
            leftSmoke.OneShot = true;
            leftSmoke.Emitting = true; 
            this.Owner.AddChild(leftSmoke);
        }
         if(cannonLocationMarker.Name == "mkr_cannonRight"){
            var rightSmoke = this.SmokeParticles.Instantiate<GpuParticles2D>();
            rightSmoke.GlobalPosition = cannonLocationMarker.GlobalPosition;
            // idk why I need to rotate because I changed the direction on the particles. 
            rightSmoke.Rotate((float)Math.PI);
            rightSmoke.OneShot = true;
            rightSmoke.Emitting = true; 
            this.Owner.AddChild(rightSmoke);
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom game play actions.
		Vector2 keyInputAsVector = Input.GetVector("player_left", "player_right", "player_up", "player_down");
		Vector2 currentVelocity = this.Velocity;
		this.UpdateVelocityFromInput(delta, keyInputAsVector, currentVelocity);
		this.UpdateRotation(delta);
		MoveAndSlide();
	}

    private void UpdateRotation(double delta)
    {
		float rotation = this.Rotation; 
    float rotationSpeed = this.ROTATION_SPEED + (this.ROTATION_SPEED * (float)delta); 

		if(this.leftPressed || this.rightPressed)
		{
			if(this.leftPressed && this.rightPressed)
			{
				rotation = this.CenterRotation(rotation, rotationSpeed);
				
			}
			else if(this.leftPressed){
				rotation -= rotationSpeed; 
				if(rotation < this.MAX_LEFT_ROTATION)
				{
					rotation = this.MAX_LEFT_ROTATION;
				}
			}
			else if(this.rightPressed){
				rotation += rotationSpeed; 
				if(rotation > this.MAX_RIGHT_ROTATION)
				{
					rotation = this.MAX_RIGHT_ROTATION;
				}
			}
		}
		else{
			rotation = this.CenterRotation(rotation, rotationSpeed);
		}

		this.Rotation = rotation;
    }

    private float CenterRotation(float rotation, float rotationSpeed)
    {
			if(rotation >= 0.05f){
				rotation -= rotationSpeed; 
				}
				else if(rotation <= -0.05f){
					rotation += rotationSpeed; 
				}
				else {
					rotation = 0.0f; 
				}
			return rotation;
    }

    private void UpdateVelocityFromInput(double delta, Vector2 inputVector, Vector2 currentVelocity)
    {
			if(inputVector.X != 0.0f)
			{
				if (currentVelocity.X > this.MAX_SPEED)
				{
					currentVelocity.X = this.MAX_SPEED;
				}
				else{
					currentVelocity.X = inputVector.X * this.ACCELERATION;
				}
			}
			if(inputVector.Y != 0.0f)
			{
				if(currentVelocity.Y > this.MAX_SPEED){
					currentVelocity.Y = this.MAX_SPEED;
				}
				else{
					currentVelocity.Y = inputVector.Y * this.ACCELERATION;
				}
			}

			if(inputVector.X == 0.0f){

				if(currentVelocity.X<0)
					currentVelocity.X += this.DECELERATION_DRIFT;

				if(currentVelocity.X>0)
					currentVelocity.X -= this.DECELERATION_DRIFT;
			}

			if(inputVector.Y == 0.0f)
			{
				currentVelocity.Y = this.VERTICAL_CURRENT; 
			}

		this.Velocity = currentVelocity+(currentVelocity * (float) delta);
    }

    public void animation_finished()
	{
		var boatSprite = GetNode<AnimatedSprite2D>("BoatSprite2D");
		if(boatSprite.Animation == "death")
		{
			this.EmitSignal(SignalName.PlayerDied);
		}
	}

    private void updateAnimationToHealthStatus()
    {
		var boatAnimation = this.GetNode<AnimatedSprite2D>("BoatSprite2D");
        if(this.health>75)
        {
            boatAnimation.Play("zeroDamage");
        }
        else if(this.health>25){
            boatAnimation.Play("oneDamage");
        }
        else if(this.health>0){
            boatAnimation.Play("twoDamage");
        }
         else if(this.health<= 0){
            GetNode<CollisionShape2D>("Col2DShip").SetDeferred(CollisionShape2D.PropertyName.Disabled, true); 
            boatAnimation.Play("death");
            GetNode<AudioStreamPlayer>("sounds/explode").Play();
        }
	}

    public void UpdateHealth(int damageTaken){
        this.health -= damageTaken;
        this.updateAnimationToHealthStatus();
        GD.Print("Player Health: "+ this.health); 
        this.EmitSignal(SignalName.HealthChanged, this.health);
    }
}

