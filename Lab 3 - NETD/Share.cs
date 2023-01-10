/*
 * Name: Shivam Janda
 * Date: November 11, 2022
 * Description: Parent class with its data members, constructors, accessors and mutators
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3___NETD
{
    public class Share // parent class
    {
        // Data members
        string buyerName;
        string buyDate;

        // Deafult constructor
        public Share()
        {
            buyerName = "";
            buyDate = "";
        }

        // Parameterized constructor
        public Share(string name, string date)
        {
            buyerName = name;
            buyDate = date;
        }

        // Accessors and mutators
        public string BuyerName
        {
            get { return this.buyerName; }
            set { this.buyerName = value; }
        }

        public string BuyDate
        {
            get { return this.buyDate; }
            set { this.buyDate = value; }
        }

    }
}
