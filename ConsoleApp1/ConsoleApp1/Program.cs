using System;
using System.Collections.Generic;
using System.Threading;

class PathFinderConsole
{
    // Структура для хранения координат
    struct Point
    {
        public int X, Y;
        public Point(int x, int y) { X = x; Y = y; }
        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point a, Point b) => !(a == b);
        public override bool Equals(object obj) => obj is Point p && this == p;
        // Используем простой хэш-код для совместимости
        public override int GetHashCode() => X ^ Y;
    }

    // Возможные направления движения (8 соседей)
    static readonly Point[] Directions = {
        new Point(-1, -1), new Point(-1, 0), new Point(-1, 1),
        new Point( 0, -1),                  new Point( 0, 1),
        new Point( 1, -1), new Point( 1, 0), new Point( 1, 1)
    };

    static void Main()
    {
        Console.Write("Введите количество строк (m): ");
        int m = int.Parse(Console.ReadLine());
        Console.Write("Введите количество столбцов (n): ");
        int n = int.Parse(Console.ReadLine());

        // 0 - свободно, 1 - препятствие
        int[,] grid = new int[m, n];

        // --- Ввод точек и препятствий ---
        var start = ReadPoint("стартовой точки A", m, n);
        var end = ReadPoint("конечной точки B", m, n);

        if (start == end)
        {
            Console.WriteLine("Старт и финиш совпадают.");
            return;
        }

        List<Point> obstacles = AddObstacles(grid, m, n, start, end);

        // --- Поиск пути ---
        var result = FindPathBFS(grid, start, end);

        if (result.path != null)
        {
            Console.WriteLine($"\nПуть найден! Длина: {result.path.Count} шагов.");
            PrintFinalGrid(grid, result.path, start, end);
        }
        else
        {
            Console.WriteLine("\nПуть не найден. Точка B недоступна.");
            PrintFinalGrid(grid, null, start, end);
        }
    }

    static (List<Point> path, List<Point> visited) FindPathBFS(int[,] grid, Point start, Point end)
    {
        int m = grid.GetLength(0);
        int n = grid.GetLength(1);

        var queue = new Queue<Point>();
        var visited = new bool[m, n];

        // Для восстановления пути и хранения порядка посещения
        var parentMap = new Dictionary<Point, Point>();
        var orderVisited = new List<Point>();

        queue.Enqueue(start);
        visited[start.X, start.Y] = true;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            orderVisited.Add(current); // Запоминаем порядок посещения

            // Отрисовка текущего шага анимации
            PrintGrid(grid, orderVisited, null, start, end);
            Thread.Sleep(80); // Задержка для анимации

            if (current == end)
            {
                // Путь найден. Восстанавливаем его.
                var path = ReconstructPath(parentMap, start, end);
                return (path, orderVisited);
            }

            foreach (var dir in Directions)
            {
                var next = new Point(current.X + dir.X, current.Y + dir.Y);

                if (next.X >= 0 && next.X < m && next.Y >= 0 && next.Y < n)
                {
                    if (grid[next.X, next.Y] == 0 && !visited[next.X, next.Y])
                    {
                        queue.Enqueue(next);
                        visited[next.X, next.Y] = true;
                        parentMap[next] = current;
                    }
                }
            }
        }

        return (null, orderVisited); // Путь не найден
    }

    static List<Point> ReconstructPath(Dictionary<Point, Point> parentMap, Point start, Point end)
    {
        var path = new List<Point>();
        var current = end;

        while (current != start)
        {
            path.Add(current);
            current = parentMap[current];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }

    #region Функции ввода и вывода

    static void PrintGrid(int[,] grid, List<Point> visitedPoints, List<Point> pathPoints, Point start, Point end)
    {
        Console.Clear(); // Очищаем консоль для нового кадра

        int m = grid.GetLength(0);
        int n = grid.GetLength(1);

        var visitedSet = new HashSet<Point>(visitedPoints ?? new List<Point>());
        var pathSet = new HashSet<Point>(pathPoints ?? new List<Point>());

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var p = new Point(i, j);
                if (p == start) Console.Write(" S ");
                else if (p == end) Console.Write(" E ");
                else if (pathSet.Contains(p)) Console.Write(" * "); // Точка (•)
                else if (visitedSet.Contains(p)) Console.Write(" + "); // Пустой квадрат □
                else if (grid[i, j] == 1) Console.Write(" \u2588\u2588"); // Закрашенный квадрат ██ (для ширины)
                else Console.Write(" . ");
            }
            Console.WriteLine();
        }
    }

    static void PrintFinalGrid(int[,] grid, List<Point> pathPoints, Point start, Point end)
    {
        Console.Clear();
        Console.WriteLine("Итоговая карта:");

        int m = grid.GetLength(0);
        int n = grid.GetLength(1);

        var pathSet = new HashSet<Point>(pathPoints ?? new List<Point>());

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                var p = new Point(i, j);
                if (p == start) Console.Write(" S ");
                else if (p == end) Console.Write(" E ");
                else if (pathSet.Contains(p)) Console.Write(" * "); // Точка (•)
                else if (grid[i, j] == 1) Console.Write(" \u2588\u2588"); // Закрашенный квадрат ██
                else Console.Write(" . ");
            }
            Console.WriteLine();
        }
    }

    static Point ReadPoint(string name, int maxX, int maxY)
    {
        while (true)
        {
            Console.Write($"Введите координаты {name} (строка столбец): ");
            string[] parts = Console.ReadLine().Split();
            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
            {
                x--; y--; // Переводим к индексам с 0
                if (x >= 0 && x < maxX && y >= 0 && y < maxY)
                    return new Point(x, y);
            }
            Console.WriteLine($"Некорректный ввод. Введите числа в диапазоне от 1 до {maxX} и от 1 до {maxY}.");
        }
    }

    static List<Point> AddObstacles(int[,] grid, int m, int n, Point start, Point end)
    {
        List<Point> obstacles = new List<Point>();
        while (true)
        {
            Console.Write("Введите координаты препятствия через пробел (или 'done' для завершения): ");
            string input = Console.ReadLine();
            if (input.ToLower() == "done") break;

            string[] parts = input.Split();
            if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
            {
                x--; y--;
                var p = new Point(x, y);
                if (p.X >= 0 && p.X < m && p.Y >= 0 && p.Y < n)
                {
                    if (p != start && p != end)
                    {
                        grid[p.X, p.Y] = 1;
                        obstacles.Add(p);
                        Console.WriteLine($"Препятствие добавлено в ({x + 1}, {y + 1}).");
                    }
                    else
                    {
                        Console.WriteLine("Нельзя поставить препятствие на старт или финиш.");
                    }
                }
                else
                {
                    Console.WriteLine("Координаты вне диапазона.");
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод.");
            }
        }
        return obstacles;
    }

    #endregion
}