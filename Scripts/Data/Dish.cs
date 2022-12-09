using System;
using Godot;

using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class Dish : Godot.Object
{

    public static readonly string TITLE_STRING = "titulo";

    public static readonly string DESCRIPTION_STRING = "descripcion";

    public static readonly string PRICE_STRING = "precio";

    public static readonly string PORTIONS_STRING = "porciones";

    private string _Key = "";

    private string _Title = "";

    private string _Description = "";

    private Single _Price = 0;

    private Single _Portions = 0;

    public string Key
    {
        get { return _Key; }
        set { _Key = value; }
    }

    public string Title
    {
        get { return _Title; }
        set { _Title = value; }
    }

    public string Description
    {
        get { return _Description; }
        set { _Description = value; }
    }

    public Single Price
    {
        get { return _Price; }
        set { _Price = value; }
    }

    public Single Portions
    {
        get { return _Portions; }
        set { _Portions = value; }
    }

    public Dish() { }

    public Dish(string key, string title, string description, Single price, Single portions)
    {
        this.Key = key;
        this.Title = title;
        this.Description = description;
        this.Price = price;
        this.Portions = portions;
    }

    public Dish(string title, string description, Single price, Single portions)
    {
        this.Title = title;
        this.Description = description;
        this.Price = price;
        this.Portions = portions;
    }

    public Dictionary ToMap()
    {
        Dictionary dictionary = new Dictionary {
            {Dish.TITLE_STRING, Title},
            {Dish.DESCRIPTION_STRING, Description},
            {Dish.PRICE_STRING, Price},
            {Dish.PORTIONS_STRING, Portions}
        };

        return dictionary;
    }

    public static Dish FromMap(Dictionary map)
    {
        if (!IsValidDish(map))
        {
            return null;
        }

        string title = "Invalid title";
        string description = "Invalid description";
        Single price = 0;
        Single portions = 0;

        if (map[TITLE_STRING] is string)
        {
            title = (string)map[TITLE_STRING];
        }

        if (map[DESCRIPTION_STRING] is string)
        {
            description = (string)map[DESCRIPTION_STRING];
        }

        if (map[PRICE_STRING] is Single)
        {
            price = (Single)map[PRICE_STRING];
        }

        if (map[PORTIONS_STRING] is Single)
        {
            portions = (Single)map[PORTIONS_STRING];
        }

        return new Dish(title, description, price, portions);
    }

    public static Dish FromMap(Dictionary map, string key)
    {
        if (!IsValidDish(map))
        {
            return null;
        }

        string title = "Invalid title";
        string description = "Invalid description";
        Single price = 0;
        Single portions = 0;

        if (map[TITLE_STRING] is string)
        {
            title = (string)map[TITLE_STRING];
        }

        if (map[DESCRIPTION_STRING] is string)
        {
            description = (string)map[DESCRIPTION_STRING];
        }

        if (map[PRICE_STRING] is Single)
        {
            price = (Single)map[PRICE_STRING];
        }

        if (map[PORTIONS_STRING] is Single)
        {
            portions = (Single)map[PORTIONS_STRING];
        }

        return new Dish(key, title, description, price, portions);
    }

    private static bool IsValidDish(Dictionary dish)
    {
        return dish == null ||
            (dish.Contains(Dish.TITLE_STRING) && dish.Contains(Dish.DESCRIPTION_STRING) &&
                dish.Contains(Dish.PRICE_STRING) && dish.Contains(Dish.PORTIONS_STRING));
    }

}
