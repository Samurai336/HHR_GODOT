using Godot;
using System;
using System.Linq;

public partial class level1 : Node
{
    [Signal]
    public delegate void ReturnToMainScreenEventHandler();

	[Export]
	public PackedScene Cottages { get; set; }

	[Export]
	public PackedScene MotorBoats { get; set; }

	[Export]
	public PackedScene ElectricBoats { get; set; }

	[Export]
	public PackedScene PowerUpSpawn { get; set; }

    [Export]
    public Timer cottageSpawnTimer {get;set;}

	private Vector2 _parallax_speed {get; set;}	

    private bool playerIsDead {get; set;} = false;

    [Export]
	public int LevelItemsZIndex { get; set; } = 5;

    private AudioStreamPlayer LevelMusic {get; set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this._parallax_speed = this.GetNode<prlx_bg_level_one>("StaticBody2DLevelOne/PrlxBgLevelOne").Speed; 
        this.LevelMusic = GetNode<AudioStreamPlayer>("music/levelOneSong");
        this.cottageSpawnTimer = GetNode<Timer>("cottageSpawn");

		this.StartLevel(); 

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartLevel()
	{
		this.cottageSpawnTimer.Start();
		GetNode<Timer>("motorBoatSpawn").Start();
		GetNode<Timer>("electricBoatSpawn").Start();
        GetNode<Timer>("powerUpSpawn").Start();
        this.GetNode<car_bdy_2d_player_ship>("CarBdy2DPlayerShip").PlayerDied += this.BeginPlayerDeadSequence;
        this.LevelMusic.Play();
	}

    private Vector2 GetRandomSpawnPosition()
    {
        var boatSpawnMin = GetNode<Marker2D>("boatSpawnLess");
		var boatSpawnMax = GetNode<Marker2D>("boatSpawnMore");
		var r = new Random();
		int randomYPosition = r.Next((int)boatSpawnMin.Position.X, (int)boatSpawnMax.Position.X);
		return new Vector2(randomYPosition, boatSpawnMin.Position.Y);
    } 

    public void OnPowerUpSpawn()
    {
        var powerUpSpawn = this.PowerUpSpawn.Instantiate<A2DPowerUp>(); 		
        powerUpSpawn.Position = this.GetRandomSpawnPosition(); 
        powerUpSpawn.ZIndex = this.LevelItemsZIndex;
        powerUpSpawn.PowerUpCollected += this.StartScreenKill;
		this.AddChild(powerUpSpawn);
    }

    private void StartScreenKill()
    {
        GetNode<Timer>("screenKillSequence").Start();
        var wobble_shader = GetNode<ColorRect>("wobble_shader_effect");
        wobble_shader.Visible = true;
    }

    public void ScreenKillSequenceCompleteTimeOut()
	{
        var wobble_shader = GetNode<ColorRect>("wobble_shader_effect");
        this.KillAll();
        wobble_shader.Visible = false;
    }

    public void playerDeadSequenceTimeout(){
        this.LevelMusic.Stop();
        this.EmitSignal(SignalName.ReturnToMainScreen);
    }

    private void KillAll()
    {
        var allActiveChildren = this.GetChildren().Where(x=> (x is BaddieBase) || (x is house) ).ToArray(); 

        foreach (var badGuy in allActiveChildren){
            if (badGuy is BaddieBase)
            {
                var boatBaddie = badGuy as BaddieBase;
                boatBaddie.SelfDestruct(); 
            }

            if(badGuy is house){
                var house = badGuy as house;
                house.SelfDestruct();
            }
        }

    }

    public void BeginPlayerDeadSequence(){
        GetNode<Timer>("playerDeadTimeOut").Start();
    }

    public void OnCottageSpawnerTimeOut()
	{
		var cottage_spawn = Cottages.Instantiate<house>();
        cottage_spawn.ZIndex = this.LevelItemsZIndex;
		cottage_spawn.SPEED = this._parallax_speed;
		cottage_spawn.Position=GetNode<Marker2D>("houseSpawnMarkerRight").Position;		
		var r = new Random();
		if (r.Next(2)==1){
			cottage_spawn.Scale = new Vector2(-1,1);
			cottage_spawn.Position=GetNode<Marker2D>("houseSpawnMarkerLeft").Position;		
		}
        this.cottageSpawnTimer.WaitTime = r.Next(1,3);
		this.AddChild(cottage_spawn);
	}

	public void OnMotorboatSpawnerTimeOut()
	{
		
		var motorBoat_spawn = this.MotorBoats.Instantiate<MotorBoat>();

		var boatSpawnMin = GetNode<Marker2D>("boatSpawnLess");
		var boatSpawnMax = GetNode<Marker2D>("boatSpawnMore");
		var r = new Random();
		int randomYPosition = r.Next((int)boatSpawnMin.Position.X, (int)boatSpawnMax.Position.X);
		motorBoat_spawn.Position = new Vector2(randomYPosition, boatSpawnMin.Position.Y);
        motorBoat_spawn.ZIndex = this.LevelItemsZIndex;
		this.AddChild(motorBoat_spawn);

	}

    public void OnElectricShipSpawnerTimeOut()
	{
		
		var electric_spawn = this.ElectricBoats.Instantiate<ElectricBoat>();
		electric_spawn.player_instance = GetNode<CharacterBody2D>("CarBdy2DPlayerShip");
		electric_spawn.current_level = this; 

		var boatSpawnMin = GetNode<Marker2D>("boatSpawnLess");
		var boatSpawnMax = GetNode<Marker2D>("boatSpawnMore");
		var r = new Random();
		int randomYPosition = r.Next((int)boatSpawnMin.Position.X, (int)boatSpawnMax.Position.X);
		electric_spawn.Position = new Vector2(randomYPosition, boatSpawnMin.Position.Y);
        electric_spawn.ZIndex = this.LevelItemsZIndex;
		this.AddChild(electric_spawn);

	}
}
