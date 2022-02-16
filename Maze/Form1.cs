using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maze
{
    public partial class Form1 : Form
    {
        Random rnd = new Random();
        // Размеры поля и размер клетки в пикселях
        public static int width = 10, height = 10, k = 30;
        public int[,] Map;// массив для хранения карты
        public static int[] dx = new int[] { -1, 0, 1, 0 };//массивы для смещений при ходьбе 
        public static int[] dy = new int[] { 0, 1, 0, -1 };

        //для связи графического изображения с полотном, которое есть на форме
        public Bitmap bitfield = new Bitmap(k * (width + 2), k * (height + 2));//+2 для барьерных элементов
        public Graphics gr; // Для рисования поля на PictureBox

        //метод, который загружает карту из файла
        public void LoadMap()
        {
            System.IO.StreamReader sr =
                new System.IO.StreamReader("input.txt",
                    Encoding.Default);
            //считываем 2 числа в одной строке
            string[] s = sr.ReadLine().Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
            // количество строк и столбцов
            height = Convert.ToInt32(s[0]);
            width = Convert.ToInt32(s[1]);
            Map = new int[height + 2, width + 2];
            // считывание карты лабиринта
            for (int i = 1; i <= height; i++)
            {
                s = sr.ReadLine().Split(new[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);
                for (int j = 1; j <= width; j++)
                    Map[i, j] = Convert.ToInt32(s[j - 1]);
            }
            // стены вокруг лабиринта
            for (int i = 0; i <= height + 1; i++)
                Map[i, 0] = Map[i, width + 1] = 1;
            for (int j = 0; j <= width + 1; j++)
                Map[0, j] = Map[height + 1, j] = 1;

        }


        //метод, который рисует форму
        public void ShowMap()
        {
            gr.Clear(Color.Black); //Очистим поле
            for (int i = 0; i <= width + 1; i++)
                for (int j = 0; j <= height + 1; j++)
                    switch (Map[i, j])
                    { // Если клетка поля существует 
                        case 1:
                            gr.FillRectangle(Brushes.Gray, i * k, j * k, k, k); // Рисуем в этом месте квадратик
                            gr.DrawRectangle(Pens.Black, i * k, j * k, k, k);
                            break;
                        case -1:
                            gr.FillRectangle(Brushes.White, i * k, j * k, k, k); // Рисуем в этом месте квадратик
                            gr.DrawRectangle(Pens.Black, i * k, j * k, k, k);
                            break;
                        case -2:
                            gr.FillRectangle(Brushes.CadetBlue, i * k, j * k, k, k); // Рисуем в этом месте квадратик
                            gr.DrawRectangle(Pens.Black, i * k, j * k, k, k);
                            break;
                        case -3:
                            gr.FillRectangle(Brushes.Yellow, i * k, j * k, k, k); // Рисуем в этом месте квадратик
                            gr.DrawRectangle(Pens.Black, i * k, j * k, k, k);
                            break;
                    }

            pictureBox1.Image = bitfield;
        }



        public void GeneratePlaces()
        {
            int x1 = 0, y1 = 0;
            do
            {
                x1 = 1 + rnd.Next(height);//начальные координаты клетки
                y1 = 1 + rnd.Next(width);
            } while (Map[x1, y1] != 0);//ищем пока клетка занята, как только клетка свободна - она является стартовой
            Map[x1, y1] = 2;
            int x2 = 0, y2 = 0;
            do
            {
                x2 = 1 + rnd.Next(height);
                y2 = 1 + rnd.Next(width);
            } while (Map[x2, y2] != 0);
            //Map[x2, y2] = 3;

            int x, y;
            Queue q = new Queue();
            q.Enqueue(x1); q.Enqueue(y1);
            // пока очередь не пуста
            while (q.Count > 0)
            {
                // извлекаем координаты клетки из очереди
                x = Convert.ToInt32(q.Dequeue());
                y = Convert.ToInt32(q.Dequeue());
                // для всех соседей
                for (int i = 0; i < 4; i++)
                    // если по ходу соседа с номером i
                    // клетка равна 0 (не стена и не пометка)
                    if (Map[x + dx[i], y + dy[i]] == 0)
                    {
                        // то мы пришли сюда на ход позже
                        // пометим количеством ходов
                        Map[x + dx[i], y + dy[i]] = Map[x, y] + 1;
                        // и поместим в очередь для обработки
                        q.Enqueue(x + dx[i]);
                        q.Enqueue(y + dy[i]);
                    }
            }


            x = x2; y = y2;
            while (x != x1 || y != y1)
            {
                int z = 0;
                for (int i = 0; i < 4; i++)
                    if (Map[x + dx[i], y + dy[i]] == Map[x, y] - 1)
                    {
                        z = i;
                        break;
                    }
                Map[x, y] = -1;
                x += dx[z];
                y += dy[z];
            }

            Map[x1, y1] = -2;
            Map[x2, y2] = -3;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // задать картинку для полотна
            gr = Graphics.FromImage(bitfield);
            LoadMap();
            Width = (width + 2) * (k + 1) + 4;
            Height = (height + 2) * (k + 3) + 4;
            GeneratePlaces();
            ShowMap();
        }
    }
}
