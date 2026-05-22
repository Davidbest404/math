using System;

class MaxSubarrayFinder
{
    // Структура для удобного возврата нескольких значений
    public struct SubarrayResult
    {
        public int MaxSum;
        public int StartIndex;
        public int EndIndex;
    }

    // Основная функция алгоритма Кадане
    static SubarrayResult FindMaxSubarray(int[] nums)
    {
        if (nums == null || nums.Length == 0)
            throw new ArgumentException("Массив не должен быть пустым.");

        // Инициализация текущими и глобальными значениями
        int current_sum = nums[0];
        int max_sum = nums[0];

        // Переменные для отслеживания границ подмассива
        int start_temp = 0; // Временная метка начала текущего подмассива
        int start_index = 0; // Финальная метка начала лучшего подмассива
        int end_index = 0;   // Финальная метка конца лучшего подмассива

        // Начинаем цикл со второго элемента (индекс 1)
        for (int i = 1; i < nums.Length; i++)
        {
            // Если current_sum + nums[i] меньше, чем nums[i],
            // значит, выгоднее начать новый подмассив с текущего элемента.
            if (nums[i] > current_sum + nums[i])
            {
                current_sum = nums[i];
                start_temp = i; // Обновляем временную метку начала
            }
            else
            {
                current_sum += nums[i]; // Расширяем текущий подмассив
            }

            // Если нашли новую глобальную максимальную сумму,
            // обновляем результат и фиксируем границы.
            if (current_sum > max_sum)
            {
                max_sum = current_sum;
                start_index = start_temp;
                end_index = i;
            }
        }

        return new SubarrayResult { MaxSum = max_sum, StartIndex = start_index, EndIndex = end_index };
    }

    static void Main()
    {
        Console.Write("Введите числа через пробел: ");
        string input = Console.ReadLine();

        // Парсинг введенной строки в массив чисел
        string[] parts = input.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
        int[] nums = new int[parts.Length];

        for (int i = 0; i < parts.Length; i++)
        {
            nums[i] = int.Parse(parts[i]);
        }

        try
        {
            SubarrayResult result = FindMaxSubarray(nums);

            // Формируем строку с элементами найденного подмассива для красивого вывода
            string subarrayStr = string.Join(", ", nums, result.StartIndex, result.EndIndex - result.StartIndex + 1);

            Console.WriteLine($"\nМаксимальная сумма: {result.MaxSum}");
            Console.WriteLine($"Непрерывный подмассив: [{subarrayStr}]");
            Console.WriteLine($"Индексы в массиве: от {result.StartIndex} до {result.EndIndex}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }
}