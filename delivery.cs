// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Definition of Delivery class. 
// Last revision on 3/13/2022

using System;
using System.Collections.Generic;

/*1. Class invariant for delivery:
• there is unbounded capacity for box array which holds an assortment of produce for delivery
• the error response: exits the function with return;
2. Interface invariant:
• no access to take and modify individual items
• For the input values, if customer is unable to pay or if storage has 0 or less size then delivery will be set to invalid
Public Functions:
• clear() discards all contents inside the box, returning all of the quantities of all the items in the box back to storage
• ForecastDelivery schedules what items will eventually be delivered to the customer
• FillBox adds the appropriate number of produce items to a delivery.
• ReplaceItem replaces a produce item with a different produce item.
• DeliverBox determines if an item needs to be replaced.
• ShareOrder provides information to display all the produce contents in a delivery. If delivery is invalid or there isn't enough produce items to make the order or if there is no produce item in the box,then exit the function with the appropriate message*/

namespace P5
{
    public class Delivery
    {
        private ICustomer customer;
        private EmployeeCustomer emp_customer;
        private bool for_employee = false;

        private int storage_size;
        private int order_size;
        private int box_size;

        private Produce[] storage;
        private Produce[] box;

        private bool delivering;
        private bool cancelled;
        private bool valid = true;
        private bool enough;
        private int getItem = 0;
        private double totalprice;  

        //constructor injection
        public Delivery(ICustomer d_customer, Produce[] inventory, int inventory_size )
        {
            if (inventory == null || inventory_size < 2)
                valid = false;
            customer = d_customer;

            storage_size = inventory_size;
            order_size = 0;
            box_size = 10; //default box capacity

            box = new Produce[box_size];
            storage = inventory;

            delivering = false;
            cancelled = false;
            enough = true;
            totalprice = 0;   
        }
        public Delivery(EmployeeCustomer d_customer, Produce[] inventory, int inventory_size)
        {
            if (inventory == null || inventory_size < 2)
                valid = false;
            emp_customer = d_customer;
            for_employee = true;

            storage_size = inventory_size;
            order_size = 0;
            box_size = 10; //default box capacity

            box = new Produce[box_size];
            storage = inventory;

            delivering = false;
            cancelled = false;
            enough = true;
            totalprice = 0;
        }

        //Post Condition expands the size of the box
        private void expand()
        {
            box_size *= 2;
            Produce[] newArr = new Produce[box_size];
            for (int i = 0; i < box_size / 2; i++)
            {
                newArr[i] = box[i].DeepCopy();
            }
            box = newArr;
        }

        //Post Condition: the amount of produce in the box is now 0
        public bool clear()
        {
            if (!valid)
                return false;
            //put all quantity of all items in box back into storage
            for(int n=0; n<order_size; n++)
            {
                for (int i = 0; i < storage_size; i++)
                {
                    if (box[n].get_name() == storage[i].get_name())
                    {
                        while (box[n].get_qty() > 0)
                        {
                            storage[i].add_qty();
                            box[n].minus_qty();
                        }
                        break;
                    }
                }
            }
            order_size = 0;
            return true;
        }
        
        //Post condition: returns the number of produce item in storage that is sellable
        private int count_sellable_item()
        {
            int count = 0;
            for (int i = 0; i < storage_size; i++)
            {
                if (!storage[i].notSellable())
                {
                    count++;
                }
            }
            return count;
        }
        //Post condition: returns true if there is enough, otherwise returns false
        private bool enough_produce_for_order()
        {
            if (count_sellable_item() > 1) // There must be at least two sellable produce item in storage for an assortment delivery
            {
                double estimate = 0;
                for (int i = 0; i < storage_size; i++)
                {
                    if (!storage[i].notSellable())
                        estimate += storage[i].get_qty() * storage[i].get_cost();
                }
                if (for_employee && estimate > emp_customer.ec.get_MinOrderPrice())
                {
                    return true;
                }
                else if (estimate > customer.get_MinOrderPrice())
                {
                    return true;
                }
            }
            enough = false;
            return false;
        }

