using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;

public interface IObserver
{
    void OnNotify(EventData data);
}

public class EventData
{
    public string EventType;
    public int HP;
    public Vector3 pos;
}
public class Subject
{
    private List<IObserver> observers = new List<IObserver>();
    public void RegisterObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void DeregisterObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void Notify(EventData data)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(data);
        }
    }

}



public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 6.5f;

    private Subject subject = new Subject();

    private int HP = 3;

    [Export]
    private Godot.Timer iFrames;

    [Export]
    private HUD ui;

    [Export]
    private Particles particles;



    public void RegisterObserver(IObserver obs)
    {
        subject.RegisterObserver(obs);
    }

    public void OnHit()
    {
        if (!iFrames.IsStopped())
        {
            return;
        }

        iFrames.Start();
        HP--;

        subject.Notify(new EventData
        {
            EventType = "IHurtie",
            HP = HP,
            pos = Position
        });
    }

    public override void _Ready()
    {
        RegisterObserver(ui);
        RegisterObserver(particles);

    }

    public override void _PhysicsProcess(double delta)
	{
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
}
