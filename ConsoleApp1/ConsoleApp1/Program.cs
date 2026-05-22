using System;
using System.Collections.Generic;
using System.Text;

// Вспомогательный класс для хранения результата и строки выражения
class ExpressionResult
{
    public int Value { get; set; }
    // Список всех возможных строк, дающих это значение (для режима "все варианты")
    public List<string> Expressions { get; set; }

    public ExpressionResult(int value, string expr)
    {
        Value = value;
        Expressions = new List<string> { expr };
    }
}

class ExpressionMaximizerDetailed
{
    static int ApplyOp(int a, char op, int b)
    {
        switch (op)
        {
            case '+': return a + b;
            case '-': return a - b;
            case '*': return a * b;
            default: throw new ArgumentException("Недопустимая операция");
        }
    }

    static void FindMaxValueDetailed(int[] nums, char[] ops, bool showAllSteps)
    {
        int n = nums.Length;

        // Таблицы теперь хранят объекты ExpressionResult
        ExpressionResult[,] M = new ExpressionResult[n, n]; // Максимумы
        ExpressionResult[,] m = new ExpressionResult[n, n]; // Минимумы

        // Инициализация: подвыражения из одного числа
        for (int i = 0; i < n; i++)
        {
            M[i, i] = new ExpressionResult(nums[i], nums[i].ToString());
            m[i, i] = new ExpressionResult(nums[i], nums[i].ToString());

            if (showAllSteps)
            {
                Console.WriteLine($"Инициализация M[{i},{i}] = {nums[i]}");
                Console.WriteLine($"Инициализация m[{i},{i}] = {nums[i]}");
            }
        }

        // l - длина цепочки (количество чисел в подвыражении)
        for (int l = 2; l <= n; l++)
        {
            for (int i = 0; i <= n - l; i++)
            {
                int j = i + l - 1;

                // Списки для сбора всех возможных значений и выражений на этом шаге
                var tempResults = new Dictionary<int, HashSet<string>>();

                for (int k = i; k < j; k++)
                {
                    // Перебираем все комбинации левого и правого подвыражений
                    var combos = new (ExpressionResult left, ExpressionResult right)[]
                    {
                        (M[i, k], M[k + 1, j]),
                        (M[i, k], m[k + 1, j]),
                        (m[i, k], M[k + 1, j]),
                        (m[i, k], m[k + 1, j])
                    };

                    foreach (var combo in combos)
                    {
                        int val = ApplyOp(combo.left.Value, ops[k], combo.right.Value);
                        string expr = $"({combo.left.Expressions[0]}{ops[k]}{combo.right.Expressions[0]})";

                        // Добавляем результат во временный словарь
                        if (!tempResults.ContainsKey(val))
                            tempResults[val] = new HashSet<string>();
                        tempResults[val].Add(expr);
                    }
                }

                // Находим максимум и минимум для текущего отрезка [i, j]
                int maxVal = int.MinValue;
                int minVal = int.MaxValue;

                foreach (var kvp in tempResults)
                {
                    if (kvp.Key > maxVal) maxVal = kvp.Key;
                    if (kvp.Key < minVal) minVal = kvp.Key;
                }

                // Формируем финальные объекты M[i,j] и m[i,j]
                M[i, j] = new ExpressionResult(maxVal, string.Join(" | ", tempResults[maxVal]));
                m[i, j] = new ExpressionResult(minVal, string.Join(" | ", tempResults[minVal]));

                if (showAllSteps)
                {
                    Console.WriteLine($"\n--- Итог для отрезка [{i},{j}] ---");
                    Console.WriteLine($"M[{i},{j}] (Макс: {maxVal}) -> {M[i, j].Expressions[0]}");
                    Console.WriteLine($"m[{i},{j}] (Мин: {minVal}) -> {m[i, j].Expressions[0]}");

                    if (tempResults[maxVal].Count > 1)
                        Console.WriteLine($"  Примечание: Есть {tempResults[maxVal].Count} вариантов для максимума.");
                }
            }
        }

        // Вывод финального результата
        Console.WriteLine("\n=== ФИНАЛЬНЫЙ РЕЗУЛЬТАТ ===");
        Console.WriteLine($"Максимальное значение: {M[0, n - 1].Value}");

        if (showAllSteps)
        {
            Console.WriteLine("Все варианты выражения для максимума:");
            foreach (var expr in M[0, n - 1].Expressions)
            {
                Console.WriteLine(expr);
            }
        }
        else
        {
            Console.WriteLine("Одно из решений:");
            Console.WriteLine(M[0, n - 1].Expressions[0]);
        }
    }

    static void Main()
    {
        Console.Write("Введите выражение (например: 3+5*6-8): ");
        string input = Console.ReadLine();

        // Парсим входную строку в массивы чисел и операций
        string[] tokens = System.Text.RegularExpressions.Regex.Split(input, @"([+\-*])");

        List<int> numsList = new List<int>();
        List<char> opsList = new List<char>();

        for (int i = 0; i < tokens.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(tokens[i]))
            {
                if (i % 2 == 0) // Числа на четных позициях
                    numsList.Add(int.Parse(tokens[i]));
                else // Операции на нечетных
                    opsList.Add(tokens[i][0]);
            }
        }

        int[] nums = numsList.ToArray();
        char[] ops = opsList.ToArray();

        if (nums.Length == 0 || ops.Length + 1 != nums.Length)
        {
            Console.WriteLine("Некорректный ввод.");
            return;
        }

        Console.WriteLine("Выберите режим вывода:");
        Console.WriteLine("1. Только ответ и одно решение");
        Console.WriteLine("2. Показать все шаги и варианты");

        bool showAllSteps = Console.ReadLine() == "2";

        FindMaxValueDetailed(nums, ops, showAllSteps);
    }
}