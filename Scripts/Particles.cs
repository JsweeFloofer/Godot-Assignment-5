using Godot;
using System;

public partial class Particles : CpuParticles3D, IObserver
{
    public void OnNotify(EventData data)
    {
        if (data.EventType != "IHurtie")
        {
            return;
        }

        Position = data.pos;
        Emitting = true;
    }
}