        //PreCondition takes in an index of box that needs to be checked with the excluded items
        //Post Condition returns true if that item is one of the excluded items
        private bool check_if_excluded(int index, string[] exclusion)
        {

            for (int j = 0; j < exclusion.Length; j++)
            {
                if (storage[index].get_name() == exclusion[j] || storage[index].get_classification() == exclusion[j])
                    return true;
            }
            return false;
        }
        
        //Precondition:
        // produce passed into the function is a produce that exists in the box array
        // produce passed into the function must have a quantity>0
        //Post condition:
        // sets the quantity of the produce item to a random quantity( or adds to the quantity of it that is already in the box)
        // subtract that same random quantity from the produce in the storage array
        // calculates the cost to add that quantity of produce item and adds it to the total price
        private bool generate_quantity(Produce p)
        {            
            for (int i = 0; i < storage_size; i++)
            {
                if (box[order_size].get_name() == storage[i].get_name())
                {
                    if (storage[i].get_qty() > 1)
                    {
                        storage[i].minus_qty();
                        box[order_size].add_qty();
                    }
                    else
                        return false;
                    break;
                }
            }
            totalprice += box[order_size].get_cost();
            return true;
        }
        
        //Precondition: the produce passed into the function is a produce from storage
        //Post condition: box now has the produce passed into the function and the amount of produce in the box is incremented
        private void add(Produce prod)
        {
            if (order_size == box_size)
                expand();
            box[order_size] = prod.DeepCopy();
            generate_quantity(box[order_size]);
            order_size++;
        }
        //Post Condition:
        //If every available item is an excluded item, then the function would set the enough bool to false and the function returns false
        //if it finds an item that is not excluded and is sellable, then the function returns true
        private bool generate_index()
        {
            bool again = false;
            int count = 0; //keeps track of the number of items that we did not choose
            do
            {
                if (count >= storage_size) //if we have checked every item in storage and they are all unsellable items or they are to be excluded
                {
                    enough = false;
                    return false;
                }
                getItem = (getItem + 1) % storage_size;
                if (storage[getItem].notSellable())
                    again = true;
                if (for_employee && emp_customer.ec.GetType() != typeof(Customer))
                {
                    if (check_if_excluded(getItem, emp_customer.ec.get_Exclusions()))
                    {
                        count++;
                        again = true;
                    }
                }
                else if (customer.GetType() == typeof(PickyCustomer) || customer.GetType() == typeof(SegmentCustomer))
                {
                    if (check_if_excluded(getItem, customer.get_Exclusions()))
                    {
                        count++;
                        again = true;
                    }
                }

            } while (again);
            return true;
        }
        //Precondition: delivery must be valid, there must be enough produce items in storage and minimum order price has not been met
        //Post condition: if it doesn't meet the pre condition, return false
        //otherwise, call add to add an assortment of sellable produce items to the box and return true when done or false if it cannot add anymore items
        public bool ForecastDelivery()
        {
            if (!valid||!enough_produce_for_order())
            {
                return false;
            }

            int assortment = 0;
            if (count_sellable_item() < 11)
            {
                assortment = 2;
            }
            else
            {
                assortment = count_sellable_item() % 11;
                if (assortment < 2)
                    assortment += 2;
            }

            for (int i = 0; i < assortment; i++)
            {
                if (generate_index())
                    add(storage[getItem]); //getItem is the index that was found in generate_index
                else
                    return false;
            }
            return true;
        }

        //Precondition: delivery must be valid, there must be enough produce items to make the order and there must be atleast one produce item in the box
        //Post condition: if the pre condition is not met, then return false.
        //Otherwise, for each produce in the box, add quantity to it until the total price is more or equal to the minimum order price
        //if there isn't enough quantity to add, return false, otherwise, true
        public bool FillBox()
        {
            if (!valid || !enough || order_size == 0)
            {
                return false;
            }
            int count = 0; //count how many items in the box that has qty<1 in storage
            for (int i = 0; i < order_size; i++)
            {
                if (for_employee)
                {
                    while (totalprice < emp_customer.ec.get_MinOrderPrice() && count < order_size) 
                    {
                        if (!generate_quantity(box[i]))
                            count++;
                    }
                }
                else
                {
                    while (totalprice < customer.get_MinOrderPrice() && count < order_size) 
                    {
                        if (!generate_quantity(box[i]))
                            count++;
                    }
                }            
            }
            if (for_employee)
            {
                if (totalprice < emp_customer.ec.get_MinOrderPrice())
                {
                    enough = false;
                    return false;
                }
            }
            else
            {
                if (totalprice < customer.get_MinOrderPrice())
                {
                    enough = false;
                    return false;
                }
            }
            return true;
        }

