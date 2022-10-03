// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Definition of SegmentCustomer class
// Last revision on 3/13/2022

using System;

/*1. Class invariant:
• The class uses constructor injection and 'valid' bool to check for bad inputs
• Balance must be more than minimum order price and exclusions must be at most 3 for a segment customer to be valid for placing an order

2. Interface invariant:
• no access to take and modify individual information of customer
 -client must pass in a positive balance and minimum order price in order for customer to be valid
 -a segment customer must have a balance that is larger than the minimum order price to be valid
 -a segment customer can have at most 3 excluded items, any more than 3 will put the segment customer in an invalid state
 -only if the segment customer is valid can the customer place an order and pay for the order
 -the client is allowed to view everything about a segment customer EXCEPT the balance
 -*/

namespace P5
{
    public class SegmentCustomer : ICustomer
    {
        protected string name;
        protected string address;
        protected double minOrderPrice;
        protected double balance;
        private string[] exclusions = new string[3];
        private bool valid = true;
        public SegmentCustomer() : base() { }

        public SegmentCustomer(string c_name, string c_address, double c_balance, double c_minOrderPrice, string[] c_excl) 
        {
            if (c_balance < 0 || c_minOrderPrice<0 || c_minOrderPrice > c_balance || c_excl.Length > 3) valid = false;
            name = c_name;
            address = c_address;
            balance = c_balance;
            minOrderPrice = c_minOrderPrice;
            exclusions = c_excl;
        }
        public bool pay(double orderPrice)
        {
            if (!valid) return false;
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
            if (!valid) return new string[0];
            return exclusions;
        }
    }
}
/*3. Implementation invariant:
 SegmentCustomer inherits from the interface ICustomer
 • Defines methods from ICustomer including pay, get_name, get_address, get_minOrderPrice, get_Exclusions 
 • The method 'pay' takes the total price calculated in delivery and deducts that amount from the segment customer's balance, the function is called in delivery after a delivery is shipped. 'pay' will return false if segment customer is not valid (due to errorneous numbers for minimum order price and balance or insufficient balance) and no amount will be deducted from balance 
 • All getters are meant to assist the functionality of Delivery*/
