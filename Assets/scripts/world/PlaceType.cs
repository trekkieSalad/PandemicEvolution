
using System;

public enum PlaceType
{
    [Capacity(5)]   // limite de gente que puede haber a la vez (numero a explorar al contagiar)
    [Quantity(25)]
    EducationalCenter,
    [Capacity(20)]
    [Quantity(25)]
    WorkCenter,
    [Capacity(10)]
    [Quantity(25)]
    LeisureZone,
    [Capacity(30)]
    [Quantity(25)]
    MarketPlace,
    [Capacity(15)]
    [Quantity(25)]
    PublicInfrastructure
}

public class CapacityAttribute : Attribute
{
    public int Capacity;

    public CapacityAttribute(int capacity)
    {
        Capacity = capacity;
    }
}

public class QuantityAttribute : Attribute
{
    public int Quantity;

    public QuantityAttribute(int quantity)
    {
        Quantity = quantity;
    }
}