        //Precondition:
        // gets index of box that needs to be replaced
        // delivery must be valid, there must be enough produce items to make the order and there must be atleast one produce item in the box
        //Post condition:
        //if we cannot get another produce to replace the item then return false
        // otherwise the produce will have been taken out of the box and a new produce will have been put in its place, then return true
        public bool ReplaceItem(int n)
        {
            if (!valid || !enough || order_size == 0)
            {
                return false;
            }
            if (generate_index()) //get a different produce from storage to replace the item taken out
            {
                //take cost out of total price and put the qty back into storage
                totalprice -= (box[n].get_cost() * box[n].get_qty());
                for (int i = 0; i < storage_size; i++)
                {
                    if (box[n].get_name() == storage[i].get_name())
                    {
                        while(box[n].get_qty() > 0)
                        {
                            storage[i].add_qty();
                            box[n].minus_qty();
                        }
                    }
                }
                //put into box
                box[n] = storage[getItem].DeepCopy();
                generate_quantity(box[n]);
                FillBox();
                return true;
            }
            return false;
        }

        //Pre condition: delivery must be valid, there must be enough produce items to make the order and there must be atleast one produce item in the box
        //Post condition:
        // If pre condition is not met then return false.
        // all the produce items inside the box should be fresh and ready to be delivered and return true
        public bool DeliverBox()
        {
            if (!valid || !enough || order_size == 0)
            {
                return false;
            }
            for (int i = 0; i < order_size; i++)
            {
                if (box[i].notSellable())
                {
                    if (!ReplaceItem(i))
                    {
                        return false;
                    }
                }
            }
            if (for_employee)
            {
                if (emp_customer.ec.pay(totalprice))
                {
                    delivering = true;
                    return delivering;
                }
                else
                {
                    cancelled = true;
                    return cancelled;
                }
            }
            else
            {
                if (customer.pay(totalprice))
                {
                    delivering = true;
                    return delivering;
                }
                else
                {
                    cancelled = true;
                    return cancelled;
                }
            }
        }
        //PostCondition: Returns necessary info about the delivery or the appropriate message for invalid delivery object
        public string ShareOrder()
        {
            if(!valid)
            {
                return "Invalid input for Delivery\n";
            }
            string display = "Ship to ";
            if (for_employee)
            {
                display += emp_customer.ec.get_name() + "\nAddress: " + emp_customer.ec.get_address() + "\n";
                if (emp_customer.ec.GetType() != typeof(Customer) && emp_customer.ec.get_Exclusions().Length != 0)
                {
                    display += "\nExclusions for the order:\n";
                    for (int i = 0; i < emp_customer.ec.get_Exclusions().Length; i++)
                    {
                        display += emp_customer.ec.get_Exclusions()[i] + "\n";
                    }
                }
            }
            else
            {
                display += customer.get_name() + "\n" + customer.get_address() + "\n";
                if (customer.GetType() != typeof(Customer) && customer.get_Exclusions().Length != 0)
                {
                    display += "\nExclusions for the order:\n";
                    for (int i = 0; i < customer.get_Exclusions().Length; i++)
                    {
                        display += customer.get_Exclusions()[i] + "\n";
                    }
                }
            }
            if (!enough)
            {
                display += "\nInsufficient produce for order\n";
            }
            else if (cancelled)
            {
                display += "\nDelivery has been cancelled\n";
            }
            else if (delivering)
            {
                display += "\nStatus: Your order is on the way\n";
                for (int i = 0; i < order_size; i++)
                {
                    display += box[i].query() + "\nTotal price is " + totalprice + "\n";
                }
            }
            else
            {
                display+="\nStatus: Your order is being prepared\n";
            }
            return display;
        }
    }
}
/*3. Implementation invariant:
• Overoaded constructors, one for ICustomer object, another for EmployeeCustomer object
• The Delivery object will be in an invalid state if customer or employee customer passes into delivery is null or storage size is less than 2 because there must be atleast 2 items in storage for an assortment delivery.
    If customer or employee customer who places the order is invalid, they will not be able to pay for the delivery when DeliverBox() is called
• 'enough' bool will be set to false if:
    -there is less than two sellable items in storage
    -the amount of sellable items in storage all together is not enough to meet the minimum order price 
    -cannot generate a storage index because every item in storage is to be excluded or is not sellable
    -with the chosen items from ForecastDelivery(), we cannot generate enough quantity in FillBox() to meet the minimum order price
• The error response for the methods are returning false or, for ShareOrder(), displaying the appropriate message

Public Functions:
• clear() discards all contents inside the box, returning all of the quantities of all the items in the box back to storage. 
    For efficiency, heap memory is not released in clear().
• ForecastDelivery() checks if Delivery object is valid and whether there is enough produce in storage to make a delivery. 
    If both are true, it decides on the number of assortment for the delivery. 
    Then it adds that many items to box by first calling generate_index to make sure that we can generate an idex of an item in storage that can be considered for delivery. 
    If we can generate an index, add that produce item at that index in storage to the box. Once enough items have been added to the box (based on number of assortment), return true. 
    If we cannot add enough items to the box (based on number of assortments) because we cannot generate an index, return false. 
• FillBox() checks if Delivery object is valid, if there is enough produce in storage to make the delivery, and if there is atleast one item in the box (order_size must be larger than 0). 
    If all are true, it keeps on adding a quantity for each item in the box until totalprice reaches the minimum order price. 
    If, with the items in the box, we cannot generate enough quantity to meet the minimum order price, then set enough bool to false and return false.
• ReplaceItem(int) checks if Delivery object is valid and if there is enough produce in storage to make the delivery. 
    If both are true, it replaces a produce item in the box (index is passed through the parameter) with a different produce item in storage.
    If we cannot find a replacement because we cannot generate a storage index, then return false.
    Otherwise, take out the item that needs replacing (qty and price from total price), then place it back into storage.
    Next, add the item at that generated index in storage to the box and generate quantity for it. Call FillBox() to make sure that order still meets the minimum order price, and add more quantity if needed.
• DeliverBox() checks if a delivery object is valid, if there is enough order in storage to make the order, and if there is atleast one item in the box (order_size must be larger than 0). 
    If all are true, it determines if there is an unsellable item that needs to be replaced.
    If there is an item that needs to be replaced and we cannot find a replacement, return false.
    If every item in the box is good, let the customer or employeecustomer pay for the delivery by calling pay method.
    When delivery is paid for, ship the box by setting the delivering bool to true.
    If they cannot pay for the delivery, cancel the delivery by setting the cancelled bool to true.
• ShareOrder() provides information to display all the produce contents in a delivery. 
    If delivery is invalid, exit the function with the appropriate message
    Otherwise, display the name and address of the customer or employee customer, and the exclusions if they have any.
    If there isn't enough produce items to make the order or if the delivery is cancelled or if the delivery has shipped, provide the appropriate message

 Private Functions:
• expand resizes box array if the order_size reaches its capacity which is box_size. When resizing the box array, the box_size is doubled
• count_sellable_item counts the number of items that are not spoiled, expired or has less than 1 quantity in storage
• enough_produce_for_order checks if there is enough produce items in storage to make the order
• check_if_excluded checks if an item is one of the excluded items specified by the customer
• generate_quantity generates a random quantity of the produce item passed through the parameter
• generate_index looks for an item in storage that can be added into the box, prioritizing the oldest produce.
• add puts a produce item in storage into the box, if the box is full, call the expand function
 <<Note: See public function description in Interface Invariant at the top of the file>>
 */



