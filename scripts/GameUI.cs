using Godot;
using System;

public partial class GameUI : CanvasLayer
{
	private ProgressBar _healthBar;
	
	// We'll reference the player to get their HealthComponent
	[Export] public HealthComponent PlayerHealth;

	public override void _Ready()
	{
		_healthBar = GetNode<ProgressBar>("ProgressBar");

		if (PlayerHealth != null)
		{
			// Set initial value
			_healthBar.Value = PlayerHealth.MaxHealth;

			// Connect to the signal we created in the HealthComponent script
			// This is the "Observer" pattern in action
			PlayerHealth.HealthChanged += OnPlayerHealthChanged;
		}
	}

	private void OnPlayerHealthChanged(int currentHealth)
	{
		// Smoothly update the UI
		_healthBar.Value = currentHealth;
		
		if (currentHealth < 25)
		{
			// You could trigger a red "warning" flash here!
			GD.Print("Low Health Warning!");
		}
	}
}
