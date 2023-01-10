/*
 * Name: Shivam Janda
 * Date: November 11, 2022
 * Description: Main Window properties and functions
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace Lab_3___NETD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When user clickes create entry, validate inputboxes and if pass through validation call the insertValues function and insert values into database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateEntry_Click(object sender, RoutedEventArgs e)
        {
            int shares;
            string errorMessage = "";
            string radio = "";
            string shareType = "";

            // Buyer name text validation if empty entry
            if (txtName.Text == string.Empty) 
            {
                errorMessage += "Name cannot be empty!";
            }

            // number of shares validation for whole number entry
            if (!int.TryParse(txtNumberOfShares.Text, out shares)) 
            {
                errorMessage += "\nHas to be a whole number for number of shares.";
            }

            // Data purchased validation if empty entry
            if (dpDatePurchased.Text == string.Empty) 
            {
                errorMessage += "\nDate cannot be empty!";
            }

            // if the common radio button is selected then set the text for radio to common and share type to numCommonShares (used for shates database table)
            if (rbCommon.IsChecked == true)
            {
                shareType = "numCommonShares";
                radio = "Common";
                
            }
            // if the preffered radio button is selected then set the text for radio to common and share type to numPreferredShares (used for shates database table)
            else if (rbPreferred.IsChecked == true)
            {
                shareType = "numPreferredShares";
                radio = "Preferred";
                
            }
            // validate if the user has something selected
            else 
            {
                errorMessage += "\nYou have to select a share type.";
            }

            // if there are any error messages then show the errors
            if (errorMessage != string.Empty)
            {
                MessageBox.Show(errorMessage, "Error(s)");
            }
            else 
            {
                // call the function that inserts the values into the database that takes share type, shares and radio for its parameters
                insertValues(shareType, shares, radio);

            }
        }

        /// <summary>
        /// Once the tab is changed then, display the summary in the view summary tabs and the entries of the table in the entry tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // call the view summary , view entry and viewObjects functions
            viewEntry();
            viewSummary();
            viewObjects();


        }
        /// <summary>
        /// Displays all the share objects into a table using the classes created
        /// </summary>
        private void viewObjects()
        {
            try
            {

                // connection to database 

                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection cn = new SqlConnection(connectString);
                cn.Open();

                //Filling the grid
                string selectionQuery = "SELECT * FROM [Buyer]";
                SqlCommand command = new SqlCommand(selectionQuery, cn);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dtt = new DataTable("Sold Shares Object");
                sda.Fill(dtt);

                // changing the header columns of the table
                dtt.Columns[0].ColumnName = "Name";
                dtt.Columns[1].ColumnName = "Number of Shares";
                dtt.Columns[2].ColumnName = "Date Purchased";
                dtt.Columns[3].ColumnName = "Share Type";

                // adding two more columns
                dtt.Columns.Add("Share Price");
                dtt.Columns.Add("Voting Power");

                
                cn.Close();

                // list of shares
                List<Share> allShares = new List<Share>();

                // going through each row
                for (int i = 0; i < dtt.Rows.Count; i++)
                {
                    // if the datatype column has common in it
                    if (dtt.Rows[i].ItemArray[3].ToString() == "Common")
                    {
                        // have that row that has commonj in it and set its name and its date to a string variable of buyer name and buyer date
                        string buyerName = dtt.Rows[i].ItemArray[0].ToString();
                        string buyDate = dtt.Rows[i].ItemArray[2].ToString();

                        // create a new commonshare object (child class to share) and use its parametrized constructor
                        CommonShare common = new CommonShare(42, 1, buyerName, buyDate);

                        allShares.Add(common);

                        // add the sharing price and voting power values from the common share object to the row accordingly
                        dtt.Rows[i][4] = common.SharePrice;
                        dtt.Rows[i][5] = common.VotingPower;


                    }

                    // if the datatype column has preferred in it
                    if (dtt.Rows[i].ItemArray[3].ToString() == "Preferred")
                    {
                        // have that row that has commonj in it and set its name and its date to a string variable of buyer name and buyer date
                        string buyerName = dtt.Rows[i].ItemArray[0].ToString();
                        string buyDate = dtt.Rows[i].ItemArray[2].ToString();

                        // create a new Preferred object (child class to share) and use its parametrized constructor
                        PreferredShare preferred = new PreferredShare(100, 10, buyerName, buyDate);

                        allShares.Add(preferred);

                        // add the sharing price and voting power values from the common share object to the row accordingly
                        dtt.Rows[i][4] = preferred.SharePrice;
                        dtt.Rows[i][5] = preferred.VotingPower;


                    }
                }

                /// clear the list of objects
                allShares.Clear();
                
                // adjust column width to 133
                gridViewObjects.ColumnWidth = 133;
                gridViewObjects.ItemsSource = dtt.DefaultView;
            }
            // error message
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// Inserts the values entered by the user to the database
        /// </summary>
        /// <param name="shareType"></param>
        /// <param name="shares"></param>
        /// <param name="radio"></param>
        private void insertValues(string shareType, int shares, string radio)
        {
           
            int totalShares;
            // connection to database 
            string connectString = Properties.Settings.Default.connect_string;
            SqlConnection cn = new SqlConnection(connectString);
            cn.Open();


            string insertQuery = "SELECT [" + shareType + "] FROM [Shares]";
            SqlCommand command = new SqlCommand(insertQuery, cn);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            totalShares = Convert.ToInt32(reader[0]);
            cn.Close();

            if (shares > totalShares) // if there isnt enough shares
            {
                MessageBox.Show("Not enough shares available!");
            }
            else
            {
                //Inserting values
                cn.Open();
                insertQuery = "INSERT INTO [Buyer] ([name], [shares], [datePurchased], [shareType]) VALUES('" + txtName.Text + "', '" + txtNumberOfShares.Text + "', '" + dpDatePurchased.Text + "', '" + radio + "')";
                command = new SqlCommand(insertQuery, cn);
                command.ExecuteNonQuery();
                cn.Close();

                // after inserting query, subtract the shares sold to the total shares
                totalShares -= shares;

                //updating the shares fo the shares database
                cn.Open();
                insertQuery = "UPDATE [Shares] SET [" + shareType + "] = " + totalShares + " WHERE [" + shareType + "] = " + (totalShares + shares);
                command = new SqlCommand(insertQuery, cn);
                command.ExecuteNonQuery();
                cn.Close();

                // clearing inputboxes and radio buttons
                rbCommon.IsChecked = false;
                rbPreferred.IsChecked = false;
                txtName.Text = string.Empty;
                txtNumberOfShares.Text = string.Empty;
                dpDatePurchased.Text = string.Empty;


                MessageBox.Show("Added entry sucessfully!");
            }
        }

        /// <summary>
        /// Used to display the table of the database in the view entry tab.
        /// </summary>
        private void viewEntry()
        {
            try
            {

                //Connecting to database
                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection cn = new SqlConnection(connectString);
                cn.Open();

                //Filling the grid
                string selectionQuery = "SELECT * FROM [Buyer]";
                SqlCommand command = new SqlCommand(selectionQuery, cn);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable("Sold Shares");
                sda.Fill(dt);

                // changing the header columns of the table
                dt.Columns[0].ColumnName = "Name";
                dt.Columns[1].ColumnName = "Number of Shares";
                dt.Columns[2].ColumnName = "Date Purchased";
                dt.Columns[3].ColumnName = "Share Type";
                
                viewGrid.ColumnWidth = 200;
                viewGrid.ItemsSource = dt.DefaultView;

                cn.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Function to calcualte and display the common shares sold, preffered shares sold, revenue generaetd, common shares available and preffered shares available 
        /// </summary>
        private void viewSummary()
        {
            try
            {
                //Connecting to database
                string connectString = Properties.Settings.Default.connect_string;
                SqlConnection cn = new SqlConnection(connectString);
                cn.Open();

                string selectionQuery = "SELECT * FROM [Buyer]";
                SqlCommand command = new SqlCommand(selectionQuery, cn);

                // lists 
                List<int> commonShares = new List<int>();
                List<int> preferredShares = new List<int>();
                int genereatedRevenue = 0;

                // this will fill out the txt number of common shares sold inputbox

                selectionQuery = "SELECT SUM([shares]) FROM[Buyer] WHERE[shareType] = 'Common'";
                command = new SqlCommand(selectionQuery, cn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                txtNumberOfCommonSharesSold.Text = reader[0].ToString();
                cn.Close();

                // this will fill out the txt number of preffered shares sold inputbox
                cn.Open();
                selectionQuery = "SELECT SUM([shares]) FROM[Buyer] WHERE[shareType] = 'Preferred'";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();
                reader.Read();
                txtNumberOfPreferredSharesSold.Text = reader[0].ToString();
                cn.Close();

                // this will fill out the txt common shares available inputbox
                cn.Open();
                selectionQuery = "SELECT TOP 1 [numCommonShares] FROM [Shares]";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();
                reader.Read();
                txtCommonSharesAvailable.Text = reader[0].ToString();
                cn.Close();

                // this will fill out the txt preferred shares available inputbox
                cn.Open();
                selectionQuery = "SELECT TOP 1 [numPreferredShares] FROM [Shares]";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();
                reader.Read();
                txtPreferredSharesAvailable.Text = reader[0].ToString();
                cn.Close();

                // insert the shares to the common shares list if the share type is preffered from the database
                cn.Open();
                selectionQuery = "SELECT [shares] FROM [Buyer] WHERE [shareType] = 'Common'";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    commonShares.Add((int)reader[0]);
                }
                cn.Close();

                // insert the shares to the preffered shares list if the share type is preffered from the database
                cn.Open();
                selectionQuery = "SELECT [shares] FROM [Buyer] WHERE [shareType] = 'Preferred'";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    preferredShares.Add((int)reader[0]);
                }
                cn.Close();


                // to fill out revenue genereated by common shares 
                cn.Open();
                selectionQuery = "SELECT [datePurchased] FROM [Buyer] WHERE [shareType] = 'Common'";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();
                int i = 0;
                while (reader.Read()) //loop through each row
                {
                    DateTime dateExtractedFromDatabase = (DateTime)reader[0];
                    int day = (int)(dateExtractedFromDatabase - new DateTime(1990, 1, 1)).TotalDays;
                    Random rnd = new Random(day);

                    int id = rnd.Next(10, 100);

                    id *= commonShares[i]; // price is multiplied to each of the common shares
                  
                    // add to the total genereated revenue 
                    genereatedRevenue += id;

                    i++; // counter incrementation 
                }
                cn.Close();

                // to fill out revenue genereated by preferred shares 
                cn.Open();
                selectionQuery = "SELECT [datePurchased] FROM [Buyer] WHERE [shareType] = 'Preferred'";
                command = new SqlCommand(selectionQuery, cn);
                reader = command.ExecuteReader();

                i = 0;
                while (reader.Read()) //loop through each row
                {
                    DateTime extractedDate = (DateTime)reader[0];
                    int day = (int)(extractedDate - new DateTime(1990, 1, 1)).TotalDays;
                    Random rnd = new Random(day);

                    int id = rnd.Next(30, 100);

                    id *= preferredShares[i]; // price is multiplied to each of the preffered shares
                    // add to the total genereated revenue 
                    genereatedRevenue += id;

                    i++; // counter incrementation 
                }
                // display in the textbox the total generated revenue
                txtRevenueGenerated.Text = genereatedRevenue.ToString();
                cn.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
       

    }

   
}
