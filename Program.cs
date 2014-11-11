using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperspaceCheeseBattle
{
    class Program
    {
        const int MAX_BOARD_LIMIT = 7;
        const int MIN_BOARD_LIMIT = 0;
        const int MAX_SIX_POWER = 3;
        static int totPlayers;
        static int[] diceValues = new int[] { 2, 2, 3, 3, 2, 2, 6, 5, 5, 6, 6, 5, 5 }; // Preset dice
        static int diceValuePos = 0; // Preset dice index
        static int diceMode = -1;
        static Player[] players;
        static bool gameOver = false;
        static bool testMode = false;
        static bool UsingSixPower;
        static int SixPowerCount;
        static string newGame;

        static void Main(string[] args)
        {
            Console.Title = "Hyperspace Cheese Battle";
            do
            {
                ResetGame();
                while (!gameOver)
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        PlayerTurn(i);
                        ShowStatus();
                        DrawBoard();
                        if (!gameOver)
                        {
                            Console.WriteLine("Press return for next turns.");
                            Console.ReadLine();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                newGame = ThisOrThat("\nDo you want to start another round? [Y or N]: ", "Invalid value. Enter Y for Yes or N for No.", "Y", "N");
                if (newGame == "N")
                {
                    Console.WriteLine("\n\n\n\nThank you for playing Hyperspace Cheese Battle");
                    Console.WriteLine("Copyright © - All Rights Reserved - 2013 - Federico Dalla Rizza");
                    Console.WriteLine("Press Enter to exit.");
                    Console.ReadLine();
                }
            } while (newGame == "Y");
        }

        /// <summary>
        /// Gets the number of players and set the required elements in players to put each player at square (0,0)
        /// </summary>
        static void ResetGame()
        {
            Console.Clear();
            gameOver = false;
            Console.WriteLine("░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");
            Console.WriteLine("░░░░\t                                    \t░░░░");
            Console.WriteLine("░░░░\tWelcome to Hyperspace Cheese Battle!\t░░░░");
            Console.WriteLine("░░░░\t                                    \t░░░░");
            Console.WriteLine("░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░");
            if (ThisOrThat("\n\nRun the game in test mode? [Y or N]: ", "Invalid value. Enter Y for Yes or N for No.", "Y", "N") == "Y")
            {
                testMode = true;
                diceMode = SelectDiceMethod();
            }
            else
            {
                testMode = false;
            }
            totPlayers = getNumber("\nHow many players? [MAX 4]: ", 2, 4);
            Console.WriteLine();
            players = new Player[totPlayers];
            if (testMode) // Automatic players naming in test mode.
            {
                for (int i = 0; i < players.Length; i++)
                {
                    int n = i + 1;
                    players[i].Name = "Rocket " + n;
                    players[i].X = 0;
                    players[i].Y = 0;
                    switch (i)
                    {
                        case 0:
                            players[i].Color = ConsoleColor.Cyan;
                            break;
                        case 1:
                            players[i].Color = ConsoleColor.Green;
                            break;
                        case 2:
                            players[i].Color = ConsoleColor.Red;
                            break;
                        case 3:
                            players[i].Color = ConsoleColor.Yellow;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].Name = getName(i);
                    players[i].X = 0;
                    players[i].Y = 0;
                    switch (i)
                    {
                        case 0:
                            players[i].Color = ConsoleColor.Cyan;
                            break;
                        case 1:
                            players[i].Color = ConsoleColor.Green;
                            break;
                        case 2:
                            players[i].Color = ConsoleColor.Red;
                            break;
                        case 3:
                            players[i].Color = ConsoleColor.Yellow;
                            break;
                        default:
                            break;
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Gets a valid and unique name.
        /// </summary>
        /// <param name="playerNo">Index of the player.</param>
        /// <returns>String player name.</returns>
        static string getName(int playerNo)
        {
            bool goodName = false;
            string playerName;
            int n = playerNo + 1;
            do
            {
                Console.Write("Enter Player {0} name: ", n);
                playerName = Console.ReadLine();
                for (int i = 0; i < players.Length; i++)
                {
                    if (playerName == players[i].Name)
                    {
                        goodName = false;
                        Console.WriteLine("Error, the name is already being used. Enter a different one.");
                        break;
                    }
                    if (playerName.Length > 20)
                    {
                        goodName = false;
                        Console.WriteLine("Invalid value. The name is too long.");
                        break;
                    }
                    if (playerName == "")
                    {
                        goodName = false;
                        Console.WriteLine("Invalid value. Enter at least 1 character.");
                        break;
                    }
                    goodName = true;
                }
            } while (goodName == false);
            return playerName;
        }

        /// <summary>
        /// Makes the user decide which dice method to use.
        /// </summary>
        /// <returns>Integer 0 for preset mode, 1 for choose value mode, 2 for random mode and normal game</returns>
        static int SelectDiceMethod()
        {
            string diceMethod;
            do
            {
                Console.Write("Which dice method do you want to use? [P]reset, [C]hoose, [R]andom: ");
                diceMethod = Console.ReadLine().ToUpper();
                if (diceMethod != "P" && diceMethod != "C" && diceMethod != "R")
                {
                    Console.WriteLine("Invalid value. Enter P for Preset, C for Choose or R for Random.");
                }
            } while (diceMethod != "P" && diceMethod != "C" && diceMethod != "R");
            switch (diceMethod)
            {
                case "P":
                    return 0;
                case "C":
                    return 1;
                case "R":
                    return 2;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Gets a dice number from 1 to 6. If no dice mode is specified, random mode will be used.
        /// </summary>
        /// <returns>Integer dice number.</returns>
        static int DiceThrow(int diceMode)
        {
            Random rand = new Random();
            switch (diceMode)
            {
                case 0:
                    int spot = diceValues[diceValuePos];
                    diceValuePos += 1;
                    if (diceValuePos == diceValues.Length)
                    {
                        diceValuePos = 0;
                    }
                    return spot;
                case 1:
                    return getNumber("Enter the dice value: ", 1, 6);
                case 2:
                    return rand.Next(1, 7);
                default:
                    return rand.Next(1, 7);
            }
        }

        /// <summary>
        /// Show the position of each player on the board as text report.
        /// </summary>
        static void ShowStatus()
        {
            Console.WriteLine("\nHyperspace Cheese Battle Status Report");
            Console.WriteLine("======================================\n");
            Console.WriteLine("There are {0} players in the game", players.Length);
            for (int i = 0; i < players.Length; i++)
            {
                Console.ForegroundColor = players[i].Color;
                Console.Write("{0} ", players[i].Name);
                Console.ResetColor();
                Console.WriteLine("is on square ({0},{1})", players[i].X, players[i].Y);
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Shows the board with direction arrows, current rockets positions and cheese squares.
        /// </summary>
        static void DrawBoard()
        {
            Console.WriteLine();
            for (int i = MAX_BOARD_LIMIT; i >= MIN_BOARD_LIMIT; i--)
            {
                Console.WriteLine("    │ ");
                Console.Write(" {0}  │ ", i); // Print square index Y on left side
                for (int j = MIN_BOARD_LIMIT; j <= MAX_BOARD_LIMIT; j++)
                {
                    switch (board[j, i])
                    {
                        case Direction.Up:
                            if (RocketInSquare(j, i))
                            {
                                Console.ForegroundColor = players[getPlayerIndex(j, i)].Color;
                                Console.Write("  ▲  ");
                                Console.ResetColor();
                            }
                            else if (CheeseBoard[j, i])
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("  ↑  ");
                                Console.ResetColor();
                            }
                            else
                                Console.Write("  ↑  ");
                            break;
                        case Direction.Down:
                            if (RocketInSquare(j, i))
                            {
                                Console.ForegroundColor = players[getPlayerIndex(j, i)].Color;
                                Console.Write("  ▼  ");
                                Console.ResetColor();
                            }
                            else if (CheeseBoard[j, i])
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("  ↓  ");
                                Console.ResetColor();
                            }
                            else
                                Console.Write("  ↓  ");
                            break;
                        case Direction.Left:
                            if (RocketInSquare(j, i))
                            {
                                Console.ForegroundColor = players[getPlayerIndex(j, i)].Color;
                                Console.Write("  ◄  ");
                                Console.ResetColor();
                            }
                            else if (CheeseBoard[j, i])
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("  ←  ");
                                Console.ResetColor();
                            }
                            else
                                Console.Write("  ←  ");
                            break;
                        case Direction.Right:
                            if (RocketInSquare(j, i))
                            {
                                Console.ForegroundColor = players[getPlayerIndex(j, i)].Color;
                                Console.Write("  ►  ");
                                Console.ResetColor();
                            }
                            else if (CheeseBoard[j, i])
                            {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("  →  ");
                                Console.ResetColor();
                            }
                            else
                                Console.Write("  →  ");
                            break;
                        case Direction.Win:
                            if (RocketInSquare(j, i))
                            {
                                Console.ForegroundColor = players[getPlayerIndex(j, i)].Color;
                                Console.Write("  ☺  ");
                                Console.ResetColor();
                            }
                            else
                                Console.Write("  ▓  ");
                            break;
                        default:
                            break;
                    }
                }
                Console.WriteLine("\n    │ ");
            }
            Console.WriteLine("    │ ");
            for (int i = MIN_BOARD_LIMIT; i <= MAX_BOARD_LIMIT; i++)
            {
                if (i == 0)
                {
                    Console.Write("────┼───");
                }
                else
                {
                    Console.Write("──────");
                }
            }
            Console.WriteLine("\n    │ ");
            Console.Write("    │ ");
            for (int i = MIN_BOARD_LIMIT; i <= MAX_BOARD_LIMIT; i++)
            {
                Console.Write("  {0}  ", i); // Print square index X at bottom
            }
            Console.WriteLine("\n    │ ");
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Gets the player array index from the coordinates X and Y.
        /// </summary>
        /// <param name="x">X position value.</param>
        /// <param name="y">Y position value.</param>
        /// <returns>Integer player index if found, else -1</returns>
        private static int getPlayerIndex(int x, int y)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].X == x && players[i].Y == y)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Shows the player dice throw, checks for collisions or cheese and eventually moves the player on the board.
        /// </summary>
        /// <param name="playerNo">Index of the player.</param>
        private static void PlayerTurn(int playerNo)
        {
            bool bounce = true;
            Console.WriteLine("\nPlayer {0} turn:", players[playerNo].Name);
            int diceN = DiceThrow(diceMode);
            if (diceN == 6)
            {
                SixPowerCount++;
                UsingSixPower = true;
            }
            else
            {
                SixPowerCount = 0;
                UsingSixPower = false;
            }
            Console.WriteLine("{0} rolled {1}", players[playerNo].Name, diceN);
            while (bounce)
            {
                switch (board[players[playerNo].X, players[playerNo].Y]) // gets position X,Y of player on the board
                {
                    case Direction.Up:
                        if (RocketInSquare(players[playerNo].X, players[playerNo].Y + diceN))
                        {
                            bounce = true;
                        }
                        else
                        {
                            bounce = false;
                        }
                        if (players[playerNo].Y + diceN <= MAX_BOARD_LIMIT) // checks if player goes out of the board UP
                        {
                            players[playerNo].Y += diceN;
                            Console.WriteLine("New position: {0},{1}", players[playerNo].X, players[playerNo].Y);
                            if (bounce)
                            {
                                Console.WriteLine("{0} landed on enemy and is bouncing on next space!", players[playerNo].Name);
                                diceN = 1;
                            }
                            else
                            {
                                CheeseInSquare(playerNo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The risk level for that journey is too high!");
                            Console.WriteLine("No movement for player {0}.", players[playerNo].Name);
                        }
                        break;
                    case Direction.Down:
                        if (RocketInSquare(players[playerNo].X, players[playerNo].Y - diceN))
                        {
                            bounce = true;
                        }
                        else
                        {
                            bounce = false;
                        }
                        if (players[playerNo].Y - diceN >= MIN_BOARD_LIMIT) // checks if player goes out of the board DOWN
                        {
                            players[playerNo].Y -= diceN;
                            Console.WriteLine("New position: {0},{1}", players[playerNo].X, players[playerNo].Y);
                            if (bounce)
                            {
                                Console.WriteLine("{0} landed on enemy and is bouncing on next space!", players[playerNo].Name);
                                diceN = 1;
                            }
                            else
                            {
                                CheeseInSquare(playerNo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The risk level for that journey is too high!");
                            Console.WriteLine("No movement for player {0}.", players[playerNo].Name);
                        }
                        break;
                    case Direction.Left:
                        if (RocketInSquare(players[playerNo].X - diceN, players[playerNo].Y))
                        {
                            bounce = true;
                        }
                        else
                        {
                            bounce = false;
                        }
                        if (players[playerNo].X - diceN >= MIN_BOARD_LIMIT) // checks if the players goes out of the board LEFT
                        {
                            players[playerNo].X -= diceN;
                            Console.WriteLine("New position: {0},{1}", players[playerNo].X, players[playerNo].Y);
                            if (bounce)
                            {
                                Console.WriteLine("{0} landed on enemy and is bouncing on next space!", players[playerNo].Name);
                                diceN = 1;
                            }
                            else
                            {
                                CheeseInSquare(playerNo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The risk level for that journey is too high!");
                            Console.WriteLine("No movement for player {0}.", players[playerNo].Name);
                        }
                        break;
                    case Direction.Right:
                        if (RocketInSquare(players[playerNo].X + diceN, players[playerNo].Y))
                        {
                            bounce = true;
                        }
                        else
                        {
                            bounce = false;
                        }
                        if (players[playerNo].X + diceN <= MAX_BOARD_LIMIT) // checks if the player goes out of the board RIGHT
                        {
                            players[playerNo].X += diceN;
                            Console.WriteLine("New position: {0},{1}", players[playerNo].X, players[playerNo].Y);
                            if (bounce)
                            {
                                Console.WriteLine("{0} landed on enemy and is bouncing on next space!", players[playerNo].Name);
                                diceN = 1;
                            }
                            else
                            {
                                CheeseInSquare(playerNo);
                            }
                        }
                        else
                        {
                            Console.WriteLine("The risk level for that journey is too high!");
                            Console.WriteLine("No movement for player {0}.", players[playerNo].Name);
                        }
                        break;
                }
            }                                                                                                                       //                                        ╔═════════════╗
            if (players[playerNo].X == MAX_BOARD_LIMIT && players[playerNo].Y == MAX_BOARD_LIMIT && SixPowerCount != MAX_SIX_POWER) // Print game-over message in the format: ║Player X won!║
            {                                                                                                                       //                                        ╚═════════════╝
                gameOver = true;
                StringBuilder winMsg = new StringBuilder("║Player " + players[playerNo].Name + " won!║");
                Console.Write("╔");
                for (int i = 0; i < winMsg.Length - 2; i++)
                {
                    Console.Write("═");
                }
                Console.WriteLine("╗");
                Console.WriteLine(winMsg);
                Console.Write("╚");
                for (int i = 0; i < winMsg.Length - 2; i++)
                {
                    Console.Write("═");
                }
                Console.WriteLine("╝");
            }
            else if (UsingSixPower)
            {
                bool freeSquare = false;
                int NewPlayerX;
                if (SixPowerCount == MAX_SIX_POWER)
                {
                    Console.WriteLine("Player {0} tried too hard with the Six Power ability!", players[playerNo].Name);
                    Console.WriteLine("The rocket engines has exploded.");
                    Console.WriteLine("\nPlayer {0} decide the position on the bottom line of the board.", players[playerNo].Name);
                    do
                    {
                        NewPlayerX = getNumber("Enter a square between 0 and 7: ", 0, 7);
                        if (RocketInSquare(NewPlayerX, 0))
                        {
                            Console.WriteLine("That square is already occupied. Choose another one.");
                        }
                        else
                        {
                            freeSquare = true;
                            players[playerNo].X = NewPlayerX;
                            players[playerNo].Y = 0;
                        }
                    } while (!freeSquare);
                    Console.WriteLine("New position: {0},{1}", players[playerNo].X, players[playerNo].Y);
                    SixPowerCount = 0;
                    UsingSixPower = false;
                }
                else
                {
                    Console.WriteLine("\nPlayer {0}, you will now use the Six Power ability level {1}.", players[playerNo].Name, SixPowerCount);
                    Console.WriteLine("Prepare for another turn!");
                    ShowStatus();
                    DrawBoard();
                    Console.WriteLine("Press return for next turns.");
                    Console.ReadLine();
                    PlayerTurn(playerNo);
                }
            }
        }

        /// <summary>
        /// Checks if the square is occupied by another rocket.
        /// </summary>
        /// <param name="X">Position X of the square to check.</param>
        /// <param name="Y">Position Y of the square to check.</param>
        /// <returns>true if the square is occupied by another rocket.</returns>
        static bool RocketInSquare(int X, int Y)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].X == X && players[i].Y == Y)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if player is on a cheese square and eventually asks the player what to do with the cheese power (throw dice again or attack an enemy player).
        /// If the player decides to throw again, another playerTurn is called.
        /// If the player decides to attack, chosen enemy can decide a free bottom board position.
        /// </summary>
        /// <param name="playerNo">Index of the player.</param>
        static void CheeseInSquare(int playerNo)
        {
            if (!(CheeseBoard[players[playerNo].X, players[playerNo].Y])) // Player not on Cheese square
            {
                return;
            }
            string decision;
            Console.WriteLine("Player {0} has landed on a Cheese Power Square.", players[playerNo].Name);
            if (!UsingSixPower)
            {
                Console.WriteLine("Does {0} want to roll the dice again or explode the engines of another rocket?", players[playerNo].Name);
                decision = ThisOrThat("Enter Throw or Explode [T or E]: ", "Invalid value. Enter T to Throw or E to Explode.", "T", "E");
                if (decision == "T") // decision Throw dice
                {
                    PlayerTurn(playerNo);
                }
                else // decision Explode
                {
                    bool exploded = false;
                    do
                    {
                        for (int i = 0; i < players.Length; i++)
                        {
                            if (players[i].Name == players[playerNo].Name)
                            {
                                continue;
                            }
                            Console.WriteLine("Do you want to explode player {0}?", players[i].Name);
                            decision = ThisOrThat("Enter Yes or No [Y or N]: ", "Invalid value. Enter Y for Yes or N for No.", "Y", "N");
                            if (decision == "Y") // decision Yes
                            {
                                bool freeSquare = false;
                                int NewPlayerX;
                                Console.WriteLine("{0} engines has exploded.", players[i].Name);
                                Console.WriteLine("\nPlayer {0} decide the position on the bottom line of the board.", players[i].Name);
                                do
                                {
                                    NewPlayerX = getNumber("Enter a square between 0 and 7: ", 0, 7);
                                    if (RocketInSquare(NewPlayerX, 0))
                                    {
                                        Console.WriteLine("That square is already occupied. Choose another one.");
                                    }
                                    else
                                    {
                                        freeSquare = true;
                                        players[i].X = NewPlayerX;
                                        players[i].Y = 0;
                                    }
                                } while (!freeSquare);
                                Console.WriteLine("New position: {0},{1}", players[i].X, players[i].Y);
                                exploded = true;
                                break;
                            }
                            else // decision No
                            {
                                continue;
                            }
                        }
                    } while (exploded == false);
                }
            }
            else
            {
                Console.WriteLine("Since you are using the Six Power ability, you MUST explode an enemy rocket!");
                bool exploded = false;
                bool freeSquare = false;
                int SquareNumber;
                do
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].Name == players[playerNo].Name)
                        {
                            continue;
                        }
                        Console.WriteLine("Do you want to explode player {0}?: ", players[i].Name);
                        decision = ThisOrThat("Enter Yes or No [Y or N]: ", "Invalid value. Enter Y for Yes or N for No.", "Y", "N");
                        if (decision == "Y") // decision Yes
                        {
                            Console.WriteLine("{0} engines has exploded.", players[i].Name);
                            while (freeSquare == false)
                            {
                                Console.WriteLine("\nPlayer {0} decide the position on the bottom line of the board.", players[i].Name);
                                SquareNumber = getNumber("Enter a square between 0 and 7: ", 0, 7);
                                if (RocketInSquare(SquareNumber, 0))
                                {
                                    Console.WriteLine("That square is already occupied, choose another one.");
                                }
                                else
                                {
                                    players[i].X = SquareNumber;
                                    players[i].Y = 0;
                                    Console.WriteLine("New position: {0},{1}", players[i].X, players[i].Y);
                                    freeSquare = true;
                                    exploded = true;
                                }
                            }
                            break;
                        }
                        else // decision No
                        {
                            continue;
                        }
                    }
                } while (exploded == false);
            }
        }

        /// <summary>
        /// Gets a number in range from min to max after prompting a message.
        /// </summary>
        /// <param name="prompt">Message for the user to show.</param>
        /// <param name="min">Minimum inclusive value.</param>
        /// <param name="max">Maximum inclusive value.</param>
        /// <returns>Integer number in the range.</returns>
        static int getNumber(string prompt, int min, int max)
        {
            if (min >= max)
            {
                throw new Exception("Invalid range, min value must be smaller than max value.");
            }
            int number;
            while (true)
            {
                Console.Write(prompt);
                try
                {
                    number = int.Parse(Console.ReadLine());
                    if (number < min || number > max)
                    {
                        Console.WriteLine("Number must be in the range between {0} and {1}.", min, max);
                        continue;
                    }
                    break;

                }
                catch
                {
                    Console.WriteLine("Enter a numeric value in the range {0} to {1}", min, max);
                    continue;
                }
            }
            return number;
        }

        /// <summary>
        /// Gets a one digit answer to the prompt.
        /// </summary>
        /// <param name="prompt">Message for the user to show.</param>
        /// <param name="errorMsg">Error message for invalid input.</param>
        /// <param name="answerOne">First possible answer.</param>
        /// <param name="answerTwo">Second possible answer.</param>
        /// <returns>String containing one answer character.</returns>
        static string ThisOrThat(string prompt, string errorMsg, string answerOne, string answerTwo)
        {
            if (answerOne == answerTwo)
            {
                throw new Exception("Invalid values, answers can not be the same.");
            }
            string answer;
            while (true)
            {
                Console.Write(prompt);
                answer = Console.ReadLine().ToUpper();
                if (answer != answerOne && answer != answerTwo)
                {
                    Console.WriteLine(errorMsg);
                }
                else
                {
                    return answer;
                }
            }
        }

        /// <summary>
        /// Variables to define arrow direction on the board squares.
        /// </summary>
        enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            Win
        };

        static Direction[,] board = new Direction[,]
        {
            {Direction.Up,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Down}, // column 0
            {Direction.Up,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Right,Direction.Right}, // column 1
            {Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Right,Direction.Up,Direction.Right}, // column 2
            {Direction.Up,Direction.Down,Direction.Left,Direction.Right,Direction.Right,Direction.Right,Direction.Down,Direction.Right}, // column 3
            {Direction.Up,Direction.Up,Direction.Left,Direction.Up,Direction.Up,Direction.Up,Direction.Up,Direction.Right}, // column 4
            {Direction.Up,Direction.Up,Direction.Left,Direction.Up,Direction.Up,Direction.Up,Direction.Left,Direction.Right}, // column 5
            {Direction.Up,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Down}, // column 6
            {Direction.Up,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Left,Direction.Win}, // column 7
        };

        static bool[,] CheeseBoard = new bool[,]
        {
            {false,false,false,true,false,false,false,false}, // column 0
            {false,false,false,false,false,false,false,false}, // column 1
            {false,false,false,false,false,false,false,false}, // column 2
            {false,false,false,false,false,true,false,false}, // column 3
            {false,true,false,false,false,false,false,false}, // column 4
            {false,false,false,false,false,false,false,false}, // column 5
            {false,false,false,false,true,false,false,false}, // column 6
            {false,false,false,false,false,false,false,false}, // column 7
        };

        /// <summary>
        /// Variables container for Player type including player name, square position, font color.
        /// </summary>
        struct Player
        {
            public string Name;
            public int X;
            public int Y;
            public ConsoleColor Color;
        }
    }
}