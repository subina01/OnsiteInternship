using Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Order.Domain.Entities;

public class OrderStatus : Enumeration
{
    public static OrderStatus Pending = new(1, nameof(Pending));
    public static OrderStatus Paid = new(2, nameof(Paid));
    public static OrderStatus Shipped = new(3, nameof(Shipped));
    public static OrderStatus Delivered = new(4, nameof(Delivered));
    public static OrderStatus Cancelled = new(5, nameof(Cancelled));

    public OrderStatus(int id, string name) : base(id, name)
    {
    }
}
