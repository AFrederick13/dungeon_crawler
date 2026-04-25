using Godot;
using System;

public partial class Player : CharacterBody3D
{
	// --- Exports ---
	[Export] public float Speed = 10.0f;
	[Export] public RayCast3D AttackRay;
	[Export] public int AttackDamage = 25;
	
	// NEW
	[Export] public PackedScene laser_projectile;
	

	// --- Private Members ---
	private NavigationAgent3D _navAgent;
	private AnimationPlayer _animPlayer;
	private Node3D _model;
	private Vector3 _targetPosition;
	
	// NEW
	private Node3D _muzzle;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_animPlayer = GetNode<AnimationPlayer>("character-g2/AnimationPlayer");
		_model = GetNode<Node3D>("character-g2");
		_targetPosition = GlobalPosition;
		
		// NEW -- may need to adjust path
		// _muzzle = GetNode<Marker3D>("%Muzzle");
		
		// NEW -- hard coded path 
		_muzzle = GetNode<Marker3D>("character-g2/character-g/root/torso/arm-right/Pistol/blaster-b2/blaster-b/Muzzle");
	}	

	// This is now the input switchboard
	public override void _Input(InputEvent @event)
	{
		// Left Click or Hold: Movement
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			SetTargetFromMouse(mouseEvent.Position);
		}

		// Right Click: Aim and Attack
		if (@event is InputEventMouseButton attackEvent && attackEvent.Pressed && attackEvent.ButtonIndex == MouseButton.Right)
		{
			AimAndPerformAttack(attackEvent.Position);
		}
		
		// Spacebar for "Panic Attacks" and testing
		if (@event.IsActionPressed("ui_accept"))
		{
			PerformAttack();
		}
	}

	private void SetTargetFromMouse(Vector2 mousePos)
	{
		var camera = GetViewport().GetCamera3D();
		if (camera == null) return;

		var from = camera.ProjectRayOrigin(mousePos);
		var to = from + camera.ProjectRayNormal(mousePos) * 1000;
		
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = GetWorld3D().DirectSpaceState.IntersectRay(query);

		if (result.Count > 0)
		{
			_targetPosition = (Vector3)result["position"];
			_navAgent.TargetPosition = _targetPosition;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// "Hold to Move" logic
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			Vector2 mousePos = GetViewport().GetMousePosition();
			SetTargetFromMouse(mousePos);
		}

		if (_navAgent.IsNavigationFinished())
		{
			_animPlayer.Play("holding-right"); 
			return;
		}

		Vector3 nextPathPos = _navAgent.GetNextPathPosition();
		Vector3 newVelocity = (nextPathPos - GlobalPosition).Normalized() * Speed;

		if (newVelocity.Length() > 0.1f)
		{
			var lookTarget = new Vector3(nextPathPos.X, GlobalPosition.Y, nextPathPos.Z);
			_model.LookAt(lookTarget, Vector3.Up);
			_animPlayer.Play("walk");
		}

		Velocity = newVelocity;
		MoveAndSlide();
	}


	// Attacking
	//private void PerformAttack()
	//{
		//GD.Print("Attempting Attack...");
//
		//if (AttackRay != null && AttackRay.IsColliding())
		//{
			//var target = AttackRay.GetCollider();
			//GD.Print($"Hit");
//
			//// Ensure this matches your lowercase 'b' naming convention from earlier
			//if (target is HitBoxComponent hitbox)
			//{
				//hitbox.HandleDamage(AttackDamage);
				//GD.Print("Damage Dealt!");
			//}
		//}
	//}
	
	// NEW Attacking
	private void PerformAttack()
	{
		if (laser_projectile == null || _muzzle == null) return;

		// Create a "Clone" of the laser scene
		var bullet = laser_projectile.Instantiate<LaserProjectile>();

		// Add it to the main scene tree
		// We add it to the "Owner" (usually the Level) so that if the player 
		// moves, the bullets don't move with them!
		GetTree().Root.AddChild(bullet);

		// Set the bullet's position and rotation to match the gun's muzzle
		bullet.GlobalTransform = _muzzle.GlobalTransform;
		
		GD.Print("Laser Fired!");
	}
	
	private void AimAndPerformAttack(Vector2 mousePos)
	{
		var camera = GetViewport().GetCamera3D();
		if (camera == null) return;

		// 1. Create the Ray from the camera
		var from = camera.ProjectRayOrigin(mousePos);
		var toNormal = camera.ProjectRayNormal(mousePos);

		// 2. Create a mathematical Plane at the player's feet height
		// Vector3.Up means the plane is horizontal (like a floor)
		// GlobalPosition.Y ensures the plane is exactly at the player's level
		Plane aimingPlane = new Plane(Vector3.Up, GlobalPosition.Y);

		// 3. Find where the camera ray hits this virtual plane
		// Intersection yields a Vector3? (nullable Vector3)
		var intersectionPoint = aimingPlane.IntersectsRay(from, toNormal);

		if (intersectionPoint.HasValue)
		{
			Vector3 lookPoint = intersectionPoint.Value;

			// 4. Rotate the model to face that point
			var target = new Vector3(lookPoint.X, GlobalPosition.Y, lookPoint.Z);
			
			// Safety check: Don't LookAt if the point is exactly where we are
			if (GlobalPosition.DistanceTo(target) > 0.1f)
			{
				_model.LookAt(target, Vector3.Up);
			}

			// 5. Stop movement and fire
			_navAgent.TargetPosition = GlobalPosition;
			PerformAttack();
		}
	}
}
