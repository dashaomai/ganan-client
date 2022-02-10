using Godot;
using System;

public class GameSelect : Node
{
    private TextureButton mbj;
    private TextureButton txp;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mbj = GetNode<TextureButton>("Layout/Icons/ItemList/MagicBlackJack");
        txp = GetNode<TextureButton>("Layout/Icons/ItemList/TexasPoker");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
