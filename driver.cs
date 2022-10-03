// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Driver Class
// Last revision on 3/13/2022

using System;
using System.Collections.Generic;
using System.IO;

namespace P5
{
    class Driver
    {
        static Random rnd = new Random();
        static DateTime generate_ExpDate(DateTime d)
        {
            int index = rnd.Next(1, 3 + 1);
            switch (index)
            {
                case 1:
                    d.AddMonths(rnd.Next(1, 4 + 1));
                    d.AddDays(rnd.Next(1, 9 + 1));
                    break;
                case 2:
                    d.AddMonths(rnd.Next(1, 4 + 1));
                    break;
                case 3:
                    d.AddDays(rnd.Next(1, 9 + 1));
                    break;
            }
            return d;
        }
        public static Produce[] storage = new Produce[100]; 
        public static int size_storage = 0;
        static void process_file(string path)
        {
            StreamReader reader = new StreamReader(path);
            string input = reader.ReadLine();
            while (input != null)
            {
                string[] data = input.Split("\t"); 
                try
                {
                    Produce prod = new Produce(data[0], int.Parse(data[1]), data[2],
                    double.Parse(data[3]),data[4],
                    (Produce.Prod_storage)Enum.Parse(typeof(Produce.Prod_storage), data[5]),
                    generate_ExpDate(DateTime.Parse(data[6])));
                 
                    storage[size_storage] = prod;
                    size_storage++;
                }
                catch { }
                input = reader.ReadLine();
            }
            reader.Close();
        }
        static void invoke_storage_problem()
        {
            int case_index;
            for (int i = 0; i < size_storage; i++)
            {
                case_index = rnd.Next(1, 4 + 1);
                switch (case_index)
                {
                    case 1:
                        storage[i].set_stored(false);
                        break;
                    case 2:
                        storage[i].set_powerLoss(true);
                        break;
                    case 3:
                        storage[i].set_unstored_duration(74); //74 hrs
                        break;
                    case 4:
                        break;
                }
            }
        }
        static string[] gen_PickyExlcusion()
        {
            int size = rnd.Next(1, 5 + 1);
            string[] exclu = new string[size];
            int rand_storageIndex = rnd.Next(0, size_storage);
            for (int i = 0; i < size; i++)
            {
                rand_storageIndex = (rand_storageIndex + 1) % size_storage;
                exclu[i] = storage[rand_storageIndex].get_name();
            }
            return exclu;
        }

        static string[] gen_SegmentExclusion()
        {
            int size = rnd.Next(1, 3 + 1);
            string[] exclu = new string[size];
            string[] classifications = { "fruit", "fungus", "herb", "veg" };
            int rndIndex = size;
            for (int i = 0; i < size; i++)
            {
                rndIndex = (rndIndex + 1) % classifications.Length;
                exclu[i] = classifications[rndIndex];
            }
            return exclu;
        }
        static ICustomer gen_customer()
        {
            string[] names = new string[] { "Peter", "Jonathon", "Kristin", "Dalila", "John", "Michelle", "Viola", "Sarah", "Nary", "Phillip", "Bob", "Karen", "Joe", "Lilly", "Avery" };
            int case_index = rnd.Next(1, 3 + 1);
            int name_index = rnd.Next(0, 14 + 1);
            double r_balance = rnd.Next(5, 50 + 1);
            double r_MinOrderPrice = 5;
            
            switch (case_index)
            {

                case 1:
                    PickyCustomer cust1 = new PickyCustomer("Picky " + names[name_index], "1234 N Picket ST", r_balance, r_MinOrderPrice, gen_PickyExlcusion());
                    return cust1;
                case 2:
                    SegmentCustomer cust2 = new SegmentCustomer("Segment " + names[name_index], "2345 N Segway ST", r_balance, r_MinOrderPrice, gen_SegmentExclusion());
                    return cust2;                    
                default:
                    Customer cust = new Customer("Basic " + names[name_index], "3456 N Bash ST", r_balance, r_MinOrderPrice);
                    return cust;
            }
        }
        static Employee gen_employee()
        {
            string[] names = new string[] { "Emily", "Joe", "Laura", "Jim", "Jack", "Jill", "Riley", "Cora", "Barbie", "Repunzel" };
            Employee.Position[] pay_level = { Employee.Position.level1, Employee.Position.level2, Employee.Position.level3 };
            
            int case_index = rnd.Next(1, 4 + 1);
            int name_index = rnd.Next(0, 9 + 1);
            int level_index = rnd.Next(0, 2 + 1);
            double r_balance = rnd.Next(5, 50 + 1);
            double r_MinOrderPrice = 5;
            switch (case_index)
            {
                case 1:
                    EmployeeCustomer emp1 = new EmployeeCustomer("Employee " + names[name_index], r_balance, pay_level[level_index], EmployeeCustomer.Customer_type.basic, "4567 E Picket ST", r_MinOrderPrice);
                    return emp1;
                case 2:
                    EmployeeCustomer emp2 = new EmployeeCustomer("Employee " + names[name_index], r_balance, pay_level[level_index], EmployeeCustomer.Customer_type.picky, "5678 E Segment ST", r_MinOrderPrice, gen_PickyExlcusion());
                    return emp2;
                case 3:
                    EmployeeCustomer emp3 = new EmployeeCustomer("Employee " + names[name_index], r_balance, pay_level[level_index], EmployeeCustomer.Customer_type.segment, "6789 E Bash ST", r_MinOrderPrice, gen_SegmentExclusion());
                    return emp3;
                default:
                    Employee emp = new Employee("Employee " + names[name_index], r_balance, pay_level[level_index]);
                    return emp;
            }

        }

        static List<Delivery> orders = new List<Delivery>();
        static int num_orders = rnd.Next(1, 8 + 1);
        static List<Employee> employees = new List<Employee>();
        static List<ICustomer> customers = new List<ICustomer>();
        static void Main(string[] args)
        {
            Console.WriteLine("<<<<<<<<<<This program mimics a delivery service>>>>>>>>>>\n");
            process_file("ProduceListTabDelimited-1.txt");
            invoke_storage_problem();

            for (int i = 0; i < num_orders; i++)
            {
                Console.WriteLine("Delivery " + (i+1) +":\n");
                employees.Add(gen_employee());
                if (employees[i].GetType() == typeof(Employee))
                {
                    customers.Add( gen_customer());
                    orders.Add( new Delivery(customers[i], storage, size_storage));
                }
                else //if (employees[i].GetType() == typeof(EmployeeCustomer))
                {
                    EmployeeCustomer emp_cust = (EmployeeCustomer)employees[i];
                    customers.Add(emp_cust.ec);
                    orders.Add(new Delivery(emp_cust, storage, size_storage));
                }
                customers[i] = gen_customer();
                orders.Add( new Delivery(customers[i], storage, size_storage));
                orders[i].ForecastDelivery();
                orders[i].FillBox();
                orders[i].DeliverBox();
                Console.WriteLine(orders[i].ShareOrder());
                employees[i].weekly_pay();
                Console.WriteLine("..............................\n");
            }
        }
    }
}
