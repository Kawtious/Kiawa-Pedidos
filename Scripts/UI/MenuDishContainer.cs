using Godot;
using System;

public class MenuDishContainer : HBoxContainer
{

    public Dish Dish;

    public VBoxContainer Details;

    public Label LabelTitle;

    public Label LabelPrice;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
        InitView();
    }

    private void InitNodes()
    {
        Dish = GetNode<Dish>("Dish");
        Details = GetNode<VBoxContainer>("Details");
        LabelTitle = Details.GetNode<Label>("Title");
        LabelPrice = Details.GetNode<Label>("Price");
    }

    private void ConnectSignals()
    {
        Dish.Connect("UpdatedTitle", this, "SetTitle");
        Dish.Connect("UpdatedPrice", this, "SetPrice");
    }

    private void InitView()
    {
        LabelTitle.Text = Dish.Title;
        LabelPrice.Text = Dish.Price.ToString();
    }

    public void SetTitle(string value)
    {
        LabelTitle.Text = value;
    }

    public void SetPrice(float value)
    {
        LabelPrice.Text = value.ToString();
    }
}
