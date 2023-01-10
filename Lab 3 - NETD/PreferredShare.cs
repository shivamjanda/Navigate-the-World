/*
 * Name: Shivam Janda
 * Date: November 11, 2022
 * Description: child class for share with its data members, constructors, accessors and mutators
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3___NETD
{
    class PreferredShare:Share
    {
        // Required data members
        float sharePrice;
        int votingPower;


        // Deafult constructor
        public PreferredShare()
        {
            sharePrice = 100;
            votingPower = 10;
        }

        // Parameterized constructor
        public PreferredShare(float price, int voting, string name, string date)
        {
            BuyerName = name;
            BuyDate = date;
            sharePrice = price;
            votingPower = voting;
        }

        // Accessors and mutators
        public float SharePrice
        {
            get { return this.sharePrice; }
            set { this.sharePrice = value; }
        }
        public int VotingPower
        {
            get { return this.votingPower; }
            set { this.votingPower = value; }
        }

    }
}
