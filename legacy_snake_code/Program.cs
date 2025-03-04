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
            GameEngine game = new GameEngine(WindowWidth, WindowHeight, InitialScore, MoveDelayMs);
            game.Run();
        }
    }

    class GameEngine
    {
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private readonly int _initialScore;
        private readonly int _moveDelayMs;
        private Renderer _renderer;
        private InputHandler _inputHandler;

        public GameEngine(int width, int height, int initialScore, int moveDelayMs)
        {
            _windowWidth = width;
            _windowHeight = height;
            _initialScore = initialScore;
            _moveDelayMs = moveDelayMs;
            _renderer = new ConsoleRenderer(width, height);
            _inputHandler = new ConsoleInputHandler();
        }

        public void Run()
        {
            var random = new Random();
            int score = _initialScore;

            var head = new Pixel(_windowWidth / 2, _windowHeight / 2, ConsoleColor.Red);
            var fruit = GenerateFruit(random);
            var body = new List<Pixel>();
            var currentDirection = Direction.Right;
            bool gameover = false;

            while (!gameover)
            {
                _renderer.Clear();
                _renderer.DrawBorder();

                if (CheckCollision(head))
                {
                    gameover = true;
                    break;
                }

                if (CheckFruitCollision(ref head, ref fruit, random))
                {
                    score++;
                }

                _renderer.DrawSnake(body, ref gameover, head);
                if (gameover) break;

                _renderer.DrawPixel(head);
                _renderer.DrawPixel(new Pixel(fruit.XPos, fruit.YPos, fruit.Color));

                DelayMovement(ref currentDirection);
                body.Add(new Pixel(head.X, head.Y, ConsoleColor.Green));
                head = MoveHead(head, currentDirection);

                if (body.Count > score)
                {
                    body.RemoveAt(0);
                }
            }

            _renderer.DisplayGameOver(score - _initialScore);
        }

        private bool CheckCollision(Pixel head) =>
            head.X == _windowWidth - 1 || head.X == 0 || head.Y == _windowHeight - 1 || head.Y == 0;

        private bool CheckFruitCollision(ref Pixel head, ref Fruit fruit, Random random)
        {
            if (fruit.XPos == head.X && fruit.YPos == head.Y)
            {
                fruit = GenerateFruit(random);
                return true;
            }
            return false;
        }

        private Fruit GenerateFruit(Random random) =>
            new Fruit(random.Next(1, _windowWidth - 2), random.Next(1, _windowHeight - 2), ConsoleColor.DarkYellow);

        private void DelayMovement(ref Direction direction)
        {
            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds <= _moveDelayMs)
            {
                direction = _inputHandler.ReadMovement(direction);
            }
        }

        private Pixel MoveHead(Pixel head, Direction direction) =>
            direction switch
            {
                Direction.Up => new Pixel(head.X, head.Y - 1, head.Color),
                Direction.Down => new Pixel(head.X, head.Y + 1, head.Color),
                Direction.Left => new Pixel(head.X - 1, head.Y, head.Color),
                Direction.Right => new Pixel(head.X + 1, head.Y, head.Color),
                _ => head
            };
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
    }

    interface Renderer
    {
        void Clear();
        void DrawBorder();
        void DrawPixel(Pixel pixel);
        void DrawSnake(List<Pixel> body, ref bool gameover, Pixel head);
        void DisplayGameOver(int score);
    }

    class ConsoleRenderer : Renderer
    {
        private int _width;
        private int _height;

        public ConsoleRenderer(int width, int height)
        {
            _width = width;
            _height = height;
            Console.WindowHeight = height;
            Console.WindowWidth = width;
            Console.CursorVisible = false;
        }

        public void Clear() => Console.Clear();

        public void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < _width; i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("■");
                Console.SetCursorPosition(i, _height - 1);
                Console.Write("■");
            }
            for (int i = 0; i < _height; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write("■");
                Console.SetCursorPosition(_width - 1, i);
                Console.Write("■");
            }
        }

        public void DrawPixel(Pixel pixel)
        {
            Console.SetCursorPosition(pixel.X, pixel.Y);
            Console.ForegroundColor = pixel.Color;
            Console.Write("■");
        }

        public void DrawSnake(List<Pixel> body, ref bool gameover, Pixel head)
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

        public void DisplayGameOver(int score)
        {
            Console.SetCursorPosition(_width / 5, _height / 2);
            Console.WriteLine($"Game Over! Score: {score}");
            Console.ReadKey();
        }
    }

    interface InputHandler
    {
        Direction ReadMovement(Direction currentDirection);
    }

    class ConsoleInputHandler : InputHandler
    {
        public Direction ReadMovement(Direction currentDirection)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                return key switch
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

    enum Direction { Up, Down, Left, Right }

    struct Pixel { public int X, Y; public ConsoleColor Color; public Pixel(int x, int y, ConsoleColor color) { X = x; Y = y; Color = color; } }
}