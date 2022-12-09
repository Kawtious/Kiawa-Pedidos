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

    private Double _Date = 0;

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

    public Double Date
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

    public Order(string key, Single ticket, Double date, Array dishes)
    {
        this.Key = key;
        this.Ticket = ticket;
        this.Date = date;
        this.Dishes = dishes;
    }

    public Order(Single ticket, Double date, Array dishes)
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

    public static Order FromMap(Dictionary order)
    {
        if (!IsValidOrder(order))
        {
            return null;
        }

        Single ticket = (Single)order[Order.TICKET_STRING];
        Double date = (Double)((Single)order[Order.DATE_STRING]);
        Array dishes = order[Order.DISHES_STRING] as Array;

        return new Order(ticket, date, dishes);
    }

    public static Order FromMap(Dictionary order, string key)
    {
        if (!IsValidOrder(order))
        {
            return null;
        }

        Single ticket = (Single)order[Order.TICKET_STRING];
        Double date = (Double)((Single)order[Order.DATE_STRING]);
        Array dishes = order[Order.DISHES_STRING] as Array;

        return new Order(key, ticket, date, dishes);
    }

    public static bool IsValidOrder(Dictionary order)
    {
        return order == null ||
            (order.Contains(Order.TICKET_STRING) && order.Contains(Order.DATE_STRING) && order.Contains(Order.DISHES_STRING));
    }
}
