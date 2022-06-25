﻿using System;
using System.Windows.Forms;

namespace Project1
{
    public partial class Kod_dostovernosti : Form
    {
        public Kod_dostovernosti()
        {
            InitializeComponent();
        }

        private void Kod_dostovernosti_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = "\n" + "    0x1 - Недостоверность: дребезг ТС\n" +
                "    0x2 - Источник: ручной ввод\n" +
                "    0x42 - Источник: ручной ввод из внешней системы\n" +
                "    0x4 - Недостоверность: недоверие телемеханике\n" +
                "    0x8 - Недостоверность: ПНУ\n" +
                "    0x10 - Источник: расчёт\n" +
                "    0x20 - Недостоверность: по параметрам функции\n" +
                "    0x40 - Источник: внешняя система\n" +
                "    0x80 - Недостоверность: сбой телеметрии\n" +
                "    0x100 - Источник: телеметрия\n" +
                "    0x200 - Недостоверность: необновление\n" +
                "    0x202 - Источник: ручной ввод неблокирующий\n" +
                "    0x400 - Недостоверность: сбой расчёта\n" +
                "    0x800 - Недостоверность: по дублю\n" +
                "    0x1000 - Недостоверность: нарушение физических границ\n" +
                "    0x2000 - Недостоверность: по оценке состояния\n" +
                "    0x4000 - Инфо: маловажное значение\n" +
                "    0x8000 - Нет данных\n" +
                "    0x8200 - Время запроса выходит за границы архива\n" +
                "    0x10000 - Нарушение: нижний аварийный\n" +
                "    0x20000 - Нарушение: верхний предупредительный\n" +
                "    0x40000 - Нарушение: нижний предупредительный\n" +
                "    0x80000 - Источник: замена отчётной информацией\n" +
                "    0x100000 - Источник: дубль\n" +
                "    0x100040 - Источник: дубль во внешней системе\n" +
                "    0x200000 - Нарушение: верхний аварийный\n" +
                "    0x800000 - Недостоверность: скачок\n" +
                "    0x1000000 - Источник: принудительная замена\n" +
                "    0x4000000 - Источник: технологическая задача\n" +
                "    0x8000000 - Недостоверность: подозрение на скачок\n" +
                "    0x10000000 - Источник: АСКУЭ\n" +
                "    0x20000000 - Источник: обнуление\n" +
                "    0x40000000 - Источник: повтор предыдущего значения\n";
        }
    }
}
