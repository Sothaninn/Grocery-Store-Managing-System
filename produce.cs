// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Definition of Produce class
// Last revision on 3/2/2022

using System;

/*1. Class invariant for delivery:
• class represents a produce at a grocery store. It has storage requirements and can be spoiled if the requirement is not met
or expired if it is passed its expiration date
2. Interface invariant:
• no access to take and modify individual items
• If the input for quantity and price are negative numbers than, Produce will be set to invalid
Public Functions:
• set_stored logs whether an item is stored properly
• set_powerOutage logs whether there is a power outage
• set_unstored_duration logs the duration of when an item is not stored properly
• query returns all the necessary information of a produce
• notSellable checks if an item is sellable based on its quantity, validity, expiration and spoilage
• All getters are meant to assist the functionality of Delivery
• add_qty and minus_qty increments or decrements the quantity of an item
  */

namespace P5
{
    public class Produce
    {
        private string name;
        private string classification; 
        private double qty;
        private string unit;
        private double cost;
        private DateTime expirationDate;
        public enum Prod_storage
        {
            dark, refrigerate, counter
        };
        private Prod_storage storage;
        private bool valid=true;
        private bool spoiled;
        private bool expired;
        private bool stored;
        private bool powerLoss;
        private int unstoredDuration;

        //Preconditions:
        //u should either be in count, bunch or lb
        //s should either be dark, refrigerate or counter
        //
        //Post Conditions:
        //The value of name is now n
        //The value of classification is now c
        //The value of qty is now q
        //The value of unit is now u
        //The value of cost is now price
        //The value of storage is now s
        //The value of expirationDate is now expDate
        public Produce(string n, double q, string u, double price, string c, Prod_storage s, DateTime expDate)
        {
            expired = false;
            spoiled = false;
            stored = true;
            powerLoss = false;
            unstoredDuration = 0;
            if (q < 0 || price < 0)
                valid = false;
            name = n;
            classification = c;
            qty = q;
            unit = u;
            cost = price;
            storage = s;
            expirationDate = expDate;
        }

        public Produce ShallowCopy()
        {
            return (Produce)this.MemberwiseClone();
        }

        public Produce DeepCopy()
        {
            Produce item = (Produce)this.MemberwiseClone();
            item.name = name;
            item.qty = qty;
            item.unit = unit;
            item.cost = cost;
            item.storage = storage;
            item.classification = classification;
            item.expirationDate = expirationDate;

            item.stored = stored;
            item.expired = expired;
            item.spoiled = spoiled;
            item.powerLoss = powerLoss;
            item.unstoredDuration = unstoredDuration;
            item.valid = valid;
            return item;
        }
        //Precondition:
        //If an item is spoiled, it can't be set to unspoiled
        //Postcondition: set spoiled bool of an item to true if:
        //there was a power outage for some period of time (for item that requires refrigeration)
        //it is not stored in a dark place for over 24 hours
        //it is not refrigerated for 60 minutes
        public bool check_if_spoiled()
        {
            if (valid && !spoiled)
            {
                if (stored)
                {
                    if (storage == Prod_storage.refrigerate && powerLoss)
                    {
                        spoiled = true;
                    }
                }
                else
                {
                    if (storage == Prod_storage.dark && unstoredDuration > 24)
                    {
                        spoiled = true;
                    }
                    else if (storage == Prod_storage.refrigerate && unstoredDuration >= 1)
                    {
                        spoiled = true;
                    }
                }
            }
            return spoiled;
        }
        //PostCondition returns true if item is expired or false otherwise
        public bool check_if_expired()
        {
            if(valid && DateTime.Now>expirationDate)
            {
                expired = true;
            }
            return expired;
        }
        public void set_stored(bool s)
        {
            stored = s;
            if (stored == true)
                unstoredDuration = 0;
        }
        public void set_powerLoss(bool l)
        {
            powerLoss = l;
        }
        public void set_unstored_duration(int hr)
        {
            if (hr > 0)
            {
                unstoredDuration = hr;
                stored = false;
            }
        }
        //Post Condition: returns the necessary information of a Produce and the appropriate message if produce is invalid
        public string query()
        {
            if (!valid) return "Invalid Produce Object error";
            string display = "Item Name: " + name + "\nQuantity: " + qty + "\nUnit: " + unit + "\nCost: " + cost + "\nStatus: ";
            if (qty == 0)
                display += "None in stock";
            else if (check_if_spoiled() && check_if_expired())
                display += "spoiled and expired";
            else if (check_if_spoiled())
                display += "spoiled";
            else if (check_if_expired())
                display += "expired";
            else
                display += "fresh";
            return display;
        }
        //PreCondition: Produce must be valid
        //Post Condition: returns true if item is sellable( not spoiled, not expired, stored properly, have at least one quantity in stock)
        public bool notSellable()
        {
            if (valid)
            {
                check_if_spoiled();
                check_if_expired();
                if (spoiled || expired || !stored || qty < 1)
                    return true;
            }
            return false;
        }
        public string get_name()
        {
            return name;
        }
        public string get_classification()
        {
            return classification;
        }
        public double get_cost()
        {
            return cost;
        }
        public double get_qty()
        {
            return qty;
        }
        public void add_qty()
        {
            qty++;
        }
        public void minus_qty()
        {
            qty--;
        }
    }
}
/*3. Implementation invariant:
• When an item is spoiled, it cannot be set to unspoiled, check_if_spoiled() may only set a fresh produce to spoiled
• Supports shallow and deep copy for efficiency
Private Functions:
• check_if_spoiled checks if an item is spoiled. If it is then update the status of the item to spoiled but only consider items that are valid and that do not already have a spoiled status
    -If the item is stored properly, only refrigerated items can become spoiled (when there is a power outage)
    -if the item is not stored properly, then:
        +An item that needs to be kept in the dark will spoil if it is not stored in a dark place for over 24 hours
        +An item that needs to be kept refrigerated will spoil if it is not refrigerated for 1hr
• check_if_expired compares an items's expiration date to the current date. If it is the current date is passed the expiration date, then updates the status of the item to expired but only consider items that are valid
 Public Functions:
• set_stored logs whether an item is stored properly by setting stored to true or false
• set_powerOutage logs whether there is a power outage by setting powerLoss to true or false
• set_unstored_duration: if the integer passed into the method is larger than 0, set unstoredDuration to that integer and set stored to false because if the item has an unstored duration then it means that the item is unstored
• query returns all the necessary information of a produce
• notSellable returns true if an item is spoiled, expired, not stored properly or the quantity is less than 1 because an item must not be in any of these conditions for it to be considered for a delivery order
• All getters are meant to assist the functionality of Delivery
• add_qty and minus_qty increments or decrements the quantity of an item (used in delivery)
 */