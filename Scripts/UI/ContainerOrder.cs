using Godot;
using System;

public class ContainerOrder : VBoxContainer
{

    private Order _Order = new Order();

    public Order Order
    {
        get { return _Order; }
        set { _Order = value; UpdateContainer(_Order); }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
    }

    private GlobalProcess GlobalProcess;

    private Firebase Firebase;

    public HBoxContainer Container;

    public VBoxContainer Details;

    public Label LabelUser;

    public Label LabelDate;

    private Label LabelPrice;

    public VBoxContainer ContainerDishes;

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        Container = GetNode<HBoxContainer>("Container");

        Details = Container.GetNode<VBoxContainer>("Details");
        LabelUser = Details.GetNode<Label>("User");
        LabelDate = Details.GetNode<Label>("Date");

        LabelPrice = Container.GetNode<Label>("Price");

        ContainerDishes = GetNode<VBoxContainer>("ContainerDishes");
    }

    private void UpdateContainer(Order value)
    {
        LabelUser.Text = "Ticket-" + value.Ticket;
        LabelDate.Text = GlobalProcess.LocalUnixTime((long)value.Date).ToString();

        foreach (string dishKey in value.Dishes)
        {
            Dish dish = Firebase.GetDish(dishKey);

            if (dish != null)
            {
                ContainerOrderDish containerDish = ContainerOrderDish.CreateOrderDishContainer(ContainerDishes, dish);
                containerDish.Connect("AmountChanged", this, "RecalculatePrice");
            }
        }
    }

    public void RecalculatePrice()
    {
        float total = 0;

        foreach (ContainerOrderDish dishContainer in ContainerDishes.GetChildren())
        {
            if (dishContainer.CheckBox.Pressed == true)
            {
                total += dishContainer.Dish.Price;
            }
        }

        LabelPrice.Text = total.ToString() + "$";
    }

    public static void CreateOrderContainer(Node parent, Order order)
    {
        PackedScene _orderContainer = GD.Load<PackedScene>("res://Scenes/UI/ContainerOrder.tscn");
        ContainerOrder orderContainer = (ContainerOrder)_orderContainer.Instance();
        parent.AddChild(orderContainer);

        orderContainer.Order = order;
    }

    public void _OnTrashButtonPressed()
    {
        Firebase.DeleteOrder(Order.Key);
    }

    public void _OnDetailsClick(InputEvent evt)
    {
        if (evt is InputEventMouseButton mbe && mbe.ButtonIndex == (int)ButtonList.Left && mbe.Pressed)
        {
            ContainerDishes.Visible = !ContainerDishes.Visible;
            Container.GetNode<TextureRect>("RectShow").Visible = ContainerDishes.Visible;
            Container.GetNode<TextureRect>("RectHide").Visible = !ContainerDishes.Visible;
        }
    }
}
