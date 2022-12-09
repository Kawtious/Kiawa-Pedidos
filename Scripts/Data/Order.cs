using System;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

public class Order : Godot.Object
{

    public static readonly string TICKET_STRING = "ticket";

    public static readonly string DATE_STRING = "date";

    public static readonly string DISHES_STRING = "dishes";

    private string _Key = "";

    private Single _Ticket = 0;

    private string _Date = "0";

    private Array _Dishes = new Array();

    public string Key
    {
        get { return _Key; }
        set { _Key = value; }
    }

    public Single Ticket
    {
        get { return _Ticket; }
        set { _Ticket = value; }
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

    public Order(string key, Single ticket, string date, Array dishes)
    {
        this.Key = key;
        this.Ticket = ticket;
        this.Date = date;
        this.Dishes = dishes;
    }

    public Order(Single ticket, string date, Array dishes)
    {
        this.Ticket = ticket;
        this.Date = date;
        this.Dishes = dishes;
    }

    public Dictionary ToMap()
    {
        Dictionary dictionary = new Dictionary {
            {Order.TICKET_STRING, Ticket},
            {Order.DATE_STRING, Date},
            {Order.DISHES_STRING, Dishes}
        };

        return dictionary;
    }

    public static Order FromMap(Dictionary map)
    {
        if (!IsValidOrder(map))
        {
            return null;
        }

        Single ticket = 0;
        string date = "0";
        Array dishes = new Array();

        if (map[TICKET_STRING] is Single)
        {
            ticket = (Single)map[TICKET_STRING];
        }

        if (map[DATE_STRING] is string)
        {
            date = (string)map[DATE_STRING];
        }

        if (map[DISHES_STRING] is Array)
        {
            dishes = map[DISHES_STRING] as Array;
        }

        return new Order(ticket, date, dishes);
    }

    public static Order FromMap(Dictionary map, string key)
    {
        if (!IsValidOrder(map))
        {
            return null;
        }

        Single ticket = 0;
        string date = "0";
        Array dishes = new Array();

        if (map[TICKET_STRING] is Single)
        {
            ticket = (Single)map[TICKET_STRING];
        }

        if (map[DATE_STRING] is string)
        {
            date = (string)map[DATE_STRING];
        }

        if (map[DISHES_STRING] is Array)
        {
            dishes = map[DISHES_STRING] as Array;
        }

        return new Order(key, ticket, date, dishes);
    }

    public static bool IsValidOrder(Dictionary order)
    {
        return order == null ||
            (order.Contains(Order.TICKET_STRING) && order.Contains(Order.DATE_STRING) && order.Contains(Order.DISHES_STRING));
    }
}
