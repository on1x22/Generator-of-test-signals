using System.ComponentModel;

namespace Project1
{
    class Triangle : ICommand
    {
        [DisplayName("Команда")]
        public string Title
        {
            get
            {
                return "   " + name + ".Треугольник (min = " +
                    value_min + ", max = " + value_max + ", period = " + period + ")";
            }
        }
        public const string Com_name = "Triangle";
        [TypeConverter(typeof(EnumConverter))]
        [DisplayName("Тип")]
        public string type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Минимальное значение")]
        public double value_min { get; set; }
        [DisplayName("Максимальное значение")]
        public double value_max { get; set; }
        [DisplayName("Период")]
        public double period { get; set; }
        [Browsable(false)]
        [DisplayName("Текущее время")]
        public double tek_time { get; set; }
        [Browsable(false)]
        [DisplayName("Шаговое значение")]
        public double middle
        {
            get
            {
                double a = (value_max - value_min) / ((period) / 2);
                return a;
            }
            set { }
        }
        [Browsable(false)]
        [DisplayName("Текущее значение")]
        public double tek_value { get; set; }

        public void Execute()
        {
        }

        public Triangle()
        {
            type = EnumConverter.ALT1;
        }

        class EnumConverter : TypeConverter
        {
            public const string ALT1 = "ТИ";
            public const string ALT2 = "ТС";

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new[] { ALT1 });
            }
        }

        string ICommand.Com_name
        {
            get
            {
                return Com_name;
            }
        }
    }
}
