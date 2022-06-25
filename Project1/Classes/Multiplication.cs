using System;
using System.ComponentModel;

namespace Project1
{
    public class Multiplication : ICommand
    {
        string perv_chast = "";
        string vtor_chast = "";

        [DisplayName("Команда")]
        public string Title
        {
            get
            {
                try
                {
                    Convert.ToDouble(value_1);
                    perv_chast = value_1;
                }
                catch (FormatException)
                {
                    perv_chast = value_1 + ".Знач.";
                }
                try
                {
                    Convert.ToDouble(value_2);
                    vtor_chast = value_2;
                }
                catch (FormatException)
                {
                    vtor_chast = value_2 + ".Знач.";
                }
                return "   " + name + ".Знач.=" +
                    perv_chast + " * " + vtor_chast;
            }
        }

        public const string Com_name = "Multiplication";
        [TypeConverter(typeof(EnumConverter))]
        [DisplayName("Тип")]
        public string type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Значение 1")]
        public string value_1 { get; set; }
        [DisplayName("Значение 2")]
        public string value_2 { get; set; }
        public void Execute()
        {
        }

        public Multiplication()
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
                return new StandardValuesCollection(new[] { ALT1, ALT2 });
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
