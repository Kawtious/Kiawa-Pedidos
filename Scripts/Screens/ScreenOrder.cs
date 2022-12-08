using Godot;
using System;
using Dictionary = Godot.Collections.Dictionary;
using Array = Godot.Collections.Array;

public class ScreenOrder : Control
{

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

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    private Control Box;

    private ScrollContainer BoxScroll;

    private VBoxContainer BoxVBox;

    private HBoxContainer BoxPrice;

    private Label LabelPrice;

    private AnimationTree TreeNotificationNoDishes;

    private AnimationNodeStateMachinePlayback StateNotificationNoDishes;

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        Box = GetNode<Control>("Box");
        BoxScroll = Box.GetNode<ScrollContainer>("BoxScroll");
        BoxVBox = BoxScroll.GetNode<VBoxContainer>("BoxVBox");

        BoxPrice = Box.GetNode<HBoxContainer>("BoxPrice");
        LabelPrice = BoxPrice.GetNode<Label>("LabelPrice");

        TreeNotificationNoDishes = GetNode<AnimationTree>("Animations/TreeNotificationNoDishes");
        StateNotificationNoDishes = TreeNotificationNoDishes.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

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

        foreach (ContainerDish dishContainer in BoxVBox.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                for (int i = 0; i < dishContainer.GetAmount(); i++)
                {
                    dishes.Add(dishContainer.Dish.Key);
                }
            }
        }

        if (dishes.Count < 1)
        {
            return;
        }

        Firebase.OrderSend(dishes);
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

        Array dayMenu = Firebase.MenuGetToday();

        if (dayMenu == null || dayMenu.Count < 1)
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
                ContainerDish containerDish = ContainerDish.CreateDishContainer(BoxVBox, dish);
                containerDish.Connect("AmountChanged", this, "RecalculatePrice");
            }
            else if (dish.Title.ToLower().Contains(Query.ToLower()))
            {
                ContainerDish containerDish = ContainerDish.CreateDishContainer(BoxVBox, dish);
                containerDish.Connect("AmountChanged", this, "RecalculatePrice");
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

        RecalculatePrice();
    }

    public void RecalculatePrice()
    {
        float total = 0;

        foreach (ContainerDish dishContainer in BoxVBox.GetChildren())
        {
            if (dishContainer.GetNode<CheckBox>("CheckBox").Pressed == true)
            {
                for (int i = 0; i < dishContainer.GetAmount(); i++)
                {
                    total += dishContainer.Dish.Price;
                }
            }
        }

        LabelPrice.Text = total.ToString() + "$";
    }

    private void ClearDishList()
    {
        foreach (ContainerDish dishContainer in BoxVBox.GetChildren())
        {
            BoxVBox.RemoveChild(dishContainer);
            dishContainer.QueueFree();
        }
    }

    private Array GetDishCheckBoxes()
    {
        Array dishContainers = BoxVBox.GetChildren();
        Array checkBoxes = new Array();

        foreach (ContainerDish dishContainer in dishContainers)
        {
            checkBoxes.Add(dishContainer.GetNode<CheckBox>("CheckBox"));
        }

        return checkBoxes;
    }

    private ContainerDish GetDishContainer(string key)
    {
        foreach (ContainerDish dishContainer in BoxVBox.GetChildren())
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

        StateNotificationNoDishes.Travel("showing");
        ShowingError = true;
    }

    public void HideNoDishesError()
    {
        if (!ShowingError)
        {
            return;
        }

        StateNotificationNoDishes.Travel("hidden");
        ShowingError = false;
    }
}
