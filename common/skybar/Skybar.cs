using Account;
using Godot;
using System;
using Rx.Net.Plus;

public class Skybar : MarginContainer
{
    private Player player;

    private TextureRect avatar;
    private Label nickname;
    private Label coins;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        avatar = GetNode<TextureRect>("Row/H/Avatar");
        nickname = GetNode<Label>("Row/H/V/Nickname");
        coins = GetNode<Label>("Row/H/V/Coins");

        player = GetNode<Player>("/root/Player");
        player.Profile.Nickname.Notify(value => nickname.Text = value);
        player.DefaultWallet.Amount.Notify(value => coins.Text = value.ToString());
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
