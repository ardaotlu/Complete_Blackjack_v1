using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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

namespace Complete_Blackjack_v1
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        Dictionary<string,int> playerScoreDb= new Dictionary<string,int>();
        Dictionary<string, string> playerPasswordDb = new Dictionary<string, string>();
        

        private MainWindow mainWindow2;

        public LoginView(MainWindow mainWindow)
        {
            InitializeComponent();
            mainWindow2 = mainWindow;
            Application.Current.MainWindow.Height = 970;
            Application.Current.MainWindow.Width = 800;

            ShowHighScoreTable();


            /*Player player = new Player { Name = "Arda", Password = "admin", Score = "10000"};

            await collection.InsertOneAsync(player);*/
        }


        public async void ShowHighScoreTable()
        {
            /*Player player = new Player { Name = "Arda", Password = "admin", Score = "10000"};

            await collection.InsertOneAsync(player);*/

            const string connectionUri = "mongodb+srv://<username>:<password>@firstcluster.xogechw.mongodb.net/?retryWrites=true&w=majority";
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            // Set the ServerApi field of the settings object to Stable API version 1
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            var client = new MongoClient(settings);
            string databaseName = "testdb";
            string collectionName = "player";
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<Player>(collectionName);

            var results = await collection.FindAsync(_ => true);

            foreach (var result in results.ToList())
            {
                playerPasswordDb.Add(result.Name,result.Password);
                playerScoreDb.Add(result.Name,int.Parse(result.Score));
            }

            var orderedDict = from entry in playerScoreDb orderby entry.Value descending select entry; 
            Dictionary<string,int> orderedPlayerScoreDb= orderedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);

            PlayerNamesListBox.ItemsSource = orderedPlayerScoreDb.Keys;
            PlayerScoresListBox.ItemsSource= orderedPlayerScoreDb.Values;

            //PlayerNamesListBox.ItemsSource =  from results.ToList();

        }

        private async void InsertToDB(Player player)
        {
            const string connectionUri = "mongodb+srv://<username>:<password>@firstcluster.xogechw.mongodb.net/?retryWrites=true&w=majority";
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            // Set the ServerApi field of the settings object to Stable API version 1
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            var client = new MongoClient(settings);
            string databaseName = "testdb";
            string collectionName = "player";
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<Player>(collectionName);
            await collection.InsertOneAsync(player);

            var results = await collection.FindAsync(_ => true);
            playerPasswordDb.Clear();
            playerScoreDb.Clear();

            foreach (var result in results.ToList())
            {
                playerPasswordDb.Add(result.Name, result.Password);
                playerScoreDb.Add(result.Name, int.Parse(result.Score));
            }

            var orderedDict = from entry in playerScoreDb orderby entry.Value descending select entry;
            Dictionary<string, int> orderedPlayerScoreDb = orderedDict.ToDictionary<KeyValuePair<string, int>, string, int>(pair => pair.Key, pair => pair.Value);

            PlayerNamesListBox.ItemsSource = orderedPlayerScoreDb.Keys;
            PlayerScoresListBox.ItemsSource = orderedPlayerScoreDb.Values;

        }


        private void OnLoginButtonPressed(object sender, RoutedEventArgs e)
        {
            if (username.Text == "")
                MessageBox.Show("Username can't be empty");
            else if (!playerPasswordDb.ContainsKey(username.Text))
                MessageBox.Show("Username not found");
            else if (playerPasswordDb[username.Text]==password.Password)
                mainWindow2.GoBackToStartPage(username.Text,playerScoreDb[username.Text]);
            else
                MessageBox.Show("Wrong Password");
        }

        private void OnRegisterButtonPressed(object sender, RoutedEventArgs e)
        {
            if (username.Text == "" || username.Text.Length < 3)
                MessageBox.Show("Username must contain at least 3 characters");
            else if (playerPasswordDb.ContainsKey(username.Text))
                MessageBox.Show("Username already taken");
            else if (password.Password=="" || password.Password.Length < 4)
                MessageBox.Show("Password must contains at least 4 characters");
            else
            {
                Player player = new Player { Name = username.Text, Password = password.Password, Score = "10000" };
                InsertToDB(player);
                MessageBox.Show($"Dear {username.Text}, welcome to Base Casino!\nYou have 10000 Base coins, enjoy!");
            }
        }


        private void PlayerNamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowUsernameFromHighscores();
        }

        private void ShowUsernameFromHighscores()
        {
            username.Text = PlayerNamesListBox.SelectedValue.ToString();
        }
    }
}
