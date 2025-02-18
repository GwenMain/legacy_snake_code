using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Console;

namespace Snake
{
    class Program
    {
        static void Main()
        {
            WindowHeight = 16;
            WindowWidth = 32;

            var rand = new Random();
            var score = 5;

            // Inicializace hlavy a ovoce
            var head = new Pixel(WindowWidth / 2, WindowHeight / 2, ConsoleColor.Red);
            var berry = new Pixel(rand.Next(1, WindowWidth - 2), rand.Next(1, WindowHeight - 2), ConsoleColor.Cyan);

            var body = new List<Pixel>();
            var currentMovement = Direction.Right;
            var gameover = false;

            // Hlavní herní smyčka
            while (!gameover)
            {
                Clear();

                // Zkontroluj kolize se stěnami
                gameover |= (head.XPos == WindowWidth - 1 || head.XPos == 0 || head.YPos == WindowHeight - 1 || head.YPos == 0);

                // Vykresli hranice
                DrawBorder();

                // Zkontroluj, zda hráč sežral ovoce
                if (berry.XPos == head.XPos && berry.YPos == head.YPos)
                {
                    score++;
                    berry = new Pixel(rand.Next(1, WindowWidth - 2), rand.Next(1, WindowHeight - 2), ConsoleColor.Cyan);
                }

                // Vykresli tělo hada a kontroluj kolize s ním
                for (int i = 0; i < body.Count; i++)
                {
                    DrawPixel(body[i]);
                    gameover |= (body[i].XPos == head.XPos && body[i].YPos == head.YPos);
                }

                // Pokud je hra u konce, ukonči smyčku
                if (gameover)
                    break;

                // Vykresli hlavu a ovoce
                DrawPixel(head);
                DrawPixel(berry);

                // Zpoždění pohybu hada (500ms)
                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds <= 500)
                {
                    currentMovement = ReadMovement(currentMovement);
                }

                // Přidej novou pozici těla
                body.Add(new Pixel(head.XPos, head.YPos, ConsoleColor.Green));

                // Pohyb hada podle aktuálního směru
                MoveHead(ref head, currentMovement);

                // Pokud délka těla přesáhne skóre, odstraň první část
                if (body.Count > score)
                {
                    body.RemoveAt(0);
                }
            }

            // Vykresli Game Over
            SetCursorPosition(WindowWidth / 5, WindowHeight / 2);
            WriteLine($"Game over, Score: {score - 5}");
            SetCursorPosition(WindowWidth / 5, WindowHeight / 2 + 1);
            ReadKey();
        }

        // Funkce pro čtení pohybu
        static Direction ReadMovement(Direction movement)
        {
            if (KeyAvailable)
            {
                var key = ReadKey(true).Key;

                // Umožní změnu směru pouze na platný směr (ne opačný)
                if (key == ConsoleKey.UpArrow && movement != Direction.Down)
                {
                    movement = Direction.Up;
                }
                else if (key == ConsoleKey.DownArrow && movement != Direction.Up)
                {
                    movement = Direction.Down;
                }
                else if (key == ConsoleKey.LeftArrow && movement != Direction.Right)
                {
                    movement = Direction.Left;
                }
                else if (key == ConsoleKey.RightArrow && movement != Direction.Left)
                {
                    movement = Direction.Right;
                }
            }

            return movement;
        }

        // Funkce pro vykreslení pixelu
        static void DrawPixel(Pixel pixel)
        {
            SetCursorPosition(pixel.XPos, pixel.YPos);
            ForegroundColor = pixel.ScreenColor;
            Write("■");
            SetCursorPosition(0, 0);
        }

        // Funkce pro vykreslení hranic
        static void DrawBorder()
        {
            for (int i = 0; i < WindowWidth; i++)
            {
                SetCursorPosition(i, 0);
                Write("■");

                SetCursorPosition(i, WindowHeight - 1);
                Write("■");
            }

            for (int i = 0; i < WindowHeight; i++)
            {
                SetCursorPosition(0, i);
                Write("■");

                SetCursorPosition(WindowWidth - 1, i);
                Write("■");
            }
        }

        // Funkce pro pohyb hada
        static void MoveHead(ref Pixel head, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    head.YPos--;
                    break;
                case Direction.Down:
                    head.YPos++;
                    break;
                case Direction.Left:
                    head.XPos--;
                    break;
                case Direction.Right:
                    head.XPos++;
                    break;
            }
        }

        // Struktura pro pixel
        struct Pixel
        {
            public Pixel(int xPos, int yPos, ConsoleColor color)
            {
                XPos = xPos;
                YPos = yPos;
                ScreenColor = color;
            }
            public int XPos { get; set; }
            public int YPos { get; set; }
            public ConsoleColor ScreenColor { get; set; }
        }

        // Enum pro směry
        enum Direction
        {
            Up,
            Down,
            Right,
            Left
        }
    }
}
