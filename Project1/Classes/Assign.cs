using System.ComponentModel;

namespace Project1
{
    public class Assign : ICommand
    {
        [DisplayName("Команда")]
        public string Title 
        {
            get { return "   " + name + "=" + value.ToString(); }
        }

        public const string Com_name = "Assign";

        [TypeConverter(typeof(EnumConverter))]
        [DisplayName("Тип")]
        public string type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Значение")]
        public double value { get; set; }
        public void Execute()
        {
        } 
        public Assign()
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
