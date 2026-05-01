using Godot;
using System;

public partial class GameUI : CanvasLayer
{
	private ProgressBar _healthBar;
	
	// Reference the player to get HealthComponent
	[Export] public HealthComponent PlayerHealth;

	public override void _Ready()
	{
		_healthBar = GetNode<ProgressBar>("ProgressBar");

		if (PlayerHealth != null)
		{
			// Set initial value
			_healthBar.Value = PlayerHealth.MaxHealth;

			// Connect to the signal created in the HealthComponent script
			// Observer pattern in action
			PlayerHealth.HealthChanged += OnPlayerHealthChanged;
		}
	}

	private void OnPlayerHealthChanged(int currentHealth)
	{
		// Update UI
		_healthBar.Value = currentHealth;
		
		if (currentHealth < 25)
		{
			// Could trigger a red "warning" flash here
			GD.Print("Low Health Warning!");
		}
	}
}
