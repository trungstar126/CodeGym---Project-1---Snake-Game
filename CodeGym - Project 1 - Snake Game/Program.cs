using System;

public class Program
{
    private const int Cols = 30;
    private const int Rows = 20;

    private const string Horizontal = "~";
    private const string Vertical = "|";
    private const string Corner = "+";
    private const string tileChar = " ";

    private const string SnakeChar = "O";
    private const string FruitChar = "*";

    private static string[,] _border = new string[Cols, Rows];
    private static int fruitAquired;
    private static readonly Random _rnd = new Random();

    public static void Main()
    {
        Console.Clear();
        while (Run()) { }
        Console.WriteLine("Game Over!");
    }

    private static bool Run()
    {
        var snake = new (int X, int Y)[1];
        snake[0] = (1, 1);

        _border = new string[Cols, Rows];
        fruitAquired = 0;

        // Populate border
        for (int i = 0; i < Cols; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                if ((i == 0 && j == 0) || (i == 0 && j == Rows - 1) || (i == Cols - 1 && j == 0) || (i == Cols - 1 && j == Rows - 1))
                {
                    _border[i, j] = Corner;
                }
                else if (j == 0 || j == Rows - 1)
                {
                    _border[i, j] = Horizontal;
                }
                else if (i == 0 || i == Cols - 1)
                {
                    _border[i, j] = Vertical;
                }
                else
                {
                    _border[i, j] = tileChar;
                }
            }
        }

        // Place fruit (ensure it's placed on an empty tile)
        (int X, int Y) fruitPosition;
        do
        {
            fruitPosition = (X: _rnd.Next(1, Cols - 1), Y: _rnd.Next(1, Rows - 1));
        } while (_border[fruitPosition.X, fruitPosition.Y] != tileChar);
        _border[fruitPosition.X, fruitPosition.Y] = FruitChar;

        // Place snake head
        foreach (var segment in snake)
        {
            _border[segment.X, segment.Y] = SnakeChar;
        }

        // Game loop
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Welcome to Snake Game");
            Console.WriteLine($"Fruit Aquired: {fruitAquired}");

            // Update border display
            for (int j = 0; j < Rows; j++)
            {
                for (int i = 0; i < Cols; i++)
                {
                    Console.Write(_border[i, j]);
                }
                Console.WriteLine();
            }

            // End conditions
            if (fruitAquired == -1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Game Over! You collided with yourself or a wall.");
                Console.ResetColor();
                Console.WriteLine("Press R to Restart or ESC to Exit.");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.R)
                    {
                        return true;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        return false;
                    }
                }
            }
            else if (fruitAquired >= 10)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You Win!");
                Console.ResetColor();
                Console.WriteLine("Press R to Restart or ESC to Exit.");
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.R)
                    {
                        return true;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        return false;
                    }
                }
            }

            // Get user input
            Console.WriteLine("Use Arrow Keys to move the snake. Press ESC to quit.");
            var input = Console.ReadKey(true);
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    Move(0, -1, ref snake);
                    break;
                case ConsoleKey.DownArrow:
                    Move(0, 1, ref snake);
                    break;
                case ConsoleKey.LeftArrow:
                    Move(-1, 0, ref snake);
                    break;
                case ConsoleKey.RightArrow:
                    Move(1, 0, ref snake);
                    break;
                case ConsoleKey.Escape:
                    return false;
                case ConsoleKey.R:
                    return true;
            }
        }
    }

    private static void Move(int x, int y, ref (int X, int Y)[] snake)
    {
        var newX = snake[0].X + x;
        var newY = snake[0].Y + y;

        // Check for collisions with walls -> game over
        if (newX <= 0 || newX >= Cols - 1 || newY <= 0 || newY >= Rows - 1)
        {
            fruitAquired = -1;
            return;
        }

        // Check for collisions 
        for (int i = 1; i < snake.Length; i++)
        {
            if (snake[i].X == newX && snake[i].Y == newY)
            {
                fruitAquired = -1;
                return;
            }
        }

        // Check for fruit
        
        if (_border[newX, newY] == FruitChar)
        {
            fruitAquired++;

            // Place new fruit on an empty tile
            (int X, int Y) fruitPosition;
            do
            {
                fruitPosition = (X: _rnd.Next(1, Cols - 1), Y: _rnd.Next(1, Rows - 1));
            } while (_border[fruitPosition.X, fruitPosition.Y] != tileChar);
            _border[fruitPosition.X, fruitPosition.Y] = FruitChar;

            // Grow the snake
            int oldLen = snake.Length;
            Array.Resize(ref snake, oldLen + 1);
            if (oldLen >= 1)
            {
                snake[snake.Length - 1] = snake[oldLen - 1];
            }
        }

        // Clear current snake segments 
        foreach (var seg in snake)
        {
            _border[seg.X, seg.Y] = tileChar;
        }

        // Move snake segments
        for (int i = snake.Length - 1; i >= 1; i--)
        {
            snake[i] = snake[i - 1];
        }
        snake[0] = (newX, newY);

        // Draw snake
        foreach (var seg in snake)
        {
            _border[seg.X, seg.Y] = SnakeChar;
        }
    }
}
