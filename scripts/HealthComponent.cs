using Godot;
using System;

public partial class HealthComponent : Node
{
	[Signal] public delegate void HealthChangedEventHandler(int currentHealth);
	[Signal] public delegate void DiedEventHandler();
	
	[Export] public int MaxHealth = 100;
	private int _currentHealth;
	
	public override void _Ready()
	{
		_currentHealth = MaxHealth;
	}
	
	public void Damage(int amount)
	{
		_currentHealth -= amount;
		EmitSignal(SignalName.HealthChanged, _currentHealth);
		
		if (_currentHealth <= 0)
		{
			EmitSignal(SignalName.Died);
		}
	}
}
