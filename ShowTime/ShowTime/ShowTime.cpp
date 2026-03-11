#include <iostream>
#include <string>
#include <vector>
#include <climits>
#include <cctype>
#include <algorithm>

using namespace std;

// Структура для хранения минимального и максимального значения для отрезка
struct ValueRange {
    long long minVal;
    long long maxVal;

    ValueRange() : minVal(LLONG_MAX), maxVal(LLONG_MIN) {}
    ValueRange(long long minV, long long maxV) : minVal(minV), maxVal(maxV) {}
};

// Функция применения операции к двум числам
long long applyOp(long long a, long long b, char op) {
    switch (op) {
    case '+': return a + b;
    case '-': return a - b;
    case '*': return a * b;
    default:
        cerr << "Неизвестная операция: " << op << endl;
        return 0;
    }
}

// Основная функция решения
ValueRange solveExpression(const vector<long long>& digits, const vector<char>& ops) {
    int n = digits.size();  // количество цифр

    // Создаем двумерные массивы для минимумов и максимумов
    vector<vector<long long>> minVal(n, vector<long long>(n, LLONG_MAX));
    vector<vector<long long>> maxVal(n, vector<long long>(n, LLONG_MIN));

    // Базовый случай: отрезки из одной цифры
    for (int i = 0; i < n; i++) {
        minVal[i][i] = digits[i];
        maxVal[i][i] = digits[i];
    }

    // Перебираем длину подвыражения (количество цифр минус 1)
    for (int length = 1; length < n; length++) {
        // Перебираем левую границу отрезка
        for (int left = 0; left < n - length; left++) {
            int right = left + length;  // правая граница отрезка

            // Перебираем место последней операции (между left и right)
            for (int mid = left; mid < right; mid++) {
                // Получаем операцию между mid и mid+1
                char op = ops[mid];

                // Рассматриваем все 4 комбинации минимумов и максимумов
                vector<long long> candidates = {
                    applyOp(minVal[left][mid], minVal[mid + 1][right], op),
                    applyOp(minVal[left][mid], maxVal[mid + 1][right], op),
                    applyOp(maxVal[left][mid], minVal[mid + 1][right], op),
                    applyOp(maxVal[left][mid], maxVal[mid + 1][right], op)
                };

                // Обновляем минимум и максимум для отрезка [left][right]
                for (long long val : candidates) {
                    if (val < minVal[left][right]) {
                        minVal[left][right] = val;
                    }
                    if (val > maxVal[left][right]) {
                        maxVal[left][right] = val;
                    }
                }
            }
        }
    }

    // Возвращаем результат для всего выражения
    return ValueRange(minVal[0][n - 1], maxVal[0][n - 1]);
}

// Функция разбора входной строки
bool parseExpression(const string& expr, vector<long long>& digits, vector<char>& ops) {
    digits.clear();
    ops.clear();

    bool expectDigit = true;  // ожидаем цифру
    bool lastWasDigit = false;

    for (char c : expr) {
        if (isspace(c)) continue;  // пропускаем пробелы

        if (isdigit(c)) {
            if (!expectDigit) {
                cerr << "Ошибка: неожиданная цифра после операции" << endl;
                return false;
            }
            digits.push_back(c - '0');
            expectDigit = false;
            lastWasDigit = true;
        }
        else if (c == '+' || c == '-' || c == '*') {
            if (expectDigit) {
                cerr << "Ошибка: выражение не может начинаться с операции" << endl;
                return false;
            }
            ops.push_back(c);
            expectDigit = true;
            lastWasDigit = false;
        }
        else {
            cerr << "Ошибка: недопустимый символ '" << c << "'" << endl;
            return false;
        }
    }

    if (expectDigit) {
        cerr << "Ошибка: выражение не может заканчиваться операцией" << endl;
        return false;
    }

    return true;
}

// Функция для демонстрации работы алгоритма на примере
void demonstrateExample(const string& expr) {
    cout << "=========================================" << endl;
    cout << "Выражение: " << expr << endl;
    cout << "=========================================" << endl;

    vector<long long> digits;
    vector<char> ops;

    if (!parseExpression(expr, digits, ops)) {
        cout << "Некорректное выражение!" << endl;
        return;
    }

    cout << "Цифры: ";
    for (long long d : digits) cout << d << " ";
    cout << endl;

    cout << "Операции: ";
    for (char op : ops) cout << op << " ";
    cout << endl;

    cout << "\nВычисляем максимальное значение..." << endl;

    ValueRange result = solveExpression(digits, ops);

    cout << "\nРЕЗУЛЬТАТЫ:" << endl;
    cout << "  Минимальное возможное значение: " << result.minVal << endl;
    cout << "  Максимальное возможное значение: " << result.maxVal << endl;
    cout << endl;

    // Выводим информацию о сложности
    cout << "Сложность алгоритма: O(n³), где n = " << digits.size() << " цифр" << endl;
    cout << "Количество обработанных отрезков: " << (digits.size() * (digits.size() + 1) / 2) << endl;
    cout << "=========================================\n" << endl;
}

