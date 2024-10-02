using Godot;

public abstract partial class BaddieBase : Area2D
{
    [Export]
	public Vector2 SPEED {get; set; } = new Vector2(0, 2.5f);

	[Export]
	public int health {get; set; } = 20;

	protected bool alive {get; set; } = true;

    public abstract void SelfDestruct();

}