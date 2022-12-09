using System;
using System.Text.RegularExpressions;
using Godot;

public class ContainerDish : HBoxContainer
{

    private Dish _Dish = new Dish();

    private string _Amount = "0";

    public Dish Dish
    {
        get { return _Dish; }
        set { _Dish = value; UpdateContainer(_Dish); }
    }

    public string Amount
    {
        get { return _Amount; }
        set { _Amount = value; SetAmount(_Amount); }
    }

    [Signal]
    delegate void AmountChanged();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    public VBoxContainer Details;

    public Label LabelTitle;

    public Label LabelPrice;

    public SpinBox SpinBox;

    public CheckBox CheckBox;

    private void InitNodes()
    {
        Details = GetNode<VBoxContainer>("Details");
        LabelTitle = Details.GetNode<Label>("Title");
        LabelPrice = Details.GetNode<Label>("Price");
        SpinBox = GetNode<SpinBox>("SpinBox");
        CheckBox = GetNode<CheckBox>("CheckBox");
    }

    private void ConnectSignals()
    {
        SpinBox.GetLineEdit().Connect("text_changed", this, "SetAmount");
    }

    private void SetAmount(string value)
    {
        if (value.Empty())
        {
            _Amount = "0";
            EmitSignal("AmountChanged");
            CheckBox.Pressed = false;
        }
        else
        {
            int amount = 0;
            if (int.TryParse(value, out amount))
            {
                _Amount = amount.ToString();
                EmitSignal("AmountChanged");

                if (amount < 1)
                {
                    CheckBox.Pressed = false;
                }
            }
        }

        SpinBox.GetLineEdit().Text = _Amount;
        SpinBox.GetLineEdit().CaretPosition = _Amount.Length;
    }

    public int GetAmount()
    {
        string amount = SpinBox.GetLineEdit().Text;

        if (amount.Empty())
        {
            return 0;
        }

        int _amount = 0;

        if (int.TryParse(amount, out _amount))
        {
            return _amount;
        }

        return 0;
    }

    private void UpdateContainer(Dish value)
    {
        LabelTitle.Text = value.Title;
        LabelPrice.Text = "$" + value.Price.ToString();
    }

    public static ContainerDish CreateDishContainer(Node parent, Dish dish)
    {
        PackedScene _dishContainer = GD.Load<PackedScene>("res://Scenes/UI/ContainerDish.tscn");
        ContainerDish dishContainer = (ContainerDish)_dishContainer.Instance();
        parent.AddChild(dishContainer);

        dishContainer.Dish = dish;
        return dishContainer;
    }

    public void _OnSpinBoxValueChanged(float value)
    {
        if (value < 1)
        {
            SpinBox.Editable = false;
            Amount = "0";
            CheckBox.Pressed = false;
        }
        else
        {
            Amount = ((int)value).ToString();
        }
    }

    public void _OnCheckBoxToggled(bool button_pressed)
    {
        if (button_pressed)
        {
            SpinBox.Editable = true;
            Amount = "1";
        }
        else
        {
            SpinBox.Editable = false;
            Amount = "0";
        }

        EmitSignal("AmountChanged");
    }
}
