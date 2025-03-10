using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Snake
{
    class Program
    {
        private static int WindowHeight = 16;
        private static int WindowWidth = 32;
        private const int InitialScore = 5;
        private const int MoveDelayMs = 500;

        static void Main()
        {
            ConsoleSetup();
            RunGame();
        }




        private static void ConsoleSetup()
        {
            Console.WindowHeight = WindowHeight;
            Console.WindowWidth = WindowWidth;
            Console.CursorVisible = false;
        }

        private static void RunGame()
        {
            Console.CursorVisible = false;

            var random = new Random();
            int score = InitialScore;
            var head = new Pixel(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var fruit = GenerateFruit(random);
            var body = new List<Pixel>();
            var currentDirection = Direction.Right;
            bool gameover = false;

            int moveDelay = MoveDelayMs; // Původní pohybová zpoždění

            while (!gameover)
            {
                Console.Clear();
                DrawBorder();

                if (CheckCollision(head))
                {
                    gameover = true;
                    break;
                }

                if (CheckFruitCollision(ref head, ref fruit, random))
                {
                    score++;
                }

                DrawSnake(body, ref gameover, head);
                if (gameover) break;

                DrawPixel(head);
                fruit.Draw();

                DelayMovement(ref currentDirection, moveDelay); // Použití původního moveDelay

                body.Add(new Pixel(head.X, head.Y, ConsoleColor.Green));
                MoveHead(ref head, currentDirection);

                if (body.Count > score)
                {
                    body.RemoveAt(0);
                }
            }

            DisplayGameOver(score);
            Console.CursorVisible = true;
        }

        private static bool CheckCollision(Pixel head) =>
            head.X == WindowWidth - 1 || head.X == 0 || head.Y == WindowHeight - 1 || head.Y == 0;

        private static bool CheckFruitCollision(ref Pixel head, ref Fruit fruit, Random random)
        {
            if (fruit.XPos == head.X && fruit.YPos == head.Y)
            {
                fruit = GenerateFruit(random);
                return true;
            }
            return false;
        }

        private static Fruit GenerateFruit(Random random) =>
            new Fruit(random.Next(1, WindowWidth - 2), random.Next(1, WindowHeight - 2), ConsoleColor.DarkYellow);


        private static void DrawSnake(List<Pixel> body, ref bool gameover, Pixel head)
        {
            foreach (var segment in body)
            {
                DrawPixel(segment);
                if (segment.X == head.X && segment.Y == head.Y)
                {
                    gameover = true;
                }
            }
        }

        private static void DelayMovement(ref Direction direction)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds <= MoveDelayMs)
            {
                direction = ReadMovement(direction);
            }
        }

        private static void DisplayGameOver(int score)
        {
            Console.SetCursorPosition(WindowWidth / 5, WindowHeight / 2);
            Console.WriteLine($"Game Over! Score: {score - InitialScore}");
            Console.SetCursorPosition(WindowWidth / 5, WindowHeight / 2 + 1);
            Console.ReadKey();
        }

        private static void DelayMovement(ref Direction direction, int moveDelay)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds <= moveDelay)
            {
                direction = ReadMovement(direction);
            }
        }

        private static Direction ReadMovement(Direction currentDirection)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                currentDirection = key switch
                {
                    ConsoleKey.UpArrow when currentDirection != Direction.Down => Direction.Up,
                    ConsoleKey.DownArrow when currentDirection != Direction.Up => Direction.Down,
                    ConsoleKey.LeftArrow when currentDirection != Direction.Right => Direction.Left,
                    ConsoleKey.RightArrow when currentDirection != Direction.Left => Direction.Right,
                    _ => currentDirection
                };
            }
            return currentDirection;
        }

        private static void DrawPixel(Pixel pixel)
        {
            Console.SetCursorPosition(pixel.X, pixel.Y);
            Console.ForegroundColor = pixel.Color;
            Console.Write("■");
            Console.SetCursorPosition(0, 0);
        }

        private static void DrawBorder()
        {

            Console.ForegroundColor = ConsoleColor.Cyan;

            for (int i = 0; i < WindowWidth; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, WindowHeight - 1);
                Console.Write("■");
            }
            for (int i = 0; i < WindowHeight; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(WindowWidth - 1, i);
                Console.Write("■");
            }
        }


        private static void MoveHead(ref Pixel head, Direction direction)
        {
            head = direction switch
            {
                Direction.Up => new Pixel(head.X, head.Y - 1, head.Color),
                Direction.Down => new Pixel(head.X, head.Y + 1, head.Color),
                Direction.Left => new Pixel(head.X - 1, head.Y, head.Color),
                Direction.Right => new Pixel(head.X + 1, head.Y, head.Color),
                _ => head
            };
        }
    }

    class Fruit
    {
        public int XPos { get; }
        public int YPos { get; }
        public ConsoleColor Color { get; }

        public Fruit(int xPos, int yPos, ConsoleColor color)
        {
            XPos = xPos;
            YPos = yPos;
            Color = color;
        }

        public void Draw()
        {
            Console.SetCursorPosition(XPos, YPos);
            Console.ForegroundColor = Color;
            Console.Write("■");
        }
    }

    struct Pixel
    {
        public int X { get; }
        public int Y { get; }
        public ConsoleColor Color { get; }

        public Pixel(int x, int y, ConsoleColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
