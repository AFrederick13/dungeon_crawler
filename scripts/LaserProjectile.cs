using Godot;
using System;

public partial class LaserProjectile : Area3D
{
	[Export] public float Speed = 10.0f;
	[Export] public int Damage = 25;
	[Export] public float Lifetime = 7.0f; // Seconds before it disappears automatically

	public override void _Ready()
	{
		// Create a timer to avoid infinite projectiles
		GetTree().CreateTimer(Lifetime).Timeout += () => QueueFree();

		// Connect signal for when this Area3D overlaps something
		AreaEntered += OnAreaEntered;
		
		BodyEntered += OnBodyEntered;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Move the projectile forward (-Z axis in Godot)
		// Transform.Basis.Z is the "Forward" vector of the projectile itself
		Position += Transform.Basis.Z * -Speed * (float)delta;
	}

	// HitBox collision
	private void OnAreaEntered(Area3D area)
	{
		// Check if collision a HitBoxComponent
		// Use same Pattern Matching logic used in Player script
		if (area is HitBoxComponent hitbox)
		{
			hitbox.HandleDamage(Damage);
			Explode();
		}
	}
	
	// Triggers when hitting a StaticBody3D (Walls/floors/objects)
	private void OnBodyEntered(Node3D body)
	{
		GD.Print($"Laser hit an object: {body.Name}");
		Explode();
	}

	private void Explode()
	{
		// Remove the projectile
		QueueFree();
	}
}
