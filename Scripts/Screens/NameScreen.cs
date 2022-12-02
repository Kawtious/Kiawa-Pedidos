using Godot;
using System;

public class NameScreen : Control
{

    private UserData UserData;

    private LineEdit LineEdit;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        LineEdit.Text = UserData.Username;
    }

    private void InitNodes()
    {
        UserData = GetNode<UserData>("/root/UserData");
        LineEdit = GetNode<LineEdit>("Box/Bottom/LineEdit");
    }

    public void _OnLineEditTextChanged(string new_text)
    {
        UserData.Username = new_text;
    }
}
