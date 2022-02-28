using Godot;

public class MobileScrollTextureButton : TextureButton
{
    private bool _justPressed = false;
    private Vector2 _previousPosition;
    protected virtual float distanceThreshold { get; } = 5f;

    [Signal]
    public delegate void ScrollButtonPressed();

    public override void _Ready()
    {
        Connect("gui_input", this, nameof(_OnGuiInput));
    }

    public virtual void _OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch screenTouch)
        {
            if (screenTouch.Pressed)
            {
                _justPressed = true;
                _previousPosition = screenTouch.Position;
            }

            if (!screenTouch.Pressed && _justPressed && screenTouch.Position.DistanceTo(_previousPosition) < distanceThreshold)
            {
                _justPressed = false;
                EmitSignal(nameof(ScrollButtonPressed));
            }
        }
    }
}

