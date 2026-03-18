using Godot;
using System;

public partial class PlayerMovement : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 6.5f;

	[Export]
	private Texture2D Full;
    [Export]
    private Texture2D Two;
    [Export]
    private Texture2D One;
    [Export]
    private Texture2D None;

    [Export]
    private TextureRect heartUI;

    [Export]
	private CpuParticles3D particles;

    private int HP = 3;

	[Export]
	private Timer iFrames;


    public override void _PhysicsProcess(double delta)
	{
		switch (HP)
		{
			case 3:
				heartUI.Texture = Full;
				break;
            case 2:
				heartUI.Texture = Two;
                break;
            case 1:
				heartUI.Texture = One;
                break;
            case 0:
                heartUI.Texture = None;
                break;
        }

        Vector3 velocity = Velocity;

		if (HP > 0)
		{
            // Add the gravity.
            if (!IsOnFloor())
            {
                velocity += GetGravity() * (float)delta;
            }

            // Handle Jump.
            if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
            {
                velocity.Y = JumpVelocity;
            }

            // Get the input direction and handle the movement/deceleration.
            // As good practice, you should replace UI actions with custom gameplay actions.
            Vector2 inputDir = Input.GetVector("Left", "Right", "Forward", "Backward");
            Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
            if (direction != Vector3.Zero)
            {
                velocity.X = direction.X * Speed;
                velocity.Z = direction.Z * Speed;
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
            }

            Velocity = velocity;
            MoveAndSlide();
        }
        else
        {
            velocity = Vector3.Zero;
            Velocity = velocity;

            MeshInstance3D mesh = FindChild("Body") as MeshInstance3D;
            mesh.Visible = false;
            if (iFrames.IsStopped())
            {
                GetTree().Quit();
            }
        }
		
	}

	public void OnHit()
	{
		if (iFrames.IsStopped())
		{
			iFrames.Start();
            HP--;
            particles.Emitting = true;
        }
	}
}
