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

    public static Dish FromMap(Dictionary dictionary)
    {
        if (!IsValidDish(dictionary))
        {
            return null;
        }

        string title = (string)dictionary[Dish.TITLE_STRING];
        string description = (string)dictionary[Dish.DESCRIPTION_STRING];
        Single price = (Single)dictionary[Dish.PRICE_STRING];
        Single portions = (Single)dictionary[Dish.PORTIONS_STRING];

        return new Dish(title, description, price, portions);
    }

    private static bool IsValidDish(Dictionary dictionary)
    {
        return dictionary.Contains(Dish.TITLE_STRING) && dictionary.Contains(Dish.DESCRIPTION_STRING) &&
            dictionary.Contains(Dish.PRICE_STRING) && dictionary.Contains(Dish.PORTIONS_STRING);
    }

}
