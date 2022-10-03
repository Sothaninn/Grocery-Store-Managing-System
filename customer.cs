// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Definition of Customer class
// Last revision on 3/13/2022

using System;

/*1. Class invariant:
• The class uses constructor injection and 'valid' bool to check for bad inputs
• Balance must be more than minimum order price for customer to be valid for placing an order

2. Interface invariant:
• no access to take and modify individual information of customer
 -client must pass in a positive balance and minimum order price in order for customer to be valid
 -a customer must have a balance that is larger than the minimum order price to be valid
 -only if the customer is valid can a customer place an order and pay for the order
 -the client is allowed to view everything about a customer EXCEPT the balance
 -*/

namespace P5
{
    public interface ICustomer
    {
        public bool pay(double orderPrice);
        public string get_name();
        public string get_address();
        public double get_MinOrderPrice();
        public string[] get_Exclusions();
    }
    public class Customer : ICustomer
    {
        protected string name;
        protected string address;
        protected double minOrderPrice;
        protected double balance;
        protected bool valid = true;

        public Customer() { }
        public Customer(string c_name, string c_address, double c_balance, double c_minOrderPrice)
        {
            if (c_balance < 0 || c_minOrderPrice<0 || c_minOrderPrice > c_balance) 
                valid = false;
            name = c_name;
            address = c_address;
            balance = c_balance;
            minOrderPrice = c_minOrderPrice;
        }
        public bool pay(double orderPrice)
        {
            if (!valid) 
                return false;
            balance -= orderPrice;
            return true;
        }

        public string get_name()
        {
            return name;
        }

        public string get_address()
        {
            return address;
        }
        public double get_MinOrderPrice()
        {
            return minOrderPrice;
        }
        public string[] get_Exclusions()
        {
            return new string[0];
        }
    }
}
/*3. Implementation invariant:
 ICustomer interface allows for polymorphic delegates

 Customer inherits from the interface ICustomer
 • Defines methods from ICustomer including pay, get_name, get_address, get_minOrderPrice, get_Exclusions 
 • The method 'pay' takes the total price calculated in delivery and deducts that amount from the customer's balance, the function is called in delivery after a delivery is shipped. 'pay' will return false if customer is not valid (due to errorneous numbers for minimum order price and balance or insufficient balance) and no amount will be deducted from balance
 • All getters are meant to assist the functionality of Delivery*/
