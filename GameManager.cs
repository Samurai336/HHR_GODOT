using Godot;
using System;

public partial class GameManager : Node
{
    public Node LevelManager { get; set; }
    public CanvasLayer MainMenu { get; set; }

    public CanvasLayer PauseMenu { get; set; }

    public Node ActiveLevel { get; set; }
    public Node2D GameUI { get; set; }

    private bool gameIsActive = false;
    private bool gameIsPaused = false;

    private AudioStreamPlayer MainMenuMusic {get; set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        this.LevelManager = GetNode<Node>("MainGameNode");
        this.MainMenu = GetNode<CanvasLayer>("MainMenu");
        this.PauseMenu = GetNode<CanvasLayer>("PauseMenu");
        this.GameUI = GetNode<Node2D>("MainGameNode/GameUI");
        this.MainMenuMusic = GetNode<AudioStreamPlayer>("MainMenu/ButterflyInParis");
        this.MainMenuMusic.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    private void UnloadLevel(){
        if(IsInstanceValid(this.ActiveLevel))
        {
            this.ActiveLevel.QueueFree();
        }
        this.ActiveLevel = null;
    }

    public override void _Input(InputEvent @event)
	{
        if(this.gameIsActive)
        {
            if(@event.IsActionPressed("pause_button")){
                this.gameIsPaused = !this.gameIsPaused; 
                this.ActiveLevel.GetTree().Paused = this.gameIsPaused; 
                this.PauseMenu.Visible = this.gameIsPaused;
            }

            if(this.gameIsPaused){
                if(@event.IsActionPressed("quit_key")){
                    this.EndGameButtonPress();
                }
            }
        }
        else {
            if(@event.IsActionPressed("start_space")){                
                this.StartGameButtonPress();
            }
        }
    }

    private void EndGameButtonPress()
    {
        this.gameIsActive = false; 
        this.PauseMenu.Visible = false;
        this.MainMenu.Visible = true;  
        this.gameIsPaused = false; 
        this.gameIsActive = false;
        this.ActiveLevel.GetTree().Paused = false; 
        this.UnloadLevel(); 
        this.MainMenuMusic.Play();
    }

    private void StartGameButtonPress()
    {
        this.UnloadLevel();
        this.ActiveLevel = GD.Load<PackedScene>("res://levels/levelOne/level1.tscn").Instantiate();
        var level = this.ActiveLevel as level1; 
        level.ReturnToMainScreen += this.EndGameButtonPress;
        this.ActiveLevel.GetNode<car_bdy_2d_player_ship>("CarBdy2DPlayerShip").HealthChanged += this.UpdateUIToHealth;
        this.LevelManager.AddChild(this.ActiveLevel);
        this.GameUI.Visible=true;
        this.MainMenu.Visible = false;        
        this.gameIsActive = true;
        GetNode<AudioStreamPlayer>("StartingBoom").Play();
        this.MainMenuMusic.Stop();        
        this.GameUI.GetNode<AnimatedSprite2D>("henryHealthMeter").Play("fullHealth");
    }

    private void UpdateUIToHealth(int currentHealth)
    {
        var henryAnimation = this.GameUI.GetNode<AnimatedSprite2D>("henryHealthMeter");
        if(currentHealth>75)
        {
            henryAnimation.Play("fullHealth");
        }
        else if(currentHealth>25){
            henryAnimation.Play("quarterHealth");
        }
        else if(currentHealth>0){
            henryAnimation.Play("closeToDead");
        }
         else if(currentHealth<= 0){
            henryAnimation.Play("death");
        }
    }
}
