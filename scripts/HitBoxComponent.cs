using Godot;
using System;

public partial class HitBoxComponent : Area3D
{
	// Drag HealthComponent into this slot in the Inspector
	[Export] public HealthComponent HealthComp;

	public void HandleDamage(int amount)
	{
		HealthComp?.Damage(amount);
	}
}
