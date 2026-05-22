using System;
using System.Collections.Generic;

class SaddlePointFinder
{
    static void Main()
    {
        Console.Write("Введите количество строк (m): ");
        int m = int.Parse(Console.ReadLine());
        Console.Write("Введите количество столбцов (n): ");
        int n = int.Parse(Console.ReadLine());

        int[,] matrix = new int[m, n];

        Console.WriteLine("Выберите способ заполнения:");
        Console.WriteLine("1. Случайные числа");
        Console.WriteLine("2. Ввод вручную");
        Console.Write("Ваш выбор (1 или 2): ");
        int choice = int.Parse(Console.ReadLine());

        if (choice == 1)
        {
            Console.Write("Введите нижнюю границу диапазона: ");
            int minVal = int.Parse(Console.ReadLine());
            Console.Write("Введите верхнюю границу диапазона: ");
            int maxVal = int.Parse(Console.ReadLine());

            Random rand = new Random();
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    matrix[i, j] = rand.Next(minVal, maxVal + 1);

            Console.WriteLine("\nСгенерированная матрица:");
            PrintMatrix(matrix);
        }
        else if (choice == 2)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write($"Введите элемент [{i},{j}]: ");
                    matrix[i, j] = int.Parse(Console.ReadLine());
                }
            }
        }

        // --- Поиск седловых точек ---

        // 1. Находим минимумы в строках
        List<(int row, int col)> rowMins = new List<(int, int)>();
        for (int i = 0; i < m; i++)
        {
            int minVal = matrix[i, 0];
            int minCol = 0;
            for (int j = 1; j < n; j++)
            {
                if (matrix[i, j] < minVal)
                {
                    minVal = matrix[i, j];
                    minCol = j;
                }
            }
            rowMins.Add((i, minCol));
        }

        // 2. Находим максимумы в столбцах
        List<(int row, int col)> colMaxs = new List<(int, int)>();
        for (int j = 0; j < n; j++)
        {
            int maxVal = matrix[0, j];
            int maxRow = 0;
            for (int i = 1; i < m; i++)
            {
                if (matrix[i, j] > maxVal)
                {
                    maxVal = matrix[i, j];
                    maxRow = i;
                }
            }
            colMaxs.Add((maxRow, j));
        }

        // 3. Ищем пересечения
        var saddlePoints = new List<(int row, int col, int value)>();

        foreach (var min in rowMins)
        {
            foreach (var max in colMaxs)
            {
                if (min.row == max.row && min.col == max.col)
                {
                    saddlePoints.Add((min.row, min.col, matrix[min.row, min.col]));
                }
            }
        }

        // --- Вывод результата ---

        if (saddlePoints.Count > 0)
        {
            Console.WriteLine("\nНайдены седловые точки:");
            foreach (var point in saddlePoints)
            {
                Console.WriteLine($"Элемент {point.value} в позиции [{point.row},{point.col}]");
            }
        }
        else
        {
            Console.WriteLine("\nСедловых точек не найдено.");
        }
    }

    static void PrintMatrix(int[,] matrix)
    {
        int m = matrix.GetLength(0);
        int n = matrix.GetLength(1);

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
                Console.Write($"{matrix[i, j],5}");

            Console.WriteLine();
        }
    }
}