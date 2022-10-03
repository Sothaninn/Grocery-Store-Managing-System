// Created by Sothaninn Sieng on 2/26/2022
// CPSC 3200
// Description: Definition of Employee class
// Last revision on 3/13/2022

using System;

/*1. Class invariant:
• The class uses constructor injection and 'valid' bool to check for bad inputs
• balance must be positive for employee to be valid 

2. Interface invariant:
• no access to take and modify individual information of Employee
 -client must pass in a positive balance in order for employee to be valid
 -only if the employee is valid can an employee be paid the weekly salary
 */

namespace P5
{
    public class Employee
    {
        const double lvl1_payment = 100;
        const double lvl2_payment = 110;
        const double lvl3_payment = 120;
        public enum Position
        {
            level1, level2, level3
        };
        protected string name;
        protected double balance;
        protected Position level;
        protected bool valid = true;

        public Employee() { }
        public Employee (string e_name, double e_balance, Position e_level)
        {
            if (e_balance < 0)
                valid = false;
            name = e_name;
            balance = e_balance;
            level = e_level;
        }
        public bool weekly_pay()
        {
            if (!valid) return false;
            if (level == Position.level1)
                balance += lvl1_payment;
            else if (level == Position.level2)
                balance += lvl2_payment;
            else
                balance += lvl3_payment;
            return true;
        }   
    }
}
/*3. Implementation invariant:
 There are three pay levels established for employees (enum called Position)
 Balance can not be negative or else Employee object will be set to invalid
 • The method 'weekly_pay' puts a weekly payment to an employee's account balance based on their pay level or position (level 1 gets 500, level 2 gets 600 and level 3 gets 700). 'weekly_pay' will return false if employee is not valid (due to errorneous numbers for balance) and no amount will be added to their balance */

