using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class OrderScreen : Control
{

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private Control Box;

    private ScrollContainer BoxScroll;

    private VBoxContainer BoxVBox;

    private AnimationTree AnimationTree;

    private AnimationNodeStateMachinePlayback AnimationState;

    private string _Query = "";

    [Export]
    public string Query
    {
        get { return _Query; }
        set { _Query = value; UpdateDayMenu(); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        ConnectSignals();
    }

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        Box = GetNode<Control>("Box");
        BoxScroll = Box.GetNode<ScrollContainer>("BoxScroll");
        BoxVBox = BoxScroll.GetNode<VBoxContainer>("BoxVBox");

        AnimationTree = GetNode<AnimationTree>("Animations/AnimationTree3");
        AnimationState = AnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

        string today = System.DateTime.Now.DayOfWeek.ToString();
        Box.GetNode<Label>("Label").Text = today;
    }

    private void ConnectSignals()
    {
        Firebase.Connect("UpdatedData", this, "UpdateData");
    }

    public void _OnSendOrderButtonPressed()
    {
        Array dishes = new Array();

        foreach (DishContainer dishContainer in BoxVBox.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                dishes.Add(dishContainer.Dish.Key);
            }
        }

        if (dishes.Count < 1)
        {
            return;
        }

        Firebase.SendOrder(dishes);
    }

    public void _OnLineEditTextChanged(string new_text)
    {
        Query = new_text;
    }

    public void UpdateData()
    {
        UpdateDayMenu();
    }

    private void UpdateDayMenu()
    {
        ClearDishList();

        Array dayMenu = Firebase.GetTodayMenu();

        if (dayMenu == null)
        {
            ShowNoDishesError();
            return;
        }

        if (dayMenu.Count < 1)
        {
            ShowNoDishesError();
            return;
        }

        bool hasDishes = false;

        foreach (object element in dayMenu)
        {
            string key = (string)element;

            Dish dish = Firebase.GetDish(key);

            if (dish == null)
            {
                continue;
            }

            hasDishes = true;

            if (Query.Empty())
            {
                DishContainer.CreateDishContainer(BoxVBox, dish);
            }
            else if (dish.Title.ToLower().Contains(Query.ToLower()))
            {
                DishContainer.CreateDishContainer(BoxVBox, dish);
            }
        }

        if (!hasDishes)
        {
            ShowNoDishesError();
        }
        else
        {
            HideNoDishesError();
        }
    }

    private void ClearDishList()
    {
        foreach (DishContainer dishContainer in BoxVBox.GetChildren())
        {
            BoxVBox.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private Array GetDishCheckBoxes()
    {
        Array dishContainers = BoxVBox.GetChildren();
        Array checkBoxes = new Array();

        foreach (DishContainer dishContainer in dishContainers)
        {
            checkBoxes.Add(dishContainer.GetNode<CheckBox>("CheckBox"));
        }

        return checkBoxes;
    }

    private DishContainer GetDishContainer(string key)
    {
        foreach (DishContainer dishContainer in BoxVBox.GetChildren())
        {
            if (dishContainer.Dish.Key.Equals(key))
            {
                return dishContainer;
            }
        }
        return null;
    }

    private bool ShowingError = true;

    public void ShowNoDishesError()
    {
        if (ShowingError)
        {
            return;
        }

        AnimationState.Travel("showing");
        ShowingError = true;
    }

    public void HideNoDishesError()
    {
        if (!ShowingError)
        {
            return;
        }

        AnimationState.Travel("hidden");
        ShowingError = false;
    }
}
