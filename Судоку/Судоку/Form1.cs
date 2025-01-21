using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Судоку
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeSudokuGrid();
            this.tableLayoutPanel1.Paint += new PaintEventHandler(this.panel1_Paint);
        }
        private Random rand = new Random();
        public class SudokuGenerator
        {
            private int[,] board = new int[9, 9];
            private Random rand = new Random();

            // Генерация пустого поля Судоку
            public int[,] GenerateBoard()
            {
                // Заполняем поле по алгоритму
                FillDiagonal();
                FillRemaining(0, 3);

                // Удаляем элементы для создания задачи
                //RemoveNumbers(1); // Удалим 40 чисел для задачи

                return board;
            }

            // Заполнение диагональных блоков
            private void FillDiagonal()
            {
                for (int i = 0; i < 9; i += 3)
                {
                    FillBlock(i, i);
                }
            }

            // Заполнение подблока
            private bool FillBlock(int row, int col)
            {
                int[] numbers = new int[9];
                for (int i = 0; i < 9; i++) numbers[i] = i + 1;

                Shuffle(numbers);

                int index = 0;
                for (int i = row; i < row + 3; i++)
                {
                    for (int j = col; j < col + 3; j++)
                    {
                        board[i, j] = numbers[index++];
                    }
                }

                return true;
            }

            // Метод для случайного перемешивания массива
            private void Shuffle(int[] array)
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    int j = rand.Next(i + 1);
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }

            // Проверка, можно ли вставить число в клетку
            private bool IsSafe(int row, int col, int num)
            {
                return !UsedInRow(row, num) && !UsedInCol(col, num) && !UsedInBlock(row - row % 3, col - col % 3, num);
            }

            // Проверка на использование числа в строке
            private bool UsedInRow(int row, int num)
            {
                for (int i = 0; i < 9; i++)
                    if (board[row, i] == num) return true;
                return false;
            }

            // Проверка на использование числа в колонке
            private bool UsedInCol(int col, int num)
            {
                for (int i = 0; i < 9; i++)
                    if (board[i, col] == num) return true;
                return false;
            }

            // Проверка на использование числа в блоке 3x3
            private bool UsedInBlock(int startRow, int startCol, int num)
            {
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        if (board[i + startRow, j + startCol] == num) return true;
                return false;
            }

            // Заполнение оставшихся клеток
            private bool FillRemaining(int row, int col)
            {
                if (row == 9)
                {
                    return true; // Все строки заполнены
                }
                if (col == 9)
                {
                    return FillRemaining(row + 1, 0); // Переходим к следующей строке
                }

                if (board[row, col] != 0)
                {
                    return FillRemaining(row, col + 1); // Если клетка уже заполнена, переходим к следующей
                }

                for (int num = 1; num <= 9; num++)
                {
                    if (IsSafe(row, col, num))
                    {
                        board[row, col] = num;
                        if (FillRemaining(row, col + 1)) return true;
                        board[row, col] = 0; // Откат, если не получилось
                    }
                }
                return false;
            }

        }

        private int slozh = 2,del =40;

        // Удаление чисел для создания задачи
        private void RemoveNumbers(int count)
        {
            int removed = 0;
            while (removed < count)
            {
                int row = rand.Next(9);
                int col = rand.Next(9);

                if (solutionBoard[row, col] != 0)
                {
                    solutionBoard[row, col] = 0;
                    removed++;
                }
            }
        }

        // Массив для хранения TextBox
        private TextBox[,] sudokuTextBoxes = new TextBox[9, 9];

        private int[,] solutionBoard = new int[9, 9];
        private int[,] solutionBoard2 = new int[9, 9];

        // Метод для инициализации сетки из TextBox
        private void InitializeSudokuGrid()
        {
            // Заполняем TextBox в TableLayoutPanel
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    TextBox txtBox = new TextBox();
                    txtBox.Width = 30;
                    txtBox.Height = 30;
                    txtBox.TextAlign = HorizontalAlignment.Center;
                    txtBox.MaxLength = 1;  // Только одно число в каждой клетке
                    txtBox.Font = new System.Drawing.Font("Arial", 14);
                    txtBox.Name = $"txtBox_{i}_{j}";
                    tableLayoutPanel1.Controls.Add(txtBox, j, i); // Добавляем в таблицу
                    sudokuTextBoxes[i, j] = txtBox; // Сохраняем в массив
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 2);
            g.DrawLine(pen, 118, 0, 118, 360);
            g.DrawLine(pen, 238, 0, 238, 360);
            g.DrawLine(pen, 0, 118, 360, 118);
            g.DrawLine(pen, 0, 238, 360, 238);
        }

        // Метод для генерации и отображения Sudoku
        private void GenerateAndDisplaySudoku()
        {
            // Генерация поля Судоку
            SudokuGenerator generator = new SudokuGenerator();
            solutionBoard = generator.GenerateBoard(); // Генерация полного поля для решения

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    solutionBoard2[i, j] = solutionBoard[i, j];
                }
            }

            RemoveNumbers(del);
            // Отображаем значения в TextBox
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudokuTextBoxes[i, j].BackColor = System.Drawing.Color.White;
                    sudokuTextBoxes[i, j].Text = solutionBoard[i, j] == 0 ? "" : solutionBoard[i, j].ToString();
                    if (solutionBoard[i, j] == 0)
                    {
                        sudokuTextBoxes[i, j].Enabled = true;
                    }
                    else
                    {
                        sudokuTextBoxes[i, j].Enabled = false;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Генерируем и отображаем Судоку при загрузке формы
            GenerateAndDisplaySudoku();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateAndDisplaySudoku();
        }
        // Метод для проверки решения
        private void CheckSolution()
        {
            bool isCorrect = true;

            // Проверяем каждое поле
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    // Получаем введенное значение из TextBox
                    string input = sudokuTextBoxes[i, j].Text;

                    // Преобразуем введенное значение в число
                    int userValue;
                    if (int.TryParse(input, out userValue) && userValue >= 1 && userValue <= 9)
                    {
                        // Сравниваем введенное значение с правильным решением
                        if (userValue != solutionBoard2[i, j])
                        {
                            isCorrect = false;
                            sudokuTextBoxes[i, j].BackColor = System.Drawing.Color.Red; // Подсвечиваем неправильное поле
                        }
                        else
                        {
                            sudokuTextBoxes[i, j].BackColor = System.Drawing.Color.White; // Если правильно, сбрасываем цвет
                        }
                    }
                    else
                    {
                        // Если введено некорректное значение (не число или число вне диапазона)
                        isCorrect = false;
                        // Если поле пустое, пропускаем его
                        if (string.IsNullOrEmpty(input))
                        {
                            sudokuTextBoxes[i, j].BackColor = System.Drawing.Color.Yellow;
                        }
                        else
                        {
                            sudokuTextBoxes[i, j].BackColor = System.Drawing.Color.Red; // Подсвечиваем неправильное поле
                        }
                    }
                }
            }

            // Выводим сообщение в зависимости от результата
            if (isCorrect)
            {
                MessageBox.Show("Поздравляем! Решение правильное.", "Проверка решения", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Некоторые числа введены неверно. Проверьте их.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CheckSolution();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            slozh = 3;
            del = 60;
            GenerateAndDisplaySudoku();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            slozh = 2;
            del = 40;
            GenerateAndDisplaySudoku();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            slozh = 1;
            del = 20;
            GenerateAndDisplaySudoku();
        }
    }
}
