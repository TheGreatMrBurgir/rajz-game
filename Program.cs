using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static char drawCharacter = '█';
    static ConsoleColor drawColor = ConsoleColor.White;
    static int x = Console.WindowWidth / 2;
    static int y = Console.WindowHeight / 2;
    static bool exit = false;
    static bool inDrawingMode = false;
    static string[] menuOptions = { "Új rajz", "Szerkesztés", "Törlés", "Kilépés" };
    static int selectedIndex = 0;

    static List<(int x, int y, char karakter, ConsoleColor szin)> rajz = new List<(int, int, char, ConsoleColor)>();
    static string saveDirectory = "rajzok";
    /*
     *static void Main(string[] args)
 {
     int x = Console.WindowWidth / 2;
     int v = 0;
     bool w = false;
     Console.Clear();
     DrawBorder();

     do
     {

         DrawMenu(x, ref v);

         switch (Console.ReadKey(true).Key)
         {
             case ConsoleKey.UpArrow:
                 v = v > 0 ? v - 1 : 3; 
                 break;
             case ConsoleKey.DownArrow:
                 v = v < 3 ? v + 1 : 0;
                 break;
             case ConsoleKey.Enter:
                 if (v == 3) w = true;
                 break;
             case ConsoleKey.Escape:
                 w = true;
                 break;
         }

     } while (!w);
 }
     */
    static void Main()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        ShowMainMenu();
        while (!exit)
        {
            if (inDrawingMode)
            {
                DrawingMode();
            }
            else
            {
                ShowMainMenu();
            }
        }
    }

    static void ShowMainMenu()
    {

        DrawMenuBorder();
        ShowMenu(menuOptions, selectedIndex);

        while (!inDrawingMode && !exit)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0) selectedIndex--;
            else if (key.Key == ConsoleKey.DownArrow && selectedIndex < menuOptions.Length - 1) selectedIndex++;
            else if (key.Key == ConsoleKey.Enter)
            {
                switch (selectedIndex)
                {
                    case 0:
                        NewDrawing();
                        break;
                    case 1:
                        EditDrawing();
                        break;
                    case 2:
                        DeleteDrawing();
                        break;
                    case 3:
                        exit = true;
                        break;
                }
            }

            Console.Clear();
            DrawMenuBorder();
            ShowMenu(menuOptions, selectedIndex);
        }
    }

    static void NewDrawing()
    {
        rajz.Clear();
        x = Console.WindowWidth / 2;
        y = Console.WindowHeight / 2;
        inDrawingMode = true;
        Console.Clear();
    }

    static void EditDrawing()
    {
        var files = Directory.GetFiles(saveDirectory);
        if (files.Length == 0)
        {
            Console.Clear();
            Console.WriteLine("Nincs elérhető rajz a szerkesztéshez.");
            Console.ReadKey();
            return;
        }

        var selectedFile = SelectFile(files);
        LoadDrawing(selectedFile);
        inDrawingMode = true;
        Console.Clear();
    }

    static void DeleteDrawing()
    {
        var files = Directory.GetFiles(saveDirectory);
        if (files.Length == 0)
        {
            Console.Clear();
            Console.WriteLine("Nincs elérhető rajz a törléshez.");
            Console.ReadKey();
            return;
        }

        var selectedFile = SelectFile(files);
        File.Delete(selectedFile);
        Console.Clear();
        Console.WriteLine("A fájl törölve lett.");
        Console.ReadKey();
    }

    static string SelectFile(string[] files)
    {
        int fileIndex = 0;
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Válassz egy rajzot:");
            for (int i = 0; i < files.Length; i++)
            {
                if (i == fileIndex)
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                else
                    Console.ResetColor();

                Console.WriteLine(Path.GetFileName(files[i]));
            }

            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow && fileIndex > 0) fileIndex--;
            else if (key.Key == ConsoleKey.DownArrow && fileIndex < files.Length - 1) fileIndex++;
            else if (key.Key == ConsoleKey.Enter)
                return files[fileIndex];
        }
    }

    static void LoadDrawing(string filePath)
    {
        rajz.Clear();
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            int x = int.Parse(parts[0]);
            int y = int.Parse(parts[1]);
            char karakter = parts[2][0];
            ConsoleColor szin = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), parts[3]);
            rajz.Add((x, y, karakter, szin));
        }

        RedrawSavedDrawing();
    }

    static void SaveDrawing(string fileName)
    {
        List<string> lines = new List<string>();
        foreach (var item in rajz)
        {
            lines.Add($"{item.x},{item.y},{item.karakter},{item.szin}");
        }

        File.WriteAllLines(Path.Combine(saveDirectory, fileName), lines);
    }

    static void DrawingMode()
    {
        bool drawing = true;
        Console.Clear();
        while (drawing)
        {
            DrawBorder();
            RedrawSavedDrawing();
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Spacebar)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = drawColor;
                Console.Write(drawCharacter);
                Console.ResetColor();
                rajz.Add((x, y, drawCharacter, drawColor));
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.Write("Add meg a mentés nevét: ");
                string fileName = Console.ReadLine();
                SaveDrawing(fileName + ".txt");
                drawing = false;
                inDrawingMode = false;
                Console.Clear();
            }
            else if (key.Key == ConsoleKey.F1)
            {
                drawCharacter = '█'; 
            }
            else if (key.Key == ConsoleKey.F2)
            {
                drawCharacter = '▓'; 
            }
            else if (key.Key == ConsoleKey.F3)
            {
                drawCharacter = '▒'; 
            }
            else if (key.Key == ConsoleKey.F4)
            {
                drawCharacter = '░'; 
            }
            else if (key.Key >= ConsoleKey.D0 && key.Key <= ConsoleKey.D9)
            {
                drawColor = (ConsoleColor)(key.Key - ConsoleKey.D0); 
            }
            else
            {
                MoveCursor(ref x, ref y, key);
            }
        }
    }

    static void RedrawSavedDrawing()
    {
        foreach (var item in rajz)
        {
            Console.SetCursorPosition(item.x, item.y);
            Console.ForegroundColor = item.szin;
            Console.Write(item.karakter);
        }
        Console.ResetColor();
    }

    static void MoveCursor(ref int x, ref int y, ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow: if (x > 1) x--; break;
            case ConsoleKey.RightArrow: if (x < Console.WindowWidth - 2) x++; break;
            case ConsoleKey.UpArrow: if (y > 1) y--; break;
            case ConsoleKey.DownArrow: if (y < Console.WindowHeight - 2) y++; break;
        }
    }
    static void ShowMenu(string[] options, int selectedIndex)
    {
        int startY = Console.WindowHeight / 2 - options.Length / 2;

        for (int i = 0; i < options.Length; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth / 2 - options[i].Length / 2, startY + i);
            if (i == selectedIndex)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(options[i]);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(options[i]);
            }
        }
    }
    static void DrawMenuBorder()
    {
        int menuWidth = 24;
        int menuHeight = menuOptions.Length + 2;
        int startX = Console.WindowWidth / 2 - menuWidth / 2;
        int startY = Console.WindowHeight / 2 - menuHeight / 2;

        Console.SetCursorPosition(startX, startY);
        Console.Write("╔");
        for (int i = 0; i < menuWidth - 2; i++) Console.Write("═");
        Console.Write("╗");

        for (int i = 1; i < menuHeight - 1; i++)
        {
            Console.SetCursorPosition(startX, startY + i);
            Console.Write("║");
            Console.SetCursorPosition(startX + menuWidth - 1, startY + i);
            Console.Write("║");
        }

        Console.SetCursorPosition(startX, startY + menuHeight - 1);
        Console.Write("╚");
        for (int i = 0; i < menuWidth - 2; i++) Console.Write("═");
        Console.Write("╝");
    }

    static void DrawBorder()
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;

        Console.SetCursorPosition(0, 0);
        Console.Write("╔");
        for (int i = 1; i < width - 1; i++) Console.Write("═");
        Console.Write("╗");

        Console.SetCursorPosition(0, height - 1);
        Console.Write("╚");
        for (int i = 1; i < width - 1; i++) Console.Write("═");
        Console.Write("╝");

        for (int i = 1; i < height - 1; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("║");
            Console.SetCursorPosition(width - 1, i);
            Console.Write("║");
        }
    }
}
