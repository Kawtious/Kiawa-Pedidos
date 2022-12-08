using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public class Order : Godot.Object
{

    public static readonly string USER_STRING = "user";

    public static readonly string DATE_STRING = "date";

    public static readonly string DISHES_STRING = "dishes";

    private string _Key = "";

    private string _User = "";

    private string _Date = "";

    private Array _Dishes = new Array();

    public string Key
    {
        get { return _Key; }
        set { _Key = value; }
    }

    public string User
    {
        get { return _User; }
        set { _User = value; }
    }

    public string Date
    {
        get { return _Date; }
        set { _Date = value; }
    }

    public Array Dishes
    {
        get { return _Dishes; }
        set { _Dishes = value; }
    }

    public Order() { }

    public Order(string key, string user, string date, Array dishes)
    {
        this.Key = key;
        this.User = user;
        this.Date = date;
        this.Dishes = dishes;
    }

    public Order(string user, string date, Array dishes)
    {
        this.User = user;
        this.Date = date;
        this.Dishes = dishes;
    }

    public Dictionary ToMap()
    {
        Dictionary dictionary = new Dictionary {
            {Order.USER_STRING, User},
            {Order.DATE_STRING, Date},
            {Order.DISHES_STRING, Dishes}
        };

        return dictionary;
    }

    public static Order FromMap(Dictionary dictionary)
    {
        if (!IsValidOrder(dictionary))
        {
            return null;
        }

        string user = (string)dictionary[Order.USER_STRING];
        string date = (string)dictionary[Order.DATE_STRING];
        Array dishes = dictionary[Order.DISHES_STRING] as Array;

        return new Order(user, date, dishes);
    }

    public static bool IsValidOrder(Dictionary order)
    {
        return order == null || 
            (order.Contains(Order.USER_STRING) && order.Contains(Order.DATE_STRING) && order.Contains(Order.DISHES_STRING));
    }
}
