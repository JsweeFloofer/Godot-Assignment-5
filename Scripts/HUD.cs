using Godot;
using System;

public partial class HUD : TextureRect, IObserver
{
    [Export]
    private Texture2D Full;
    [Export]
    private Texture2D Two;
    [Export]
    private Texture2D One;
    [Export]
    private Texture2D None;

    public void OnNotify(EventData data)
    {
        if (data.EventType != "IHurtie")
        {
            return;
        }

        switch (data.HP)
        {
            case 3:
                Texture = Full;
                break;
            case 2:
                Texture = Two;
                break;
            case 1:
                Texture = One;
                break;
            case 0:
                Texture = None;
                break;
        }
    }
}
