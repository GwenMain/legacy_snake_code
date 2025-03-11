using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            var head = new SnakeSegment(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var fruit = GenerateFruit(random);
            var body = new List<SnakeSegment>();
            var currentDirection = Direction.Right;
            bool gameover = false;

            int moveDelay = MoveDelayMs;

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

                head.Draw();
                fruit.Draw();

                DelayMovement(ref currentDirection, moveDelay);

                body.Add(new SnakeSegment(head.X, head.Y, ConsoleColor.Green));
                MoveHead(ref head, currentDirection);

                if (body.Count > score)
                {
                    body.RemoveAt(0);
                }
            }

            DisplayGameOver(score);
            Console.CursorVisible = true;
        }

        private static bool CheckCollision(SnakeSegment head) =>
            head.X == WindowWidth - 1 || head.X == 0 || head.Y == WindowHeight - 1 || head.Y == 0;

        private static bool CheckFruitCollision(ref SnakeSegment head, ref Fruit fruit, Random random)
        {
            if (fruit.X == head.X && fruit.Y == head.Y)
            {
                fruit = GenerateFruit(random);
                return true;
            }
            return false;
        }

        private static Fruit GenerateFruit(Random random) =>
            new Fruit(random.Next(1, WindowWidth - 2), random.Next(1, WindowHeight - 2));

        private static void DrawSnake(List<SnakeSegment> body, ref bool gameover, SnakeSegment head)
        {
            foreach (var segment in body)
            {
                segment.Draw();
                if (segment.X == head.X && segment.Y == head.Y)
                {
                    gameover = true;
                }
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

        private static void MoveHead(ref SnakeSegment head, Direction direction)
        {
            head = direction switch
            {
                Direction.Up => new SnakeSegment(head.X, head.Y - 1, head.Color),
                Direction.Down => new SnakeSegment(head.X, head.Y + 1, head.Color),
                Direction.Left => new SnakeSegment(head.X - 1, head.Y, head.Color),
                Direction.Right => new SnakeSegment(head.X + 1, head.Y, head.Color),
                _ => head
            };
        }
    }

    abstract class GameObject
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public ConsoleColor Color { get; protected set; }

        public GameObject(int x, int y, ConsoleColor color)
        {
            X = x;
            Y = y;
            Color = color;
        }

        public virtual void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.ForegroundColor = Color;
            Console.Write("■");
        }
    }

    class Fruit : GameObject
    {
        public Fruit(int x, int y) : base(x, y, ConsoleColor.DarkYellow) { }
    }

    class SnakeSegment : GameObject
    {
        public SnakeSegment(int x, int y, ConsoleColor color) : base(x, y, color) { }
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
