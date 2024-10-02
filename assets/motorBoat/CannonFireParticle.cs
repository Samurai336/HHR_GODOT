using Godot;
using System;

public partial class CannonFireParticle : GpuParticles2D
{
    public void SmokeComplete()
	{
		this.QueueFree();
	}
}
