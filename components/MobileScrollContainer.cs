using Godot;
using System;

public class MobileScrollContainer : ScrollContainer
{
    protected bool _swiping = false;
    protected Vector2 _swipeStart;
    protected Vector2 _swipeMouseStart;

    protected Godot.Collections.Array<ulong> _swipeMouseTimes = new Godot.Collections.Array<ulong>();
    protected Godot.Collections.Array<Vector2> _swipeMousePositions = new Godot.Collections.Array<Vector2>();

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.Pressed)
            {
                _swiping = true;
                _swipeStart = new Vector2(ScrollHorizontal, ScrollVertical);
                _swipeMouseStart = mouseButtonEvent.Position;
                _swipeMouseTimes = new Godot.Collections.Array<ulong> { OS.GetTicksMsec() };
                _swipeMousePositions = new Godot.Collections.Array<Vector2> { _swipeMouseStart };
            }
            else
            {
                _swipeMouseTimes.Add(OS.GetTicksMsec());
                _swipeMousePositions.Add(mouseButtonEvent.Position);

                var source = new Vector2(ScrollHorizontal, ScrollVertical);
                int index = _swipeMouseTimes.Count - 1;
                ulong now = OS.GetTicksMsec();
                ulong cutOff = now - 100;

                for (int i = _swipeMouseTimes.Count - 1; i > 0; i--)
                {
                    if (_swipeMouseTimes[i] >= cutOff)
                    {
                        index = i;
                    }
                    else
                    {
                        break;
                    }
                }

                var flickStart = _swipeMousePositions[index];
                var flickDuration = Mathf.Min(.3f, (mouseButtonEvent.Position - flickStart).Length() / 1000f);
                if (flickDuration > 0f)
                {
                    Tween tween = new Tween();
                    AddChild(tween);

                    var delta = mouseButtonEvent.Position - flickStart;
                    var target = source - delta * flickDuration * 15f;

                    tween.InterpolateMethod(this, "set_h_scroll", source.x, target.x, flickDuration, Tween.TransitionType.Linear, Tween.EaseType.Out);
                    tween.InterpolateMethod(this, "set_v_scroll", source.y, target.y, flickDuration, Tween.TransitionType.Linear, Tween.EaseType.Out);
                    tween.InterpolateCallback(tween, flickDuration, "queue_free");
                    tween.Start();
                }

                _swiping = false;
            }
        }
        else if (_swiping && @event is InputEventMouseMotion mouseMotionEvent)
        {
            var delta = mouseMotionEvent.Position - _swipeMouseStart;
            ScrollHorizontal = Mathf.RoundToInt(_swipeStart.x - delta.x);
            ScrollVertical = Mathf.RoundToInt(_swipeStart.y - delta.y);

            _swipeMouseTimes.Add(OS.GetTicksMsec());
            _swipeMousePositions.Add(mouseMotionEvent.Position);
        }
    }
}
