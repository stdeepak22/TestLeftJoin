using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestLeftJoin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IEnumerable<Customer> customers = Db.GetCustomer();
            var orders = Db.GetOrder();
             customers = customers
                 .GroupJoin(orders, c=>c.Id, o=>o.CustomerId, (c,os)=>{
                     return c.MergeOrderDtails(os);
                 })
                .GroupJoin(Db.GetAddress(), c=>c.Id, o=>o.CustomerId, (c,os)=>{
                    return c.MergeAddresDtails(os);
                 });

             Console.WriteLine(customers.Count());

             foreach (var c in customers)
             {
                 Console.WriteLine();                 
                 Console.WriteLine(c.FormatInput(x=>x.Id));
                 Console.WriteLine(c.FormatInput(x => x.Name));
                 Console.WriteLine(c.FormatInput(x => x.OrderCount));
                 Console.WriteLine(c.FormatInput(x => x.AddressCount));
                 Console.WriteLine(c.FormatInput(x => x.TotalSpends));
                 Console.WriteLine(c.FormatInput(x => x.FirstOrderDate));
             }
             Console.ReadLine();
        }

        

    }


    public static  class Db
    {
        public static List<Customer> GetCustomer()
        {
            var list = new List<Customer>();
            list.Add(new Customer { Id = 1, Name = "Deepak" });
            list.Add(new Customer { Id = 2, Name = "Alok" });
            return list;
        }

        public static List<Order> GetOrder()
        {
            var list = new List<Order>();
            list.Add(new Order { CustomerId = 1, OrderDate = Convert.ToDateTime("01-21-2017"), Price = 100, Quantity = 1, Item = "Gas" });
            list.Add(new Order { CustomerId = 1, OrderDate = Convert.ToDateTime("01-21-2017"), Price = 200, Quantity = 1, Item = "Cooker" });
            list.Add(new Order { CustomerId = 1, OrderDate = Convert.ToDateTime("01-22-2017"), Price = 300, Quantity = 1, Item = "Spoon" });
            list.Add(new Order { CustomerId = 1, OrderDate = Convert.ToDateTime("01-22-2017"), Price = 400, Quantity = 1, Item = "Box" });
            list.Add(new Order { CustomerId = 1, OrderDate = Convert.ToDateTime("01-23-2017"), Price = 500, Quantity = 1, Item = "Non Stick Pan" });
            return list;
        }

        public static List<Address> GetAddress()
        {
            var list = new List<Address>();
            list.Add(new Address { CustomerId = 1, City ="Gurgaon" });
            list.Add(new Address { CustomerId = 2, City = "Hydrabad" });
            list.Add(new Address { CustomerId = 2, City = "Noida" });
            return list;
        }
    }

    public static class ExtMethods
    {
        public static string FormatInput<TModel, TValue>(this TModel obj, Expression<Func<TModel, TValue>> func)
        {
            var me = func.Body as MemberExpression;
            return string.Format("{0} - {1}", me.Member.Name, func.Compile().Invoke(obj));
        }

        public static Customer MergeOrderDtails(this Customer c, IEnumerable<Order> os)
        {
            if (os.Any())
            {
                c.OrderCount = os.Count();
                c.TotalSpends = os.Sum(x => x.Price);
                c.FirstOrderDate = os.FirstOrDefault().OrderDate; 
            }

            return c;
        }
        public static Customer MergeAddresDtails(this Customer c, IEnumerable<Address> add)
        {
            if (add.Any())
            {
                c.AddressCount = add.Count();                
            }

            return c;
        }
    }


    public interface IPoco
    {

    }
    public partial class Customer : IPoco
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public partial class Customer : IPoco
    {
        public int OrderCount { get; set; }
        public decimal TotalSpends { get; set; }
        public DateTime FirstOrderDate { get; set; }
    }

    public partial class Customer : IPoco
    {
        public int AddressCount { get; set; }        
    }


    public class Order : IPoco
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Item { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }

    public class Address : IPoco
    {
        public int CustomerId { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }

}
