using Godot;

public class Dish : Node
{

    private int _Id = -1;

    private string _Title = "";

    private string _Description = "";

    private float _Price = 0;

    private int _Portions = 0;

    [Export]
    public int Id
    {
        get { return _Id; }
        set { _Id = value; SetId(_Id); }
    }

    [Export]
    public string Title
    {
        get { return _Title; }
        set { _Title = value; SetTitle(_Title); }
    }

    [Export]
    public string Description
    {
        get { return _Description; }
        set { _Description = value; SetDescription(_Description); }
    }

    [Export]
    public float Price
    {
        get { return _Price; }
        set { _Price = value; SetPrice(_Price); }
    }

    [Export]
    public int Portions
    {
        get { return _Portions; }
        set { _Portions = value; SetPortions(_Portions); }
    }

    [Signal]
    delegate void UpdatedId(int value);

    [Signal]
    delegate void UpdatedTitle(string value);

    [Signal]
    delegate void UpdatedDescription(string value);

    [Signal]
    delegate void UpdatedPrice(float value);

    [Signal]
    delegate void UpdatedPortions(int value);

    public void SetId(int value)
    {
        EmitSignal("UpdatedId", value);
    }

    public void SetTitle(string value)
    {
        EmitSignal("UpdatedTitle", value);
    }

    public void SetDescription(string value)
    {
        EmitSignal("UpdatedDescription", value);
    }

    public void SetPrice(float value)
    {
        EmitSignal("UpdatedPrice", value);
    }

    public void SetPortions(int value)
    {
        EmitSignal("UpdatedPortions", value);
    }

}
