using Godot;
using System;

public partial class LaserProjectile : Area3D
{
	[Export] public float Speed = 10.0f;
	[Export] public int Damage = 25;
	[Export] public float Lifetime = 7.0f; // Seconds before it disappears automatically

	public override void _Ready()
	{
		// Create a timer so we don't have infinite projectiles flying into the void
		GetTree().CreateTimer(Lifetime).Timeout += () => QueueFree();

		// Connect the signal for when this Area3D overlaps something
		AreaEntered += OnAreaEntered;
		
		// NEW: Hitting objects 
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
		// Check if we hit a HitBoxComponent
		// Using the same Pattern Matching logic we used in the Player script
		if (area is HitBoxComponent hitbox)
		{
			hitbox.HandleDamage(Damage);
			Explode();
		}
	}
	
	// NEW: This triggers when hitting a StaticBody3D (Walls/floors/objects)
	private void OnBodyEntered(Node3D body)
	{
		// We don't need to check for a Hitbox here, 
		// because a wall is just a physical object.
		GD.Print($"Laser hit an object: {body.Name}");
		Explode();
	}

	private void Explode()
	{
		// This is where we'd spawn a "splash" effect later
		// For now, we just remove the projectile
		QueueFree();
	}
}
