using Godot;
using System;

public partial class Enemy : Node3D
{
	[Export]
	private Node3D player;

	[Export]
	private Timer timer;

	[Export]
	private float detectionDistance = 2.5f;

	private int state = 0;

	[Export]
	private float Speed = 5f;


    private Vector3 dir;

	private Vector3 scal;
	private Vector3 pos;
    private Vector3 rot;

	private RandomNumberGenerator rng = new RandomNumberGenerator();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		rng.Randomize();

		scal = Scale;
		pos = Position;
		rot = Rotation;

		var damageArea = FindChild("DamageArea") as Area3D;
        var killArea = FindChild("KillArea") as Area3D;

		damageArea.BodyEntered += OnDamageEntered;
		killArea.BodyEntered += OnKillEntered;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		
		switch(state)
		{
			case 0: // idle
				rot.Y += 0.01f;
				
				//GD.Print(Position.DistanceTo(player.Position));
				
				if (Position.DistanceTo(player.Position) < detectionDistance)
				{
                    dir = player.Position - pos;
                    dir.Y = 0;

                    if (dir.Length() > 0.001f)
                    {
                        rot.Y = Mathf.Atan2(dir.X, dir.Z);
                    }

                    state = 1;
					GD.Print("Swapping to Spotted");
				}
				break;

            case 1: //spotted
				
				if(pos.Y < 0.75f)
				{
					pos.Y += 0.15f;
				}
				else
				{
                    pos.Y = 0.75f;
                    state = 2;
                    GD.Print("Swapping to Spotted2");
                }
				break;

            case 2: //spotted2
                if (pos.Y > 0f)
                {
                    pos.Y -= 0.15f;
                }
                else
                {
					pos.Y = 0f;

					timer.WaitTime = 0.25f;
					timer.Start();
                    state = 3;
                    GD.Print("Swapping to SpottedDelay");
                }
                break;

            case 3:
                if (timer.IsStopped())
                {
                    state = 4;
                    GD.Print("Swapping to Chasing");
                }

                break;

            case 4: // Chasing
				dir = player.Position - pos;
				dir.Y = 0f;

				if (dir.Length() > 0.001f)
				{
					rot.Y = Mathf.Atan2(dir.X, dir.Z);
				}

				Vector2 pos2D = new Vector2(pos.X, pos.Z);
				pos2D = pos2D.MoveToward(new Vector2(player.Position.X, player.Position.Z), Speed * (float)delta);
				pos = new Vector3(pos2D.X, 0f, pos2D.Y);


                if (Position.DistanceTo(player.Position) > detectionDistance * 1.25f)
                {
					timer.WaitTime = 1f;
					timer.Start();
                    state = 5;
                    GD.Print("Swapping to Confusion");
                }
                break;

			case 5: // Confused
				if (timer.IsStopped())
				{
					state = 0;
                    GD.Print("Swapping to Idle");
                }
				
				break;
			case 6: // Stomped
				if (scal.Y > 0.5f)
				{
					scal.Y -= scal.Y / 10f;
                }
				else
				{
					timer.WaitTime = 2f;
					timer.Start();
					state = 7; 
				}
				break;
			case 7: // Ded
                if (timer.IsStopped())
				{
					Visible = false;
                    timer.WaitTime = 3f;
                    timer.Start();
                    state = 8; 
				}
				break;
			case 8: // Erased out of existance
                if (timer.IsStopped())
                {
					float rngX;
					float rngZ;


                    if (rng.Randf() > 0.5f)
					{
						rngX = rng.RandfRange(2.5f, 5f);
					}
					else
					{
						rngX = rng.RandfRange(-2.5f, -5f);
					}

                    if (rng.Randf() > 0.5f)
                    {
                        rngZ = rng.RandfRange(2.5f, 5f);
                    }
                    else
                    {
                        rngZ = rng.RandfRange(-2.5f, -5f);
                    }

					pos = new Vector3(pos.X + rngX, 0, pos.Z + rngZ);

                    Visible = true;
                    state = 9;
                }
                break;
            case 9: // Spawn
                if (scal.Y < 1f)
                {
                    scal.Y += scal.Y / 10f;
                }
                else
				{
					scal.Y = 1f;
                    state = 0;
                }
                break;
			case 10:
				if (timer.IsStopped())
				{
					state = 0;
				}
				break;
        }

		Scale = scal;
		Rotation = rot;
		Position = pos;
	}

	private void OnDamageEntered(Node3D body)
	{
		if (body.IsInGroup("Player") && state < 6)
		{
			timer.WaitTime = 2f;
			timer.Start();
			player.Call("OnHit");
			state = 10;
		}
	}

    private void OnKillEntered(Node3D body)
    {
        if (body.IsInGroup("Player") && state < 6)
        {
            state = 6; //Squish
        }
    }
}
