// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Definition of EmployeeCustomer class
// Last revision on 3/13/2022

using System;

/*1. Class invariant:
• EmployeeCustomer is a child class of Employee, therefore inherits all public and protected members of Employee
• The class uses constructor injection and 'valid' bool to check if an EmployeeCustomer object called the correct overloaded constructor based on its customer type 

2. Interface invariant:
• no access to take and modify individual information of EmployeeCustomer
 -client must pass in a positive balance and minimum order price in order for an employee customer to be valid
 -an employee customer must have a balance that is larger than the minimum order price to be valid
 -only if employee customer is valid can the employee customer be paid the weekly salary, place an delivery order and pay for the order
 -*/

namespace P5
{
    public class EmployeeCustomer : Employee
    {
        public enum Customer_type
        {
            basic, picky, segment
        };
        private Customer_type c_type;
        public ICustomer ec;
        public EmployeeCustomer(string e_name, double e_balance, Position e_level, Customer_type type, string e_address, double e_minOrderPrice) : base(e_name, e_balance, e_level)
        {
            if (e_balance<0 || e_minOrderPrice<0 || type == Customer_type.picky || type == Customer_type.segment)
            {
                valid = false;
            } 
            else 
            {
                c_type = type;
                ec = new Customer(e_name, e_address, e_balance, e_minOrderPrice);
            }
        }
        public EmployeeCustomer(string e_name, double e_balance, Position e_level, Customer_type type, string e_address, double e_minOrderPrice, string[] e_exclu) : base(e_name, e_balance, e_level)
        {
            if (e_balance < 0 || e_minOrderPrice < 0 || type == Customer_type.basic)
            {
                valid = false;
            }
            else
            {
                c_type = type;
                if (type == Customer_type.picky)
                    ec = new PickyCustomer(e_name, e_address, e_balance, e_minOrderPrice, e_exclu);
                else if (type == Customer_type.segment)
                    ec = new SegmentCustomer(e_name, e_address, e_balance, e_minOrderPrice, e_exclu);
            }
        }
    }
}
/*3. Implementation invariant:
 EmployeeCustomer is a child class of Employee and has an ICustomer delegate 
 Balance can not be negative or else EmployeeCustomer object will be set to invalid
 EmployeeCustomer inherits weeklypay method from Employee so, an EmployeeCustomer will still be paid weekly if the object is valid
 Employee Customer has overloaded constructors, one for instantiating a basic employee customer and another for instantiating a picky or segment employee customer (the signature difference is the latter having an array of exclusions as one of its parameters)
 The overloaded constructors use the ICustomer delegate to hold a Customer or PickyCustomer or SegmentCustomer object
 If 'picky' or 'segment' customer_type is passed into the first overloaded constructor (constructor without array of excluions in the parameter), the EmployeeCustomer Object will be set to invalid
 If 'basic' customer_type is passed into the second overloaded constructor (constructor with array of excluions in the parameter), the EmployeeCustomer Object will be set to invalid
 */
