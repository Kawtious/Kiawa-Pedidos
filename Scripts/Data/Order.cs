using Godot;
using Array = Godot.Collections.Array;

public class Order : Node
{

    private string _Key = "";

    private string _Date = "";

    private string _User = "";

    private Array _Dishes = new Array();

    [Export]
    public string Key
    {
        get { return _Key; }
        set { _Key = value; SetKey(_Key); }
    }

    [Export]
    public string Date
    {
        get { return _Date; }
        set { _Date = value; SetDate(_Date); }
    }

    [Export]
    public string User
    {
        get { return _User; }
        set { _User = value; SetUser(_User); }
    }

    [Export]
    public Array Dishes
    {
        get { return _Dishes; }
        set { _Dishes = value; SetDishes(_Dishes); }
    }

    [Signal]
    delegate void UpdatedKey(string value);

    [Signal]
    delegate void UpdatedDate(string value);

    [Signal]
    delegate void UpdatedUser(string value);

    [Signal]
    delegate void UpdatedDishes(Array value);

    public void SetKey(string value)
    {
        EmitSignal("UpdatedKey", value);
    }

    public void SetDate(string value)
    {
        EmitSignal("UpdatedDate", value);
    }

    public void SetUser(string value)
    {
        EmitSignal("UpdatedUser", value);
    }

    public void SetDishes(Array value)
    {
        EmitSignal("UpdatedDishes", value);
    }
}
