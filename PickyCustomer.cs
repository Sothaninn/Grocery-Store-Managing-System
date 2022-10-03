// Created by Sothaninn Sieng 2/26/2022
// CPSC 3200
// Description: PickyCustomer Class
// Last revision on 3/13/2022

using System;

/*1. Class invariant:
• The class uses constructor injection and 'valid' bool to check for bad inputs
• Balance must be more than minimum order price and exclusions must be at most 5 for a picky customer to be valid for placing an order

2. Interface invariant:
• no access to take and modify individual information of customer
 -client must pass in a positive balance and minimum order price in order for customer to be valid
 -a picky customer must have a balance that is larger than the minimum order price to be valid
 -a picky customer can have at most 5 excluded items, any more than 5 will put the picky customer in an invalid state
 -only if the picky customer is valid can the customer place an order and pay for the order
 -the client is allowed to view everything about a picky customer EXCEPT the balance
 -*/

namespace P5
{
    public class PickyCustomer : ICustomer
    {
        protected string name;
        protected string address;
        protected double minOrderPrice;
        protected double balance;
        private string[] exclusions = new string[5];
        private bool valid = true;

        public PickyCustomer() {}
        public PickyCustomer(string c_name, string c_address, double c_balance, double c_minOrderPrice, string[] c_excl)
        {
            if (c_balance< 0 || c_minOrderPrice<0 || c_minOrderPrice > c_balance || c_excl.Length > 5) valid = false;
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
 PickyCustomer inherits from the interface ICustomer
 • Defines methods from ICustomer including pay, get_name, get_address, get_minOrderPrice, get_Exclusions
 • The method 'pay' takes the total price calculated in delivery and deducts that amount from the pickycustomer's balance, the function is called in delivery after a delivery is shipped. 'pay' will return false if pickycustomer is not valid (due to errorneous numbers for minimum order price and balance or insufficient balance) and no amount will be deducted from balance 
 • All getters are meant to assist the functionality of Delivery*/
