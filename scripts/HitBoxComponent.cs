using Godot;
using System;

public partial class HitBoxComponent : Area3D
{
	[Export] public HealthComponent HealthComp;

	public void HandleDamage(int amount)
	{
		HealthComp?.Damage(amount);
	}
}