// Функция для тестирования с проверкой вручную
void runTests() {
    cout << "\n========== ЗАПУСК ТЕСТОВ ==========" << endl;

    // Тест 1: Простое выражение
    {
        cout << "ТЕСТ 1: Простое сложение" << endl;
        vector<long long> digits = { 3, 5 };
        vector<char> ops = { '+' };
        ValueRange result = solveExpression(digits, ops);
        cout << "3+5 = " << result.maxVal << " (ожидается 8)" << endl;
        cout << (result.maxVal == 8 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН") << "\n" << endl;
    }

    // Тест 2: Умножение
    {
        cout << "ТЕСТ 2: Умножение" << endl;
        vector<long long> digits = { 4, 7 };
        vector<char> ops = { '*' };
        ValueRange result = solveExpression(digits, ops);
        cout << "4*7 = " << result.maxVal << " (ожидается 28)" << endl;
        cout << (result.maxVal == 28 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН") << "\n" << endl;
    }

    // Тест 3: Три числа с разными операциями
    {
        cout << "ТЕСТ 3: Три числа" << endl;
        vector<long long> digits = { 2, 3, 4 };
        vector<char> ops = { '+', '*' };
        ValueRange result = solveExpression(digits, ops);
        cout << "2+3*4" << endl;
        cout << "Варианты: (2+3)*4=20, 2+(3*4)=14" << endl;
        cout << "Максимум = " << result.maxVal << " (ожидается 20)" << endl;
        cout << (result.maxVal == 20 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН") << "\n" << endl;
    }

    // Тест 4: С отрицательными числами (проверка важности минимумов)
    {
        cout << "ТЕСТ 4: Важность минимумов" << endl;
        vector<long long> digits = { 1, -2, -3 };
        vector<char> ops = { '*', '*' };
        ValueRange result = solveExpression(digits, ops);
        cout << "1 * (-2) * (-3)" << endl;
        cout << "Варианты: (1*(-2))*(-3)=6, 1*((-2)*(-3))=6" << endl;
        cout << "Максимум = " << result.maxVal << " (ожидается 6)" << endl;
        cout << "Минимум = " << result.minVal << " (ожидается 6)" << endl;
        cout << (result.maxVal == 6 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН") << "\n" << endl;
    }

    // Тест 5: Классический пример из лекции
    {
        cout << "ТЕСТ 5: Классический пример" << endl;
        vector<long long> digits = { 5, 8, 7, 4, 8, 9 };
        vector<char> ops = { '-', '+', '*', '-', '+' };
        // Выражение: 5 - 8 + 7 * 4 - 8 + 9
        ValueRange result = solveExpression(digits, ops);
        cout << "5 - 8 + 7 * 4 - 8 + 9" << endl;
        cout << "Максимум = " << result.maxVal << " (ожидается 200)" << endl;
        cout << (result.maxVal == 200 ? "✓ ПРОЙДЕН" : "✗ НЕ ПРОЙДЕН") << "\n" << endl;
    }

    cout << "========== ТЕСТЫ ЗАВЕРШЕНЫ ==========\n" << endl;
}

// Интерактивный режим
void interactiveMode() {
    cout << "\n========== ИНТЕРАКТИВНЫЙ РЕЖИМ ==========" << endl;
    cout << "Введите арифметическое выражение (цифры и операции +, -, *):" << endl;
    cout << "Пример: 5-8+7*4-8+9" << endl;
    cout << "Или введите 'exit' для выхода" << endl;

    string input;
    while (true) {
        cout << "\n> ";
        getline(cin, input);

        if (input == "exit" || input == "quit") {
            break;
        }

        if (input.empty()) continue;

        demonstrateExample(input);
    }
}

// Функция для генерации и решения случайного выражения
void randomExample() {
    cout << "\n========== СЛУЧАЙНЫЙ ПРИМЕР ==========" << endl;

    int numDigits = rand() % 7 + 3;  // от 3 до 9 цифр
    vector<long long> digits;
    vector<char> ops;

    char opChars[] = { '+', '-', '*' };

    for (int i = 0; i < numDigits; i++) {
        digits.push_back(rand() % 9 + 1);  // цифры от 1 до 9
        if (i < numDigits - 1) {
            ops.push_back(opChars[rand() % 3]);
        }
    }

    cout << "Сгенерировано выражение: ";
    for (int i = 0; i < numDigits; i++) {
        cout << digits[i];
        if (i < numDigits - 1) {
            cout << ops[i];
        }
    }
    cout << endl;

    ValueRange result = solveExpression(digits, ops);

    cout << "Минимальное значение: " << result.minVal << endl;
    cout << "Максимальное значение: " << result.maxVal << endl;
    cout << "=========================================\n" << endl;
}

int main() {
    setlocale(LC_ALL, "Russian");

    cout << "=========================================" << endl;
    cout << "   АЛГОРИТМ МАКСИМИЗАЦИИ ВЫРАЖЕНИЯ" << endl;
    cout << "   (расстановка скобок) " << endl;
    cout << "=========================================" << endl;

    // Меню выбора режима
    cout << "\nВыберите режим работы:" << endl;
    cout << "1 - Демонстрация на примерах из лекции" << endl;
    cout << "2 - Запуск тестов" << endl;
    cout << "3 - Интерактивный режим (свой пример)" << endl;
    cout << "4 - Случайный пример" << endl;
    cout << "0 - Выход" << endl;

    int choice;
    cout << "\nВаш выбор: ";
    cin >> choice;
    cin.ignore();  // очищаем буфер после cin

    switch (choice) {
    case 1:
        demonstrateExample("5-8+7*4-8+9");
        demonstrateExample("2+3*4");
        demonstrateExample("1*2*3*4");
        break;
    case 2:
        runTests();
        break;
    case 3:
        interactiveMode();
        break;
    case 4:
        srand(time(0));
        randomExample();
        break;
    case 0:
        cout << "До свидания!" << endl;
        break;
    default:
        cout << "Неверный выбор!" << endl;
    }

    return 0;
}