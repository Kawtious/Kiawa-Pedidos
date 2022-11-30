using Godot;
using System;

public class UserData : Node
{
    private string _Username = "default";

    [Export]
    public string Username
    {
        get { return _Username; }
        set { _Username = value; }
    }
}
