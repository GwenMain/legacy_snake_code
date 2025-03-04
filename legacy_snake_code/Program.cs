using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Snake
{
    class Program
    {
        private const int WindowHeight = 16;
        private const int WindowWidth = 32;
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
        }

        private static void RunGame()
        {
            var random = new Random();
            int score = InitialScore;

            var head = new Pixel(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var berry = GenerateBerry(random);
            var body = new List<Pixel>();
            var currentDirection = Direction.Right;
            bool gameover = false;

            while (!gameover)
            {
                Console.Clear();
                DrawBorder();

                if (CheckCollision(head))
                {
                    gameover = true;
                    break;
                }

                if (CheckBerryCollision(ref head, ref berry, random))
                {
                    score++;
                }

                DrawSnake(body, ref gameover, head);
                if (gameover) break;

                DrawPixel(head);
                DrawPixel(berry);

                DelayMovement(ref currentDirection);

                body.Add(new Pixel(head.X, head.Y, ConsoleColor.Green));
                MoveHead(ref head, currentDirection);

                if (body.Count > score)
                {
                    body.RemoveAt(0);
                }
            }

            DisplayGameOver(score);
        }

        private static bool CheckCollision(Pixel head) =>
            head.X == WindowWidth - 1 || head.X == 0 || head.Y == WindowHeight - 1 || head.Y == 0;

        private static bool CheckBerryCollision(ref Pixel head, ref Pixel berry, Random random)
        {
            if (berry.X == head.X && berry.Y == head.Y)
            {
                berry = GenerateBerry(random);
                return true;
            }
            return false;
        }

        private static Pixel GenerateBerry(Random random) =>
            new Pixel(random.Next(1, WindowWidth - 2), random.Next(1, WindowHeight - 2), ConsoleColor.Cyan);

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
}
