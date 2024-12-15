using Microsoft.VisualBasic;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace mastermind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool codeCracked = false;

        bool gameEnded = false;

        int attempts = 0;

        int playerScore = 100;

        string nameInput;

        private bool _isDebugMode = false;

        string attemptString;

        private int maxAttempts = 10;

        int attemptsInput;





        // Beschikbare kleuren
        List<string> kleuren = new List<string> { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };


        //pogingen tellen van de speler
        private List<string> playerAttempts = new List<string>();


        // Lijst voor de gegenereerde code
        List<string> secretCode = new List<string>();

        //lijst voor de highscores te weergeven
        private List<string> highscores = new List<string>(); // Lijst voor highscores

        //lijst met opgeslagen scores
        private int highscoreCount = 0; // Huidig aantal opgeslagen scores

        //lijst met spelernamen
        List<string> playerNames = new List<string>();






        private DispatcherTimer _countdownTimer;
        private int _currentTime;
        public MainWindow()
        {
            InitializeComponent();
            // Voeg een globale toetsencombinatie toe voor Ctrl+F12
            this.KeyDown += MainWindow_KeyDown;
            StartGame();



        }

        private void StartGame()
        {
            //Ask the players name
            MessageBoxResult result;
            if(playerNames.Count == 0)
            {
                do
                {
                    nameInput = Interaction.InputBox("Geef uw naam op", "Invoer", "", 500);
                    while (string.IsNullOrEmpty(nameInput))
                    {
                        MessageBox.Show("Geef je naam!", "Foutieve invoer");
                        nameInput = Interaction.InputBox("Geef uw naam op", "Invoer", "", 500);
                    }
                    playerNames.Add(nameInput);
                    nameInput = "";
                    result = MessageBox.Show("Wilt u nog spelers toevoegen?", "Solo, of niet? :)", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                while (result == MessageBoxResult.Yes);
            }

            nameInput = playerNames[0];
            codeCracked = false;
            gameEnded= false;


            GenerateRandomCode(); //generate a new random code

            VulComboBoxen(); // fill the combo-boxes with the colors





            // Timer instellen
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Timer loopt elke seconde
            };
            _countdownTimer.Tick += CountdownTimer_Tick;




            // Zet de timer initieel op 0
            _currentTime = 0;
            UpdateCountdownLabel();
            StartCountdown();

            //start the countdown
            StartCountdown();



            attemptsListBox.Items.Clear();
            secretCodeTextBox.Text = "Mastermind oplossing: " + string.Join(", ", secretCode);
            attempts = 0;
            playerScore = 100;
            playerScoreTextBox.Text = $"Score: {playerScore}/100   ({nameInput})";
            StartCountdown();
            this.Title = $"Mastermind - poging {attempts}/{maxAttempts}";
            codeCracked = false;


        }


        private void EndGame()
        {
            playerNames.RemoveAt(0);
            if (playerNames.Count() > 0)
            {
                if (codeCracked)
                {
                    MessageBoxResult result = MessageBox.Show($"Code gekraakt in {attempts} pogingen.\nNu is de beurt aan {playerNames[0]}", $"{nameInput}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    AddHighscore(nameInput, attempts, playerScore); // Voeg de highscore toe
                    if (result == MessageBoxResult.OK)
                    {
                        _countdownTimer.Stop();
                        secretCode.Clear();
                        label1.Background = System.Windows.Media.Brushes.White;
                        label1.BorderBrush = System.Windows.Media.Brushes.White;
                        label2.Background = System.Windows.Media.Brushes.White;
                        label2.BorderBrush = System.Windows.Media.Brushes.White;
                        label3.Background = System.Windows.Media.Brushes.White;
                        label3.BorderBrush = System.Windows.Media.Brushes.White;
                        label4.Background = System.Windows.Media.Brushes.White;
                        label4.BorderBrush = System.Windows.Media.Brushes.White;
                        comboBox1.SelectedItem = -1;
                        comboBox2.SelectedItem = -1;
                        comboBox3.SelectedItem = -1;
                        comboBox4.SelectedItem = -1;
                        StartGame();
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"You failed! De corecte code was " + string.Join(", ", secretCode) + $".\nNu is de beurt aan {playerNames[0]}", $"{nameInput}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    AddHighscore(nameInput, attempts, playerScore); // Voeg de highscore toe
                    if (result == MessageBoxResult.OK)
                    {
                        _countdownTimer.Stop();
                        secretCode.Clear();
                        label1.Background = System.Windows.Media.Brushes.White;
                        label1.BorderBrush = System.Windows.Media.Brushes.White;
                        label2.Background = System.Windows.Media.Brushes.White;
                        label2.BorderBrush = System.Windows.Media.Brushes.White;
                        label3.Background = System.Windows.Media.Brushes.White;
                        label3.BorderBrush = System.Windows.Media.Brushes.White;
                        label4.Background = System.Windows.Media.Brushes.White;
                        label4.BorderBrush = System.Windows.Media.Brushes.White;
                        comboBox1.SelectedItem = -1;
                        comboBox2.SelectedItem = -1;
                        comboBox3.SelectedItem = -1;
                        comboBox4.SelectedItem = -1;
                        StartGame();
                    }
                }
            }
            else
            {
                if (codeCracked)
                {
                    MessageBoxResult result = MessageBox.Show($"Code gekraakt in {attempts} pogingen.", $"{nameInput}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    AddHighscore(nameInput, attempts, playerScore); // Voeg de highscore toe
                    if (result == MessageBoxResult.OK)
                    {
                        _countdownTimer.Stop();
                        secretCode.Clear();
                        label1.Background = System.Windows.Media.Brushes.White;
                        label1.BorderBrush = System.Windows.Media.Brushes.White;
                        label2.Background = System.Windows.Media.Brushes.White;
                        label2.BorderBrush = System.Windows.Media.Brushes.White;
                        label3.Background = System.Windows.Media.Brushes.White;
                        label3.BorderBrush = System.Windows.Media.Brushes.White;
                        label4.Background = System.Windows.Media.Brushes.White;
                        label4.BorderBrush = System.Windows.Media.Brushes.White;
                        comboBox1.SelectedItem = -1;
                        comboBox2.SelectedItem = -1;
                        comboBox3.SelectedItem = -1;
                        comboBox4.SelectedItem = -1;
                        StartGame();
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show($"You failed! De corecte code was " + string.Join(", ", secretCode) + ".", $"{nameInput}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    AddHighscore(nameInput, attempts, playerScore); // Voeg de highscore toe
                    if (result == MessageBoxResult.OK)
                    {
                        _countdownTimer.Stop();
                        secretCode.Clear();
                        label1.Background = System.Windows.Media.Brushes.White;
                        label1.BorderBrush = System.Windows.Media.Brushes.White;
                        label2.Background = System.Windows.Media.Brushes.White;
                        label2.BorderBrush = System.Windows.Media.Brushes.White;
                        label3.Background = System.Windows.Media.Brushes.White;
                        label3.BorderBrush = System.Windows.Media.Brushes.White;
                        label4.Background = System.Windows.Media.Brushes.White;
                        label4.BorderBrush = System.Windows.Media.Brushes.White;
                        comboBox1.SelectedItem = -1;
                        comboBox2.SelectedItem = -1;
                        comboBox3.SelectedItem = -1;
                        comboBox4.SelectedItem = -1;
                        StartGame();
                    }
                }
            }

        }


        

        private void AddHighscore(string name, int attempts, int score)
        {
            string newHighscore = $"{name} - {attempts} pogingen - {score}/100";

            // Voeg de nieuwe highscore toe aan de lijst
            highscores.Add(newHighscore);

            // Sorteer de lijst: eerst op score (aflopend), dan op pogingen (oplopend)
            highscores = highscores
                .OrderByDescending(h => ParseScore(h))  // Gebruik een veilige parse-methode voor de score
                .ThenBy(h => ParseAttempts(h))          // Gebruik een veilige parse-methode voor de pogingen
                .ToList();

            // Beperk de lijst tot maximaal 15 highscores
            if (highscores.Count > 15)
            {
                highscores = highscores.Take(15).ToList();
            }
        }

        // Methode om de score veilig te parsen
        private int ParseScore(string highscore)
        {
            try
            {
                // Haal de score (het gedeelte voor de '/')
                var scoreString = highscore.Split('-')[2].Split('/')[0].Trim();
                return int.Parse(scoreString);
            }
            catch
            {
                return 0; // Als er een fout optreedt, geef dan 0 terug
            }
        }

        // Methode om het aantal pogingen veilig te parsen
        private int ParseAttempts(string highscore)
        {
            try
            {
                // Haal het aantal pogingen (het tweede gedeelte)
                var attemptsString = highscore.Split('-')[1].Split()[0].Trim();
                return int.Parse(attemptsString);
            }
            catch
            {
                return int.MaxValue; // Als er een fout optreedt, geef dan een zeer hoog getal terug zodat het altijd onderaan komt
            }
        }










        /// <summary>
        /// Start de countdown timer vanaf 1 seconde en reset de huidige tijd.
        /// Wordt gebruikt bij het genereren van een nieuwe code of bij een poging.
        /// </summary>
        private void StartCountdown()
        {
            // Reset de timer en start opnieuw vanaf 1
            _currentTime = 1;
            UpdateCountdownLabel();




            if (_countdownTimer.IsEnabled)
            {
                _countdownTimer.Stop();
            }

            _countdownTimer.Start();
        }




        /// <summary>
        /// Stopt de countdown timer.
        /// Wordt gebruikt als een maximale tijdslimiet of poging is bereikt.
        /// </summary>
        private void StopCountdown()
        {
            _countdownTimer.Stop(); // Timer stoppen
            UpdateCountdownLabel();
        }



        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            // Verhoog de timer elke seconde
            _currentTime++;
            UpdateCountdownLabel();
            if (attempts >= maxAttempts)
            {
                StopCountdown();
                timerLabel.Content = "EINDE SPEL!";
                gameEnded = true;

            }

                // Controleer of de timer op 10 staat
                if (_currentTime == 10)
            {
                HandleTimerAtTen();
            }
        }



        private void HandleTimerAtTen()
        {
            playerScore -= 10;
            playerScoreTextBox.Text = $"Score: {playerScore}/100";
            attemptsListBox.Items.Add($"Poging {attempts + 1}: TIJD OM!");
            // Actie uitvoeren als de timer op 10 komt
            _countdownTimer.Stop(); // Timer stoppen als voorbeeld
            attempts++;
            this.Title = $"Mastermind - Poging: {attempts}/{maxAttempts}";
            if (attempts >= maxAttempts)
            {
                StopCountdown(); // Timer stoppen als voorbeeld
                timerLabel.Content = "EINDE SPEL!";
                gameEnded = true;
                EndGame();
            }
            else
            {
                StartCountdown();
            }

        }




        private void UpdateCountdownLabel()
        {
            if(attempts == 10)
            {
                timerLabel.Content = "EINDE SPEL!";
            }
            else
            {
                timerLabel.Content = $"Tijd voor kans voorbij gaat: {_currentTime}/10 seconden";
            }
        }


















        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Controleer of de gebruiker CTRL+F12 indrukt
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.F12)
                {
                    ToggleDebug();
                }
            }
        }













        /// <summary>
        /// Schakelt de debug-modus in of uit.
        /// Wanneer de debug-modus actief is, wordt de geheime code getoond in een TextBox.
        /// </summary>
        private void ToggleDebug()
        {
            // Wissel debug-modus aan/uit
            _isDebugMode = !_isDebugMode;



            // Update de zichtbaarheid van de TextBox
            secretCodeTextBox.Visibility = _isDebugMode ? Visibility.Visible : Visibility.Collapsed;



            // Voeg debuginformatie toe als voorbeeld
            if (_isDebugMode)
            {
                secretCodeTextBox.Text = "Mastermind oplossing: " + string.Join(", ", secretCode);
            }
        }












        // Methode om de willekeurige code te genereren
        private void GenerateRandomCode()
        {


            // Random object voor willekeurige getallen
            Random random = new Random();



            // Genereer een willekeurige code van 4 kleuren
            for (int i = 0; i < 4; i++)
            {
                secretCode.Add(kleuren[random.Next(kleuren.Count)]);
            }




            // Zet de pogingen in de titel van het window
            this.Title = $"Mastermind - poging {attempts}/{maxAttempts}";
        }









        private void VulComboBoxen()
        {
            comboBox1.ItemsSource = kleuren;
            comboBox2.ItemsSource = kleuren;
            comboBox3.ItemsSource = kleuren;
            comboBox4.ItemsSource = kleuren;
        }






        private void CheckComboBoxChanges(object sender)
        {
            // Controleer welke ComboBox is geselecteerd en werk het juiste Label bij
            if (sender == comboBox1)
            {
                UpdateLabelColor(label1, comboBox1.SelectedItem.ToString());
            }
            else if (sender == comboBox2)
            {
                UpdateLabelColor(label2, comboBox2.SelectedItem.ToString());
            }
            else if (sender == comboBox3)
            {
                UpdateLabelColor(label3, comboBox3.SelectedItem.ToString());
            }
            else if (sender == comboBox4)
            {
                UpdateLabelColor(label4, comboBox4.SelectedItem.ToString());
            }
        }




        // Event handler voor de selectie van een kleur in de ComboBox
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Controleer welke ComboBox is geselecteerd en werk het juiste Label bij
            CheckComboBoxChanges(sender);
        }











        // Methode om de achtergrondkleur van een label bij te werken
        private void UpdateLabelColor(System.Windows.Controls.Label label, string colorName)
        {
            // Zet de achtergrondkleur van het label op basis van de geselecteerde kleur
            switch (colorName)
            {
                case "Rood":
                    label.Background = System.Windows.Media.Brushes.Red;
                    break;
                case "Geel":
                    label.Background = System.Windows.Media.Brushes.Yellow;
                    break;
                case "Oranje":
                    label.Background = System.Windows.Media.Brushes.Orange;
                    break;
                case "Wit":
                    label.Background = System.Windows.Media.Brushes.White;
                    break;
                case "Groen":
                    label.Background = System.Windows.Media.Brushes.Green;
                    break;
                case "Blauw":
                    label.Background = System.Windows.Media.Brushes.Blue;
                    break;
                default:
                    label.Background = System.Windows.Media.Brushes.Transparent; // Als er geen kleur is geselecteerd
                    break;
            }
        }











        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Example: Show confirmation dialog
            var result = MessageBox.Show("Weet je zeker dat je het spel wilt sluiten?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                // Cancel the closing
                e.Cancel = true;
            }
            else
            {
                return;
            }
        }









        // Event handler voor het klikken op de Check Code knop
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if(gameEnded)
            {
                MessageBox.Show("Code is al beeindigd, Start een nieuw spel via het menu vanboven.", "Al beeindigd!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (codeCracked)
            {
                MessageBox.Show("Code is al gekraakt, Start een nieuw spel via het menu vanboven.", "Al gekraakt!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if(attempts < maxAttempts)
            {




                // Haal de ingevoerde kleuren uit de ComboBoxen
                List<string> userInput = new List<string>
                {
                    comboBox1.SelectedItem?.ToString(),
                    comboBox2.SelectedItem?.ToString(),
                    comboBox3.SelectedItem?.ToString(),
                    comboBox4.SelectedItem?.ToString()
                };





            // Controleer of de gebruiker een waarde heeft geselecteerd in elke ComboBox
            if (userInput.Contains(null))
                {
                    MessageBox.Show("Selecteer een kleur voor alle vakken!");
                    return;
                }
                else
                {
                    StartCountdown();
                }



                // Voeg poging toe aan de lijst
                attemptString = string.Join(", ", userInput);
                playerAttempts.Add(attemptString);



                // Update de ListBox
                attemptsListBox.Items.Add($"Poging {attempts + 1}: {attemptString}");




                //voeg 1 poging toe aan de attempts
                attempts++;


            //weergeef het geupdaten attempt variabel
            this.Title = $"Mastermind - Poging: {attempts}/{maxAttempts}";






            // Vergelijk de ingevoerde code met de geheime code
            for (int i = 0; i < 4; i++)
            {

                // Als de kleur op de juiste plaats staat (Rood)
                if (userInput[i] == secretCode[i])
                {
                    SetLabelBorder(i, Colors.DarkRed); // Rode rand voor correcte positie
                }
                else if (secretCode.Contains(userInput[i]))
                {
                    SetLabelBorder(i, Colors.Wheat); // Witte rand voor correcte kleur maar verkeerde positie
                        playerScore--;
                        playerScoreTextBox.Text = $"Score: {playerScore}/100   ({nameInput})";
                    }
                else
                {
                    SetLabelBorder(i, Colors.Transparent); // Geen rand als de kleur niet in de code zit
                        playerScore -= 2;
                        playerScoreTextBox.Text = $"Score: {playerScore}/100   ({nameInput})";
                    }
            }

                if (attempts == maxAttempts)
                {
                    gameEnded = true;
                    EndGame();
                    StopCountdown();
                    _countdownTimer.Stop();
                }
                if (userInput.SequenceEqual(secretCode))
                {
                    codeCracked = true;
                    gameEnded = true;
                    EndGame();
                }
            }
            else
            {
                StopCountdown();
                MessageBox.Show("Het spel is al beeindigd, U heeft maximum aantal kansen gebruikt.");
            }
        }












        // Methode om de rand van het label in te stellen
        private void SetLabelBorder(int index, Color borderColor)
        {
            switch (index)
            {
                case 0:
                    label1.BorderBrush = new SolidColorBrush(borderColor);
                    label1.BorderThickness = new Thickness(2);
                    break;
                case 1:
                    label2.BorderBrush = new SolidColorBrush(borderColor);
                    label2.BorderThickness = new Thickness(2);
                    break;
                case 2:
                    label3.BorderBrush = new SolidColorBrush(borderColor);
                    label3.BorderThickness = new Thickness(2);
                    break;
                case 3:
                    label4.BorderBrush = new SolidColorBrush(borderColor);
                    label4.BorderThickness = new Thickness(2);
                    break;
            }
        }



        private void newGameMenu_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
            MessageBox.Show("Een nieuw spel is gestart!", "Nieuw Spel");
        }

        private void highScoreMenu_Click(object sender, RoutedEventArgs e)
        {
            if (highscores.Count == 0)
            {
                MessageBox.Show("Er zijn nog geen highscores geregistreerd.", "Highscores", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Toon de lijst met highscores
                StringBuilder sb = new StringBuilder("Highscores:\n\n");
                for (int i = 0; i < highscores.Count; i++)
                {
                    sb.AppendLine($"{i + 1}. {highscores[i]}");
                }
                MessageBox.Show(sb.ToString(), "Highscores", MessageBoxButton.OK, MessageBoxImage.Information);
            }


        }





        private void closeGameMenu_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Shutdown();

        }






        private void numberOfAttemptsMenu_Click(object sender, RoutedEventArgs e)
        {

            // Open een InputBox om het aantal maximaal toegestane pogingen te vragen
            string input = Interaction.InputBox("Geef het maximum aantal pogingen op (tussen 3 en 20):", "Instellen Maximum Pogingen", "Huidinge max pogingen: " + maxAttempts.ToString()); // Laat huidige waarde zien

            // Controleer of de invoer geldig is
            if (int.TryParse(input, out int tempMaxAttempts) && tempMaxAttempts >= 3 && tempMaxAttempts <= 20)
            {
                maxAttempts = tempMaxAttempts; // Update de maximale pogingen
                MessageBox.Show($"Het maximum aantal pogingen is ingesteld op {maxAttempts}.", "Bevestiging");
                // Werk de UI bij als het spel bezig is
                this.Title = $"Mastermind - Poging: {attempts}/{maxAttempts}";
            }
            else
            {
                MessageBox.Show("Ongeldige invoer. Zorg ervoor dat het getal tussen 3 en 20 is.", "Fout");
            }




        }

        private void colorHintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Weet u zeker dat u een kleur als hint wilt kopen?\nLET OP: Dit kost 15 strafpunten!\n\nDeze hints worden willekeurig gegenereerd en houden niet rekening met vorige hints. Het is dus mogelijk om de zelfde hint meerdere keren te krijgen!", $"Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            AddHighscore(nameInput, attempts, playerScore); // Voeg de highscore toe
            if (result == MessageBoxResult.Yes)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 4);

                MessageBox.Show($"Een juiste kleur is: {secretCode[randomNumber]}", "Hint gekocht!", MessageBoxButton.OK, MessageBoxImage.Warning);
                playerScore -= 15;
                playerScoreTextBox.Text = $"Score: {playerScore}/100   ({nameInput})";

                return;

            }
        }

        private void colorAndPositionHintButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Weet u zeker dat u een kleur + positie als hint wilt kopen?\nLET OP: Dit kost 25 strafpunten!\n\nDeze hints worden willekeurig gegenereerd en houden niet rekening met vorige hints. Het is dus mogelijk om de zelfde hint meerdere keren te krijgen!", $"Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            AddHighscore(nameInput, attempts, playerScore); // Voeg de highscore toe
            if (result == MessageBoxResult.Yes)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 4);
                MessageBox.Show($"Een juiste kleur is: {secretCode[randomNumber]}\nJuiste positie is: {randomNumber+1}", "Hint gekocht!", MessageBoxButton.OK, MessageBoxImage.Warning);
                playerScore -= 25;
                playerScoreTextBox.Text = $"Score: {playerScore}/100   ({nameInput})";

                return;
            }

        }
    }
}