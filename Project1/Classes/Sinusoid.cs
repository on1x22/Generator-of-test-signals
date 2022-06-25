using System.ComponentModel;

namespace Project1
{
    class Sinusoid : ICommand
    {
        [DisplayName("Команда")]
        [Description("Расчет синусоидального сигнала ведется по формуле: A+B*sin(2*pi*t/T+phase)")]
        public string Title
        {
            get
            {
                return "   " + name + ".Синусоида (A = " +
                    value_start + ", B = " + value_max + ", T = " + period + ", phase = " + middle + ")";
            }
        }
        
        public const string Com_name = "Sinusoid";
        [TypeConverter(typeof(EnumConverter))]
        [DisplayName("Тип")]
        public string type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Среднее значение")]
        public double value_start { get; set; }
        [DisplayName("Амплитуда")]
        public double value_max { get; set; }
        [DisplayName("Период")]
        public double period { get; set; }
        [DisplayName("Начальная фаза")]
        public double middle { get; set; } // Фаза

        public void Execute()
        {
        }

        public Sinusoid()
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
