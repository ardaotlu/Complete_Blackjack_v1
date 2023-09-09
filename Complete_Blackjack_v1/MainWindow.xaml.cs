
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Numerics;

namespace Complete_Blackjack_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Total, bet input, bet, thank you delay duration
        public int myTotal { get; set; }
        public int myBetInput { get; set; }
        public int myBetGame { get; set; }
        public int currentDur { get; set; }
        public Timer myTimer;

        // Dealer card total and player card total
        public int dealerCardTotal { get; set; }
        List<int> playerCardTotal = new List<int>();

        // Hand count and stand count
        int handCount = 0;
        int standCount = 0;
        // playerHands list contains playerCards lists
        List<Kart> playerCards = new List<Kart>();
        List<List<Kart>> playerHands = new List<List<Kart>>();
        List<int> myBets = new List<int>();
        List<TextBlock> myBetBlocks = new List<TextBlock>();

        // dealerCards list
        List<Kart> dealerCards = new List<Kart>();

        // Deck generation
        Hashtable deste = new Hashtable();
        Kart myKart = null;
        int ID = 0;
        Random randomer = new Random();
        int myTempRandom = 0;
        List<int> myRandom = new List<int>();
        int cardNo = 0;

        // player hand stackpanels list
        List<StackPanel> myStacks = new List<StackPanel>();
        private object content;
        string dbName="";

        public MainWindow()
        {

            InitializeComponent();
            content = Content;
            ChangeContentToPage1Action();
            //MainContent.Content = new LoginView();


            myTotal = 0;
            cTotal.Text = myTotal.ToString();
            myBetInput = 0;
            cBetInput.Text = myBetInput.ToString();
            //cBetGame.Text = "";
            GenerateDeck();
        }

        public void ChangeContentToPage1Action()
        {
            LoginView page = new LoginView(this);
            this.Content = page;
        }
        public void GoBackToStartPage(string name,int dbTotal)
        {
            Content = content;
            //Sets the height and width of the application back to default if it differs from the page calling this method
            Application.Current.MainWindow.Height = 970;
            Application.Current.MainWindow.Width = 800;
            myTotal = dbTotal;
            cTotal.Text = myTotal.ToString();
            dbName = name;
        }


        // Generates 8 decks with IDs 1-416
        // random ID list to generate deck order for first 240 cards (240 is shuffle time)
        public void GenerateDeck()
        {
            for (int k = 0; k < 8; k++)
            {
                for (int i = 2; i < 15; i++)
                {
                    ID++;
                    myKart = new Kart("kupa", i);
                    deste.Add(ID, myKart);
                }
                for (int i = 2; i < 15; i++)
                {
                    ID++;
                    myKart = new Kart("karo", i);
                    deste.Add(ID, myKart);
                }
                for (int i = 2; i < 15; i++)
                {
                    ID++;
                    myKart = new Kart("sinek", i);
                    deste.Add(ID, myKart);
                }
                for (int i = 2; i < 15; i++)
                {
                    ID++;
                    myKart = new Kart("maca", i);
                    deste.Add(ID, myKart);
                }
            }

            while (true)
            {
                int i = 0;
                myTempRandom = randomer.Next(1, 417);
                if (!myRandom.Contains(myTempRandom))
                    myRandom.Add(myTempRandom);
                i++;
                if (myRandom.Count == 240)
                    break;
            }

        }

        public void BlackjackControl(int handNo)
        {
            int tempTotal = 0;
            foreach (Kart kart in playerHands[handNo])
            {
                if (kart.Mag < 11)
                    tempTotal += kart.Mag;
                else if (kart.Mag > 10 && kart.Mag < 14)
                    tempTotal += 10;
                else
                    tempTotal += 11;
            }


            if (tempTotal == 21 && dealerCards[0].Mag < 10 && handCount == 0)
                Blackjack();

            else if (tempTotal == 21 && handCount == 0)
            {
                int dealerCardTotal = 0;
                if (dealerCards[0].Mag < 11)
                    dealerCardTotal += dealerCards[0].Mag;
                else if (dealerCards[0].Mag > 10 && dealerCards[0].Mag < 14)
                    dealerCardTotal += 10;
                else
                    dealerCardTotal += 1;

                Kart temp = (Kart)deste[myRandom[4]];
                dealerCards.Add(temp);
                SpawnDealerCard(1);

                if (temp.Mag < 11)
                    dealerCardTotal += temp.Mag;
                else if (temp.Mag > 10 && temp.Mag < 14)
                    dealerCardTotal += 10;
                else
                    dealerCardTotal += 1;

                foreach (Kart kart in dealerCards)
                {
                    if (kart.Mag == 14 && dealerCardTotal + 10 > 16 && dealerCardTotal + 10 < 22)
                    {
                        dealerCardTotal += 10;
                    }
                }
                if (dealerCardTotal == 21)
                    Tie();
                if (dealerCardTotal < 21)
                    Blackjack();
            }

            /*else if (playerCardTotal[handNo] == 21 && handCount > 0)
            {
                Stand
            }*/



        }
        private async void GameStart()
        {
            OneBut.IsEnabled = false;
            TwoBut.IsEnabled = false;
            FiveBut.IsEnabled = false;
            TenBut.IsEnabled = false;
            BetBut.IsEnabled = false;
            ClearBetBut.IsEnabled = false;



            // Get two random player cards and one random dealer card
            Kart temp = (Kart)deste[myRandom[cardNo]];
            playerCards.Add(temp);
            cardNo++;
            temp = (Kart)deste[myRandom[cardNo]];
            playerCards.Add(temp);
            playerHands.Add(playerCards);
            cardNo++;
            temp = (Kart)deste[myRandom[cardNo]];
            dealerCards.Add(temp);
            cardNo++;

            // Create first hand stack panel and spawn first cards
            PlayerHandStackPanelGenerator();
            GameStartCardSpawns();
            playerCardTotal.Add(0);
            playerCardTotal[0] = 0;
            foreach (Kart kart in playerHands[0])
            {
                if (kart.Mag < 11)
                    playerCardTotal[0] += kart.Mag;
                else if (kart.Mag > 10 && kart.Mag < 14)
                    playerCardTotal[0] += 10;
                else
                {
                    playerCardTotal[0] += 1;
                }
            }

            if (dealerCards[0].Mag < 11)
                dealerCardTotal = dealerCards[0].Mag;
            else if (dealerCards[0].Mag > 10 && dealerCards[0].Mag < 14)
                dealerCardTotal = 10;
            else
                dealerCardTotal = 1;

            await Task.Delay(2300);


            if ((playerHands[0][0].Mag == playerHands[0][1].Mag) || (playerHands[0][0].Mag > 9 && playerHands[0][0].Mag < 14 && playerHands[0][1].Mag > 9 && playerHands[0][1].Mag < 14))
            {
                SplitBut.IsEnabled = true;
                SplitBut.Opacity = 0.9;
            }
            HitBut.IsEnabled = true;
            StandBut.IsEnabled = true;
            if (myBetGame < myTotal || myBetGame == myTotal)
                DoubleBut.IsEnabled = true;
            BlackjackControl(0);

        }

        private async void GameStartCardSpawns()
        {
            SpawnPlayerCard(0, 0);
            await Task.Delay(800);
            SpawnDealerCard(0);
            await Task.Delay(800);
            SpawnPlayerCard(0, 1);

        }

        private void Clearer()
        {
            playerCardTotal.Clear();
            cDealerCards.Children.Clear();
            cStacks.Children.Clear();
            cBets.Children.Clear();
            playerCards.Clear();
            myStacks.Clear();
            playerHands.Clear();
            dealerCards.Clear();
            handCount = 0;
            standCount = 0;
            //cBetGame.Text = "";
            cBetInput.Text = "";
            cBetInput.Opacity = 0.5;
            myBetInput = 0;
            myBets.Clear();
            myBetBlocks.Clear();
            // cardNo = 3;
            cInfo.Text = "Place your bet please > ";
            OneBut.IsEnabled = true;
            TwoBut.IsEnabled = true;
            FiveBut.IsEnabled = true;
            TenBut.IsEnabled = true;
            BetBut.IsEnabled = true;
            ClearBetBut.IsEnabled = true;
            HitBut.IsEnabled = false;
            StandBut.IsEnabled = false;
            DoubleBut.IsEnabled = false;
            SplitBut.IsEnabled = false;
            SplitBut.Opacity = 0;
        }

        private void Blackjack()
        {
            if (handCount == 0)
            {
                MessageBox.Show("Blackjack!");
                myTotal = myTotal + 2 * myBetGame + (myBetGame / 2);
                cTotal.Text = myTotal.ToString();
                Clearer();
            }
        }

        private void PlayerHandStackPanelGenerator()
        {
            StackPanel mainStackPanel = (StackPanel)FindName("cStacks");
            StackPanel playerPanel = new StackPanel();
            playerPanel.Orientation = Orientation.Horizontal;
            playerPanel.HorizontalAlignment = HorizontalAlignment.Center;
            mainStackPanel.Children.Add(playerPanel);
            myStacks.Add(playerPanel);
        }
        private void SpawnDealerCard(int cardNo)
        {
            Kart kart = dealerCards[cardNo];
            int imageID;
            if (kart.ID % 52 == 0)
                imageID = 52;
            else if (kart.ID > 52)
                imageID = kart.ID % 52;
            else
                imageID = kart.ID;
            string mySource = "/Img/NewFolder/" + imageID + ".png";
            Image newImage = new Image();
            newImage.Height = 160;
            newImage.HorizontalAlignment = HorizontalAlignment.Center;
            BitmapImage image = new BitmapImage(new Uri(mySource, UriKind.Relative));
            newImage.Source = image;
            StackPanel dealerPanel = (StackPanel)FindName("cDealerCards");
            dealerPanel.Children.Add(newImage);
        }
        private void SpawnPlayerCard(int handNo, int cardNo)
        {
            Kart kart = playerHands[handNo][cardNo];
            int imageID;
            if (kart.ID % 52 == 0)
                imageID = 52;
            else if (kart.ID > 52)
                imageID = kart.ID % 52;
            else
                imageID = kart.ID;
            string mySource = "/Img/NewFolder/" + imageID + ".png";
            Image newImage = new Image();
            newImage.Height = 160;
            newImage.HorizontalAlignment = HorizontalAlignment.Center;
            BitmapImage image = new BitmapImage(new Uri(mySource, UriKind.Relative));
            newImage.Source = image;
            myStacks[handNo].Children.Add(newImage);
        }

        

        private void Hit(int handNo)
        {
            Kart temp = (Kart)deste[myRandom[cardNo]];
            playerHands[handNo].Add(temp);
            SpawnPlayerCard(handNo, playerHands[handNo].Count - 1);
            cardNo++;

            if (temp.Mag < 11)
                playerCardTotal[handNo] += temp.Mag;
            else if (temp.Mag > 10 && temp.Mag < 14)
                playerCardTotal[handNo] += 10;
            else
            {
                playerCardTotal[handNo] += 1;
            }
            OverCheck(handNo);
        }
        private void Hit_Click(object sender, RoutedEventArgs e)
        {
            DoubleBut.IsEnabled = false;
            Hit(standCount);
        }

        private Boolean StandNeeded()
        {
            bool needed = false;
            for (int i = 0; i < handCount; i++)
            {
                if (playerCardTotal[i] < 22)
                    needed = true;
            }
            return needed;
        }
        private async void OverCheck(int handNo)
        {
            if (playerCardTotal[handNo] == 21)
                HitBut.IsEnabled = false;
            if (playerCardTotal[handNo] > 21)
            {
                HitBut.IsEnabled = false;
                Lose();
                if (handCount > 0)
                {
                    await Task.Delay(1200);
                    myStacks[handNo].Children.Clear();
                    myBetBlocks[handNo].Text = "";

                    if (handCount == standCount)
                    {
                        if (StandNeeded())
                            Stand();
                        else
                        {
                            MessageBox.Show("All hands lost");
                            Clearer();
                        }

                    }
                    else
                    {
                        ++standCount;
                        DoubleBut.IsEnabled = true;
                        HitBut.IsEnabled = true;
                        Hit(handNo + 1);
                    }

                }
            }

        }
        private void Lose()
        {
            if (handCount == 0)
            {
                MessageBox.Show("You lose!");
                Clearer();
            }
        }
        private void Win()
        {
            if (handCount == 0)
            {
                MessageBox.Show("You Win");
                myTotal = myTotal + 2 * myBetGame;
                cTotal.Text = myTotal.ToString();
                Clearer();
            }
        }
        private void Tie()
        {
            if (handCount == 0)
            {
                MessageBox.Show("Tie");
                myTotal = myTotal + myBetGame;
                cTotal.Text = myTotal.ToString();
                Clearer();
            }

        }

        private async void Stand()
        {

            HitBut.IsEnabled = false;
            StandBut.IsEnabled = false;
            dealerCardTotal = 0;

            if (dealerCards[0].Mag < 11)
                dealerCardTotal += dealerCards[0].Mag;
            else if (dealerCards[0].Mag > 10 && dealerCards[0].Mag < 14)
                dealerCardTotal += 10;
            else
            {
                dealerCardTotal += 1;
            }

            while (dealerCardTotal < 17)
            {
                await Task.Delay(800);
                Kart temp = (Kart)deste[myRandom[cardNo]];
                dealerCards.Add(temp);
                SpawnDealerCard(dealerCards.Count - 1);
                cardNo++;

                if (temp.Mag < 11)
                    dealerCardTotal += temp.Mag;
                else if (temp.Mag > 10 && temp.Mag < 14)
                    dealerCardTotal += 10;
                else
                {
                    dealerCardTotal += 1;
                }

                foreach (Kart kart in dealerCards)
                {
                    if (kart.Mag == 14 && dealerCardTotal + 10 > 16 && dealerCardTotal + 10 < 22)
                    {
                        dealerCardTotal += 10;
                    }
                }

            }

            for (int p = 0; p < handCount + 1; p++)
            {
                foreach (Kart kart in playerHands[p])
                {
                    if (kart.Mag == 14 && playerCardTotal[p] + 10 < 22)
                        playerCardTotal[p] += 10;
                }
            }
            string x = "";
            for (int i = 0; i < handCount + 1; i++)
            {
                if (playerCardTotal[i] > 21)
                {
                    x += "Hand no:" + i + " loses\n";
                }
                else if (dealerCardTotal > 21)
                {
                    if (playerHands[i].Count == 2 && playerCardTotal[i] == 21)
                    {
                        myTotal += 2 * myBets[i] + myBets[i] / 2;
                        x += "Hand no:" + i + " blackjack\n";
                    }
                    else
                    {
                        myTotal += 2 * myBets[i];
                        x += "Hand no:" + i + " wins\n";
                    }
                }
                else if (dealerCardTotal == 21 && playerCardTotal[i] == 21)
                {
                    if (playerHands[i].Count == 2 && dealerCards.Count == 2)
                    {
                        myTotal += myBets[i];
                        x += "Hand no:" + i + " blackjack but dealer also has blackjack\n";
                    }
                    else if (playerHands[i].Count == 2)
                    {
                        myTotal += 2 * myBets[i] + myBets[i] / 2;
                        x += "Hand no:" + i + " blackjack\n";
                    }
                    else if (dealerCards.Count == 2)
                    {
                        x += "Hand no:" + i + " loses\n";
                    }
                    else
                    {
                        myTotal += myBets[i];
                        x += "Hand no:" + i + " ties\n";
                    }
                }
                else if (dealerCardTotal == playerCardTotal[i])
                {
                    myTotal += myBets[i];
                    x += "Hand no:" + i + " ties\n";
                }

                else if (dealerCardTotal < playerCardTotal[i])
                {
                    myTotal += 2 * myBets[i];
                    x += "Hand no:" + i + " wins\n";
                }
                else
                    x += "Hand no:" + i + " loses\n";

            }
            MessageBox.Show(x);
            cTotal.Text = myTotal.ToString();
            PlayerScoreUpdateDB();
            Clearer();

        }

        private async void Stand_Click(object sender, RoutedEventArgs e)
        {
            if (handCount == standCount)
            {
                Stand();
            }
            else
            {
                ++standCount;
                Hit(standCount);
                await Task.Delay(800);
                DoubleBut.IsEnabled = true;
                HitBut.IsEnabled = true;
                if ((playerHands[standCount][0].Mag == playerHands[standCount][1].Mag) || (playerHands[standCount][0].Mag > 9 && playerHands[standCount][0].Mag < 14 && playerHands[standCount][1].Mag > 9 && playerHands[standCount][1].Mag < 14))
                {
                    SplitBut.IsEnabled = true;
                    SplitBut.Opacity = 0.9;
                }
            }
        }


        private void Double_Click(object sender, RoutedEventArgs e)
        {

            myTotal = myTotal - myBetGame;
            cTotal.Text = myTotal.ToString();
            myBets[standCount] = myBetGame * 2;
            myBetBlocks[standCount].Text = myBets[standCount].ToString();
            //cBetGame.Text = myBetGame.ToString();

            Hit(standCount);
            if (handCount == standCount)
            {
                Stand();
            }
            else
            {
                ++standCount;
                DoubleBut.IsEnabled = true;
            }
        }
        private void One_Click(object sender, RoutedEventArgs e)
        {
            if (myTotal > 99)
            {
                myBetInput += 100;
                myTotal -= 100;
                cBetInput.Text = myBetInput.ToString();
                cTotal.Text = myTotal.ToString();
            }
        }
        private void Two_Click(object sender, RoutedEventArgs e)
        {
            if (myTotal > 199)
            {
                myBetInput += 200;
                myTotal -= 200;
                cBetInput.Text = myBetInput.ToString();
                cTotal.Text = myTotal.ToString();
            }
        }
        private void Five_Click(object sender, RoutedEventArgs e)
        {
            if (myTotal > 499)
            {
                myBetInput += 500;
                myTotal -= 500;
                cBetInput.Text = myBetInput.ToString();
                cTotal.Text = myTotal.ToString();
            }
        }
        private void Ten_Click(object sender, RoutedEventArgs e)
        {
            if (myTotal > 999)
            {
                myBetInput += 1000;
                myTotal -= 1000;
                cBetInput.Text = myBetInput.ToString();
                cTotal.Text = myTotal.ToString();
            }
        }
        private void Bet_Click(object sender, RoutedEventArgs e)
        {
            if (myBetInput > 0)
            {
                ThankYou();
                myBetGame = myBetInput;
                myBets.Add(myBetGame);
                //cBetGame.Text = myBetGame.ToString();



                //< TextBlock Focusable = "False" x: Name = "cBetGame" Grid.Row = "8" Grid.Column = "1"
                //FontSize = "22" FontFamily = "Lucida Fax"
                //Background = "{x:Null}" HorizontalAlignment = "Center" VerticalAlignment = "Center" Margin = "0,0,0,0" />
                StackPanel myBetPanel = (StackPanel)FindName("cBets");
                TextBlock myBetText = new TextBlock();
                myBetText.Text = myBetGame.ToString();
                myBetText.FontSize = 22;
                myBetText.HorizontalAlignment = HorizontalAlignment.Center;
                myBetBlocks.Add(myBetText);
                myBetPanel.Children.Add(myBetText);
                PlayerScoreUpdateDB();
                cBetInput.Opacity = 0;
                GameStart();
            }

        }
        private void ClearBet_Click(object sender, RoutedEventArgs e)
        {
            myTotal = myTotal + myBetInput;
            cTotal.Text = myTotal.ToString();
            myBetInput = 0;
            cBetInput.Text = myBetInput.ToString();
        }
        private async void ThankYou()
        {
            cInfo.Text = "Thank you!";
            await Task.Delay(2000);
            cInfo.Text = "";
        }

        private async void Split()
        {
            SplitBut.IsEnabled = false;
            SplitBut.Opacity = 0;
            ++handCount;
            int i = standCount;
            myTotal = myTotal - myBetGame;
            myBets.Add(myBetGame);
            cTotal.Text = myTotal.ToString();

            List<Kart> newPlayerCards = new List<Kart>();
            newPlayerCards.Add(playerHands[i][1]);
            playerHands[i].RemoveAt(1);
            playerHands.Add(newPlayerCards);
            if (playerHands[i][0].Mag < 11)
                playerCardTotal[i] = playerHands[i][0].Mag;
            else if (playerHands[i][0].Mag > 10 && playerHands[i][0].Mag < 14)
                playerCardTotal[i] = 10;
            else
            {
                playerCardTotal[i] = 1;
            }
            int temp = 0;
            if (playerHands[playerHands.Count - 1][0].Mag < 11)
                temp = playerHands[playerHands.Count - 1][0].Mag;
            else if (playerHands[playerHands.Count - 1][0].Mag > 10 && playerHands[playerHands.Count - 1][0].Mag < 14)
                temp = 10;
            else
            {
                temp = 1;
            }
            playerCardTotal.Add(temp);

            StackPanel stackPanel = (StackPanel)FindName("cStacks");

            myStacks[i].Children.Clear();

            SpawnPlayerCard(i, 0);

            PlayerHandStackPanelGenerator();
            myStacks[myStacks.Count - 1].Margin = new Thickness(100, 0, 0, 0);

            SpawnPlayerCard(playerHands.Count - 1, 0);

            StackPanel myBetPanel = (StackPanel)FindName("cBets");
            TextBlock myBetText = new TextBlock();
            myBetText.Text = myBetGame.ToString();
            myBetText.FontSize = 22;
            myBetText.VerticalAlignment = VerticalAlignment.Center;
            myBetText.HorizontalAlignment = HorizontalAlignment.Center;
            myBetText.Margin = new Thickness(100, 0, 0, 0);
            myBetBlocks.Add(myBetText);
            myBetPanel.Children.Add(myBetText);

            await Task.Delay(800);
            Hit(i);

            if ((playerHands[i][0].Mag == playerHands[i][1].Mag) || (playerHands[i][0].Mag > 9 && playerHands[i][0].Mag < 14 && playerHands[i][1].Mag > 9 && playerHands[i][1].Mag < 14))
            {
                SplitBut.IsEnabled = true;
                SplitBut.Opacity = 0.9;
            }
        }
        private void Split_Click(object sender, RoutedEventArgs e)
        {
            Split();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            /*foreach (var item in playerHands)
            {
                foreach (Kart x in item)
                {
                    MessageBox.Show(x.Mag.ToString());
                }
            }
            */
            Clearer();
        }

        private async void PlayerScoreUpdateDB()
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


            var filter = Builders<Player>.Filter.Eq(player => player.Name, dbName);
            var update = Builders<Player>.Update.Set(player => player.Score, myTotal.ToString());

            await collection.UpdateOneAsync(filter, update);
        }
    }
}


