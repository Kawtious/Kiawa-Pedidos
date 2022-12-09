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

    public Label LabelTicket;

    public Label LabelDate;

    private Label LabelPrice;

    public VBoxContainer ContainerDishes;

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");
        Firebase = GetNode<Firebase>("/root/Firebase");

        Container = GetNode<HBoxContainer>("Container");

        Details = Container.GetNode<VBoxContainer>("Details");
        LabelTicket = Details.GetNode<Label>("User");
        LabelDate = Details.GetNode<Label>("Date");

        LabelPrice = Container.GetNode<Label>("Price");

        ContainerDishes = GetNode<VBoxContainer>("ContainerDishes");
    }

    private void UpdateContainer(Order order)
    {
        LabelTicket.Text = GetTicketString(order);
        LabelDate.Text = GetDateString(order);

        foreach (string dishKey in order.Dishes)
        {
            Dish dish = Firebase.GetDish(dishKey);

            if (dish != null)
            {
                ContainerOrderDish containerDish = ContainerOrderDish.CreateOrderDishContainer(ContainerDishes, dish);
                containerDish.Connect("AmountChanged", this, "RecalculatePrice");
            }
        }
    }

    private string GetTicketString(Order order)
    {
        float ticket = order.Ticket;
        string ticketString;

        if (ticket <= 0)
        {
            ticketString = "Invalid ticket";
        }
        else
        {
            ticketString = "Ticket-" + ticket;
        }

        return ticketString;
    }

    private string GetDateString(Order order)
    {
        Double dateUnix;

        string dateString = "Invalid date";

        if (!Double.TryParse(order.Date, out dateUnix))
        {
            return dateString;
        }

        if (dateUnix > 0)
        {
            dateString = GlobalProcess.GetDateFromUnixTime(dateUnix).ToString();
        }

        return dateString;
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
