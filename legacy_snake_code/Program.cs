using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Snake
{
    class Game
    {
        private const int WindowHeight = 16;
        private const int WindowWidth = 32;
        private const int InitialScore = 5;
        private const int MoveDelayMs = 200;

        private Renderer renderer;
        private Snake snake;
        private Berry berry;
        private bool gameover;
        private int score;
        private Direction currentDirection;
        private Random random;

        public Game()
        {
            ConsoleSetup();
            renderer = new Renderer(WindowWidth, WindowHeight);
            random = new Random();
            snake = new Snake(WindowWidth / 2, WindowHeight / 2, 3);
            berry = new Berry(random.Next(1, WindowWidth - 2), random.Next(1, WindowHeight - 2));
            score = InitialScore;
            currentDirection = Direction.Right;
            gameover = false;
        }

        private void ConsoleSetup()
        {
            Console.WindowHeight = WindowHeight;
            Console.WindowWidth = WindowWidth;
            Console.CursorVisible = false;
        }

        public void Run()
        {
            // První krok bez čekání
            snake.Move(currentDirection);

            while (!gameover)
            {
                Console.Clear();
                renderer.DrawBorder();
                renderer.DrawSnake(snake);
                renderer.DrawBerry(berry);

                if (snake.CheckCollision(WindowWidth, WindowHeight))
                {
                    gameover = true;
                    break;
                }

                if (berry.CheckCollision(snake.Head))
                {
                    score++;
                    berry.Respawn(random, WindowWidth, WindowHeight);
                    snake.Grow();
                }

                DelayMovement();
                snake.Move(currentDirection);
            }

            renderer.DisplayGameOver(score - InitialScore);
        }

        private void DelayMovement()
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds <= MoveDelayMs)
            {
                currentDirection = ReadMovement(currentDirection);
            }
        }

        private Direction ReadMovement(Direction currentDirection)
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
    }

    class Snake
    {
        public List<SnakeSegment> Body { get; private set; }
        public SnakeSegment Head => Body[^1];

        public Snake(int startX, int startY, int initialSize)
        {
            Body = new List<SnakeSegment>();
            for (int i = 0; i < initialSize; i++)
            {
                Body.Add(new SnakeSegment(startX - i, startY, i == initialSize - 1 ? ConsoleColor.Red : ConsoleColor.Green));
            }
        }

        public void Move(Direction direction)
        {
            var newHead = direction switch
            {
                Direction.Up => new SnakeSegment(Head.X, Head.Y - 1, ConsoleColor.Red),
                Direction.Down => new SnakeSegment(Head.X, Head.Y + 1, ConsoleColor.Red),
                Direction.Left => new SnakeSegment(Head.X - 1, Head.Y, ConsoleColor.Red),
                Direction.Right => new SnakeSegment(Head.X + 1, Head.Y, ConsoleColor.Red),
                _ => Head
            };

            if (Body.Exists(segment => segment.X == newHead.X && segment.Y == newHead.Y))
            {
                return;
            }

            Body.Add(newHead);
            Body.RemoveAt(0);

            // Nastavení barev, aby hlava byla vždy červená
            for (int i = 0; i < Body.Count - 1; i++)
            {
                Body[i] = new SnakeSegment(Body[i].X, Body[i].Y, ConsoleColor.Green);
            }
            Body[^1] = new SnakeSegment(Body[^1].X, Body[^1].Y, ConsoleColor.Red);
        }

        public void Grow()
        {
            Body.Insert(0, new SnakeSegment(Body[0].X, Body[0].Y, ConsoleColor.Green));
        }

        public bool CheckCollision(int width, int height)
        {
            return Head.X == width - 1 || Head.X == 0 || Head.Y == height - 1 || Head.Y == 0;
        }
    }

    class Berry
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Berry(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool CheckCollision(SnakeSegment head) => head.X == X && head.Y == Y;

        public void Respawn(Random random, int width, int height)
        {
            X = random.Next(1, width - 2);
            Y = random.Next(1, height - 2);
        }

        public void Draw()
        {
            Console.SetCursorPosition(X, Y);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("■");
        }
    }

    class Renderer
    {
        private int width, height;

        public Renderer(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, height - 1);
                Console.Write("■");
            }
            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(width - 1, i);
                Console.Write("■");
            }
        }

        public void DrawSnake(Snake snake)
        {
            foreach (var segment in snake.Body)
            {
                Console.SetCursorPosition(segment.X, segment.Y);
                Console.ForegroundColor = segment.Color;
                Console.Write("■");
            }
        }

        public void DrawBerry(Berry berry)
        {
            berry.Draw();
        }

        public void DisplayGameOver(int score)
        {
            Console.SetCursorPosition(width / 5, height / 2);
            Console.WriteLine($"Game Over! Score: {score}");
            Console.SetCursorPosition(width / 5, height / 2 + 1);
            Console.ReadKey();
        }
    }

    class SnakeSegment
    {
        public int X { get; }
        public int Y { get; }
        public ConsoleColor Color { get; }

        public SnakeSegment(int x, int y, ConsoleColor color)
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

    class Program
    {
        static void Main()
        {
            new Game().Run();
        }
    }
}
