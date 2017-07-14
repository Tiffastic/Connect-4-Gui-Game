using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace Connect4Gui
{

    /**
     * Connect4Gui class simulates the Connect Four game where the connection
     * of 4 same tokens horizontally, vertically, diagonally up, and diagonally down wins the game.
     * 
     * Players can push any enabled 7 buttons to drop their tokens in columns 1-7.  The tokens are dropped in an animated motion.
     * 
     * The label at the top constantly announces the player's turn, or if there is a winner or a tie of the game.
     * 
     * 
     * Thuy Nguyen
     * C# class
     * October 6, 2014
     * 
     */

    public partial class MainWindow : Window
    {

        // ellipses
        private Ellipse[,] ellipses = new Ellipse[6, 7];
        private const int TOKEN_HEIGHT = 91;
        private const int TOKEN_WIDTH = 92;

        // buttons
        private List<Button> buttons = new List<Button>();

        // colors
        private SolidColorBrush blueBlack = new SolidColorBrush();
        private SolidColorBrush red = new SolidColorBrush();
        private SolidColorBrush yellow = new SolidColorBrush();
        private SolidColorBrush player;

        // game
        private string name1, name2;
        private bool turn1 = true;
        private int column, row = 0;

        // timer
        private DispatcherTimer tokenTimer = new DispatcherTimer();
        private DispatcherTimer spaceTimer = new DispatcherTimer();

        // public method
        public MainWindow()
        {
            InitializeComponent();
            MakeBrushes();
            MakeEllipses();
            GroupButtons();
            PreparePlayers("Thuy", "Friend");
            SetTimer();

        }


        // private methods
        private void PreparePlayers(string player1, string player2)
        {
            player = red;
            name1 = player1;
            name2 = player2;
            LabelTurn(name1);
        }

        private void LabelTurn(string name)
        {
            playerLabel.Content = string.Format("{0}'s turn", name);
        }

        private void GroupButtons()
        {
            buttons.Add(button1);
            buttons.Add(button2);
            buttons.Add(button3);
            buttons.Add(button4);
            buttons.Add(button5);
            buttons.Add(button6);
            buttons.Add(button7);
        }

        private void SetTimer()
        {
            tokenTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            tokenTimer.Tick += tokenTimer_Tick;


            spaceTimer.Interval = new TimeSpan(0, 0, 0, 0, 363);
            spaceTimer.Tick += spaceTimer_Tick;
        }

        private void spaceTimer_Tick(object sender, EventArgs e)
        {

            if (!HasSpace(row, column))
            {

                spaceTimer.Stop();
            }
            else
            {
                ellipses[row, column].Fill = blueBlack;
                row++;
            }


        }

        private void tokenTimer_Tick(object sender, EventArgs e)
        {
            DropToken(column);

        }

        private void TakeTurns()
        {
            turn1 = (turn1) ? false : true;
            player = (turn1) ? red : yellow;

            foreach (Button b in buttons)
            {
                b.IsEnabled = true;
                b.Background = turn1 ? red : yellow;
            }

            LabelTurn(turn1 ? name1 : name2);
        }

        private void MakeMove()
        {
            foreach (Button b in buttons)
            {
                b.IsEnabled = false;
            }

            row = 0;
            tokenTimer.Start();
            spaceTimer.Start();
        }

        private void DropToken(int col)
        {


            if (HasSpace(row, col))
            {
                ellipses[row, col].Fill = player;

                if (!HasSpace(row + 1, col))
                {
                    ellipses[row, column].Tag = turn1 ? "Red" : "Yellow";
                }
            }
            else
            {
                tokenTimer.Stop();

                if (HasWon())
                {
                    playerLabel.Content = string.Format("{0} is the winner!", turn1 ? name1 : name2);
                }
                else if (HasTie())
                {
                    playerLabel.Content = string.Format("{0} and {1} has tie!", name1, name2);
                }
                else
                {

                    TakeTurns();
                }

            }

        }

        private bool HasWon()
        {
            for (int i = 0; i < ellipses.GetLength(0); i++)
            {
                for (int j = 0; j < ellipses.GetLength(1); j++)
                {
                    if (!ellipses[i, j].Tag.Equals("space"))
                    {
                        string token = ellipses[i, j].Tag.ToString();


                        if (Connected4Horizontally(i, j, token))
                        {
                            return true;
                        }

                        if (Connected4Vertically(i, j, token))
                        {
                            return true;
                        }

                        if (Connected4DiagonallyUp(i, j, token))
                        {
                            return true;
                        }

                        if (Connected4DiagonallyDown(i, j, token))
                        {
                            return true;
                        }

                    }
                }
            }

            return false;
        }

        private bool HasTie()
        {
            for (int j = 0; j < ellipses.GetLength(1); j++)
            {
                if (ellipses[0, j].Tag.Equals("space"))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Connected(int rowI, int colJ, string token)
        {
            if (rowI >= ellipses.GetLength(0) || rowI < 0 || colJ >= ellipses.GetLength(1))
            {
                return false;
            }

            return ellipses[rowI, colJ].Tag.Equals(token);
        }

        private bool Connected4Horizontally(int rowI, int colJ, string token)
        {
            for (int j = colJ + 1; j < colJ + 4; j++)
            {
                if (!Connected(rowI, j, token))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Connected4Vertically(int rowI, int colJ, string token)
        {
            for (int i = rowI + 1; i < rowI + 4; i++)
            {
                if (!Connected(i, colJ, token))
                {
                    return false;
                }

            }

            return true;
        }

        private bool Connected4DiagonallyDown(int rowI, int colJ, string token)
        {
            for (int i = rowI + 1, j = colJ + 1; i < rowI + 4; i++, j++)
            {
                if (!Connected(i, j, token))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Connected4DiagonallyUp(int rowI, int colJ, string token)
        {
            for (int i = rowI - 1, j = colJ + 1; i > rowI - 4; i--, j++)
            {
                if (!Connected(i, j, token))
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasSpace(int rowI, int colJ)
        {
            if (rowI >= ellipses.GetLength(0))
            {
                return false;
            }

            else
            {
                return ellipses[rowI, colJ].Tag.Equals("space");
            }

        }

        private void MakeBrushes()
        {
            blueBlack = (SolidColorBrush) (new BrushConverter().ConvertFrom("#FF010D23"));

            red = (SolidColorBrush) (new BrushConverter().ConvertFrom("#FFDE2C0F"));

            yellow = (SolidColorBrush) (new BrushConverter().ConvertFrom("#FFF7F706"));

        }

        private void MakeEllipses()
        {
            int top = 30, left = 35;
            int horizontalGap = 22, verticalGap = 15;

            for (int i = 0; i < ellipses.GetLength(0); i++)
            {
                for (int j = 0; j < ellipses.GetLength(1); j++)
                {
                    ellipses[i, j] = new Ellipse();
                    ellipses[i, j].Height = TOKEN_HEIGHT;
                    ellipses[i, j].Width = TOKEN_WIDTH;
                    ellipses[i, j].Fill = blueBlack;  //FF010D23
                    ellipses[i, j].Tag = "space";

                    Canvas.SetTop(ellipses[i, j], top);
                    Canvas.SetLeft(ellipses[i, j], left);
                    CanvasBoard.Children.Add(ellipses[i, j]);

                    left += TOKEN_WIDTH + horizontalGap;
                }

                left = 35;
                top += TOKEN_HEIGHT + verticalGap;
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            column = 0;
            MakeMove();

        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            column = 1;
            MakeMove();

        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            column = 2;
            MakeMove();

        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            column = 3;
            MakeMove();

        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            column = 4;
            MakeMove();

        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            column = 5;
            MakeMove();

        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            column = 6;
            MakeMove();

        }

    }
}
