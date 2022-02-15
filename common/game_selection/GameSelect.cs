using Godot;
using GodotLogger;
using System;

public class GameSelect : Node
{
    private static readonly Logger _log = LoggerHelper.GetLogger(typeof(GameSelect));

    private TextureButton mbj;
    private TextureButton txp;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mbj = GetNode<TextureButton>("Layout/Icons/ItemList/MagicBlackJack");
        txp = GetNode<TextureButton>("Layout/Icons/ItemList/TexasPoker");

        var mbjArgs = new Godot.Collections.Array();
        mbjArgs.Add("mbj");

        var txpArgs = new Godot.Collections.Array();
        txpArgs.Add("txp");

        mbj.Connect("pressed", this, nameof(_OnGameSelected), mbjArgs);
        txp.Connect("pressed", this, nameof(_OnGameSelected), txpArgs);       
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    private void _OnGameSelected(string gameName)
    {
        string scenePath = null;

        switch (gameName)
        {
            case "mbj":
                scenePath = "res://games/blackjack/halls/halls.tscn";
                break;

            case "txp":
                scenePath = "res://games/texasPoker/halls/halls.tscn";
                break;
        }

        if (scenePath != null)
        {
            _log.Info($"change scene for game: {gameName} to: {scenePath}");

            var scene = ResourceLoader.Load<PackedScene>(scenePath);
            GetTree().ChangeSceneTo(scene);
        }
        else
        {
            _log.Warn($"cant change scene for un-support game: {gameName}");
        }
    }
}
