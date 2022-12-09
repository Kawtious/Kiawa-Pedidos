using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class Firebase : Node
{

    public const string MENU_STRING = "menu";

    public const string DISHES_STRING = "dishes";

    public const string ORDERS_STRING = "orders";

    public const string DATABASE_REFERENCE = "https://kiawa-service-default-rtdb.firebaseio.com/";

    public const string DATA_REFERENCE = DATABASE_REFERENCE + "data/";

    public const string MENU_REFERENCE = DATA_REFERENCE + MENU_STRING + "/";

    public const string DISH_REFERENCE = DATA_REFERENCE + DISHES_STRING + "/";

    public const string ORDER_REFERENCE = DATA_REFERENCE + ORDERS_STRING + "/";

    public const string JSON_EXTENSION = ".json";

    private Dictionary _Data = new Dictionary();

    private Dictionary Data
    {
        get { return _Data; }
        set { _Data = value; ValidateData(); }
    }

    private Dictionary GetMenusInDictionary(Dictionary dictionary) => GetData(dictionary, MENU_STRING) as Dictionary;

    private Dictionary GetDishesInDictionary(Dictionary dictionary) => GetData(dictionary, DISHES_STRING) as Dictionary;

    private Dictionary GetOrdersInDictionary(Dictionary dictionary) => GetData(dictionary, ORDERS_STRING) as Dictionary;

    public Dictionary Menu => GetData(Data, MENU_STRING) as Dictionary;

    public Dictionary Dishes => GetData(Data, DISHES_STRING) as Dictionary;

    public Dictionary Orders => GetData(Data, ORDERS_STRING) as Dictionary;

    public Array GetMenu(string key) => GetData(Menu, key.ToLower()) as Array;

    public Dish GetDish(string key) => Dish.FromMap(GetData(Dishes, key) as Dictionary, key);

    public Order GetOrder(string key) => Order.FromMap(GetData(Orders, key) as Dictionary, key);

    public Array GetMenuToday() => GetMenu(GlobalProcess.Today.DayOfWeek.ToString());

    public void UpdateData() => RequestData.Get(DATA_REFERENCE);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitNodes();
        UpdateData();
        Ping();
    }

    private GlobalProcess GlobalProcess;

    private Timer Timer;

    private Timer TimerPing;

    private RESTRequest RequestData;

    private RESTRequest RequestSetMenu;

    private RESTRequest RequestSendOrder;

    private RESTRequest RequestCreateDish;

    private RESTRequest RequestPing;

    private RESTRequest RequestDeleteOrder;

    private RESTRequest RequestDeleteDish;

    [Signal]
    delegate void UpdatedData();

    [Signal]
    delegate void ValidateMenus();

    [Signal]
    delegate void ValidateDishes();

    [Signal]
    delegate void ValidateOrders();

    [Signal]
    delegate void InvalidateMenus();

    [Signal]
    delegate void InvalidateDishes();

    [Signal]
    delegate void InvalidateOrders();

    [Signal]
    delegate void PingSuccess();

    [Signal]
    delegate void PingTimeout();

    [Signal]
    delegate void NewOrders();

    private void InitNodes()
    {
        GlobalProcess = GetNode<GlobalProcess>("/root/GlobalProcess");

        Timer = GetNode<Timer>("Timer");
        TimerPing = GetNode<Timer>("TimerPing");

        RequestData = GetNode<RESTRequest>("RequestData");
        RequestSetMenu = GetNode<RESTRequest>("RequestSetMenu");
        RequestSendOrder = GetNode<RESTRequest>("RequestSendOrder");
        RequestCreateDish = GetNode<RESTRequest>("RequestCreateDish");
        RequestPing = GetNode<RESTRequest>("RequestPing");
        RequestDeleteOrder = GetNode<RESTRequest>("RequestDeleteOrder");
        RequestDeleteDish = GetNode<RESTRequest>("RequestDeleteDish");
    }

    public void Ping()
    {
        RequestPing.Get(DATABASE_REFERENCE);
        TimerPing.Start(2);
    }

    private ICollection GetData(Dictionary data, string key)
    {
        if (data == null || !data.Contains(key))
        {
            return null;
        }

        return data[key] as ICollection;
    }

    public void SetMenu(string day, Array dishes)
    {
        RequestSetMenu.Put(MENU_REFERENCE + day.ToLower(), dishes);
    }

    public void CreateDish(string key, Dish dish)
    {
        if (!key.Empty())
        {
            RequestCreateDish.Put(DISH_REFERENCE + "/" + key, dish.ToMap());
        }
        else
        {
            RequestCreateDish.Post(DISH_REFERENCE, dish.ToMap());
        }
    }

    public void DeleteDish(string key)
    {
        if (key.Empty())
        {
            return;
        }

        string reference = DISH_REFERENCE + "/" + key;

        RequestDeleteDish.Delete(reference);
    }

    public void SendOrder(Array dishes)
    {
        Order order = new Order(NextTicketNumber(), GlobalProcess.TodayEpoch, dishes);

        RequestSendOrder.Post(ORDER_REFERENCE, order.ToMap());
    }

    private Single NextTicketNumber()
    {
        List<Single> tickets = new List<Single>();

        Dictionary orders = Orders;

        if (!ValidateDictionary(orders))
        {
            return 1;
        }

        foreach (DictionaryEntry item in orders)
        {
            Order order = Order.FromMap(item.Value as Dictionary);
            if (order != null)
            {
                tickets.Add(order.Ticket);
            }
        }

        if (tickets.Count < 1)
        {
            return 1;
        }

        return tickets.Max() + 1;
    }

    public void DeleteOrder(string key)
    {
        if (key.Empty())
        {
            return;
        }

        string reference = ORDER_REFERENCE + "/" + key;

        RequestDeleteOrder.Delete(reference);
    }

    private void CompareOrders(Dictionary oldOrders, Dictionary newOrders)
    {
        if (((oldOrders != null && newOrders != null) && (oldOrders.Count < newOrders.Count))
                || (oldOrders == null && newOrders != null))
        {
            EmitSignal("NewOrders");
        }
    }

    private void ValidateData()
    {
        string menuValidation = ValidateDictionary(GetMenusInDictionary(Data)) ? "ValidateMenus" : "InvalidateMenus";
        string dishValidation = ValidateDictionary(GetDishesInDictionary(Data)) ? "ValidateDishes" : "InvalidateDishes";
        string orderValidation = ValidateDictionary(GetOrdersInDictionary(Data)) ? "ValidateOrders" : "InvalidateOrders";

        EmitSignal(menuValidation);
        EmitSignal(dishValidation);
        EmitSignal(orderValidation);
    }

    private bool ValidateDictionary(Dictionary data)
    {
        return data != null && data.Count > 0;
    }

    public void _OnDataRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        if (body == null)
        {
            return;
        }

        JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));
        object _result = json.Result;

        Dictionary data = new Dictionary();

        if (_result != null && _result is Dictionary)
        {
            data = _result as Dictionary;
        }

        if (Data.ToString().Equals(data.ToString()))
        {
            return;
        }

        CompareOrders(GetOrdersInDictionary(Data), GetOrdersInDictionary(data));

        Data = data;

        EmitSignal("UpdatedData");

        Timer.Start(5);
    }

    public void _OnRequestUpdateData(int result, int response_code, string[] headers, byte[] body)
    {
        UpdateData();
    }

    public void _OnTimerTimeout()
    {
        UpdateData();
    }

    public void _OnPingRequestCompleted(int result, int response_code, string[] headers, byte[] body)
    {
        EmitSignal("PingSuccess");
        Ping();
    }

    public void _OnPingTimerTimeout()
    {
        EmitSignal("PingTimeout");
        Ping();
    }

}
