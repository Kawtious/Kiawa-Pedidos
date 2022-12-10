using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;
using System.Text.RegularExpressions;
using System.Collections;

public class ScreenCreateDish : Control
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private Firebase Firebase;

    private Control BoxLeft;

    private VBoxContainer ContainerVBoxLeft;

    private LineEdit LineEditTitle;

    private LineEdit LineEditDescription;

    private LineEdit LineEditPrice;

    private LineEdit LineEditPortions;

    private Control BoxRight;

    private ScrollContainer ScrollBox;

    private VBoxContainer ContainerVBoxRight;

    private void InitNodes()
    {
        Firebase = GetNode<Firebase>("/root/Firebase");

        BoxLeft = GetNode<Control>("BoxLeft");
        ContainerVBoxLeft = BoxLeft.GetNode<VBoxContainer>("ContainerVBoxLeft");

        BoxRight = GetNode<Control>("BoxRight");
        ScrollBox = BoxRight.GetNode<ScrollContainer>("ScrollBox");
        ContainerVBoxRight = ScrollBox.GetNode<VBoxContainer>("ContainerVBoxRight");

        LineEditTitle = ContainerVBoxLeft.GetNode<LineEdit>("ContainerTitle/NinePatchRect/LineEdit");
        LineEditDescription = ContainerVBoxLeft.GetNode<LineEdit>("ContainerDescription/NinePatchRect/LineEdit");
        LineEditPrice = ContainerVBoxLeft.GetNode<LineEdit>("ContainerPrice/NinePatchRect/LineEdit");
        LineEditPortions = ContainerVBoxLeft.GetNode<LineEdit>("ContainerPortion/NinePatchRect/LineEdit");
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");
    }

    public void UpdateData()
    {
        UpdateDishes();
    }

    private void UpdateDishes()
    {
        ClearDishList();

        if (Firebase.Dishes == null)
        {
            return;
        }

        foreach (DictionaryEntry entry in Firebase.Dishes)
        {
            Dictionary map = entry.Value as Dictionary;

            Dish dish = Dish.FromMap(map);
            dish.Key = (string)entry.Key;

            ContainerDishEditable container = ContainerDishEditable.CreateContainerDishEditable(ContainerVBoxRight, dish);
            container.Connect("DishSelected", this, "UpdateTextFields");
            container.Connect("DishDeselected", this, "ResetFields");
        }
    }

    public void ResetFields()
    {
        LineEditTitle.Text = "";
        LineEditDescription.Text = "";
        LineEditPrice.Text = "";
        LineEditPortions.Text = "";
    }

    public void UpdateFields(ContainerDishEditable container)
    {
        LineEditTitle.Text = container.Dish.Title;
        LineEditDescription.Text = container.Dish.Description;
        LineEditPrice.Text = container.Dish.Price.ToString();
        LineEditPortions.Text = container.Dish.Portions.ToString();
    }

    private void ClearDishList()
    {
        foreach (ContainerDishEditable dishContainer in ContainerVBoxRight.GetChildren())
        {
            ContainerVBoxRight.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private string GetSelectedDish()
    {
        string selectedDishKey = "";

        foreach (ContainerDishEditable dishContainer in ContainerVBoxRight.GetChildren())
        {
            CheckBox checkbox = dishContainer.GetNode<CheckBox>("CheckBox");

            if (checkbox.Pressed)
            {
                selectedDishKey = dishContainer.Dish.Key;
                break;
            }
        }

        return selectedDishKey;
    }

    public void _OnCreateDishButtonPressed()
    {
        if (LineEditPrice.Text.Empty() || LineEditDescription.Text.Empty() ||
            LineEditPrice.Text.Empty() || LineEditPortions.Text.Empty())
        {
            return;
        }

        string title = LineEditTitle.Text;
        string description = LineEditDescription.Text;
        Single price;
        Single portions;

        if (!Single.TryParse(LineEditPrice.Text, out price))
        {
            return;
        }

        if (!Single.TryParse(LineEditPortions.Text, out portions))
        {
            return;
        }

        Dictionary map = new Dictionary {
            {Dish.TITLE_STRING, title},
            {Dish.DESCRIPTION_STRING, description},
            {Dish.PRICE_STRING, price},
            {Dish.PORTIONS_STRING, portions}
        };

        Dish dish = Dish.FromMap(map);

        if (dish == null)
        {
            return;
        }

        Firebase.CreateDish(GetSelectedDish(), dish);
        ResetFields();
    }
}
