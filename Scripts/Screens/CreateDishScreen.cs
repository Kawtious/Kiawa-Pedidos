using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;
using System.Text.RegularExpressions;
using System.Collections;

public class CreateDishScreen : Control
{

    private Firebase Firebase;

    private Control Box;

    private VBoxContainer BoxVBox;

    private LineEdit LineEditTitle;

    private LineEdit LineEditDescription;

    private LineEdit LineEditPrice;

    private LineEdit LineEditPortions;

    private Control BoxDishes;

    private ScrollContainer BoxLeftScroll;

    private VBoxContainer BoxLeftVBox;

    private string _Title = "";

    private string _Description = "";

    private string _Price = "";

    private string _Portions = "";

    public string Title
    {
        get { return _Title; }
        set { SetTitle(value); }
    }

    public string Description
    {
        get { return _Description; }
        set { SetDescription(value); }
    }

    public string Price
    {
        get { return _Price; }
        set { SetPrice(value); }
    }

    public string Portions
    {
        get { return _Portions; }
        set { SetPortions(value); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private void InitNodes()
    {
        Firebase = GetNode<Firebase>("/root/Firebase");

        Box = GetNode<Control>("Box");
        BoxVBox = Box.GetNode<VBoxContainer>("BoxVBox");

        BoxDishes = GetNode<Control>("BoxDishes");
        BoxLeftScroll = BoxDishes.GetNode<ScrollContainer>("BoxLeftScroll");
        BoxLeftVBox = BoxLeftScroll.GetNode<VBoxContainer>("BoxLeftVBox");

        LineEditTitle = BoxVBox.GetNode<LineEdit>("TitleContainer/LineEdit");
        LineEditDescription = BoxVBox.GetNode<LineEdit>("DescriptionContainer/LineEdit");
        LineEditPrice = BoxVBox.GetNode<LineEdit>("PriceContainer/LineEdit");
        LineEditPortions = BoxVBox.GetNode<LineEdit>("PortionContainer/LineEdit");
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");
    }

    public void _OnCreateDishButtonPressed()
    {
        if (!ValidateDish())
        {
            return;
        }

        string title = Title;
        string description = Description;
        float price = float.Parse(Price);
        int portions = Int32.Parse(Portions);

        Dish dish = new Dish(title, description, price, portions);

        Firebase.CreateDish(GetSelectedDish(), dish);

        ResetFields();
    }

    public bool ValidateDish()
    {
        return !Title.Empty() && !Description.Empty() && !Price.Empty() && !Portions.Empty();
    }

    public void _OnTitleTextChanged(string new_text)
    {
        Title = new_text;
    }

    public void _OnDescriptionTextChanged(string new_text)
    {
        Description = new_text;
    }

    public void _OnPortionTextChanged(string new_text)
    {
        Portions = new_text;
    }

    public void _OnPriceTextChanged(string new_text)
    {
        Price = new_text;
    }

    private void SetTitle(string value)
    {
        _Title = value;

        LineEditTitle.Text = _Title;
        LineEditTitle.CaretPosition = _Title.Length;
    }

    private void SetDescription(string value)
    {
        _Description = value;

        LineEditDescription.Text = _Description;
        LineEditDescription.CaretPosition = _Description.Length;
    }

    private void SetPrice(string value)
    {
        string pattern = "^[0-9]*$";
        Regex rgx = new Regex(pattern);

        if (rgx.IsMatch(value))
        {
            _Price = value;
            LineEditPrice.Text = _Price;
            LineEditPrice.CaretPosition = _Price.Length;
        }
        else
        {
            LineEditPrice.Text = _Price;
            LineEditPrice.CaretPosition = _Price.Length;
        }
    }

    private void SetPortions(string value)
    {
        string pattern = "^([0-9]+([.][0-9]*)?|[.][0-9]+)*$";
        Regex rgx = new Regex(pattern);

        if (rgx.IsMatch(value))
        {
            _Portions = value;
            LineEditPortions.Text = _Portions;
            LineEditPortions.CaretPosition = _Portions.Length;
        }
        else
        {
            LineEditPortions.Text = _Portions;
            LineEditPortions.CaretPosition = _Portions.Length;
        }
    }

    public void ResetFields()
    {
        Title = "";
        Description = "";
        Price = "";
        Portions = "";
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

            ContainerDishEditable container = ContainerDishEditable.CreateContainerDishEditable(BoxLeftVBox, dish);
            container.Connect("DishSelected", this, "UpdateTextFields");
            container.Connect("DishDeselected", this, "ResetFields");
        }
    }

    public void UpdateTextFields(ContainerDishEditable container)
    {
        Title = container.Dish.Title;
        Description = container.Dish.Description;
        Price = container.Dish.Price.ToString();
        Portions = container.Dish.Portions.ToString();
    }

    private void ClearDishList()
    {
        foreach (ContainerDishEditable dishContainer in BoxLeftVBox.GetChildren())
        {
            BoxLeftVBox.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private string GetSelectedDish()
    {
        string selectedDishKey = "";

        foreach (ContainerDishEditable dishContainer in BoxLeftVBox.GetChildren())
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
}
