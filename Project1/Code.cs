using System.ComponentModel;

namespace Project1
{
    class Code : ICommand
    {
        [DisplayName("Команда")]
        public string Title
        {
            get
            {
                return "   " + name + ".Код = " + code;
            }
        }

        public const string Com_name = "Code";
        [TypeConverter(typeof(EnumConverter))]
        [DisplayName("Тип")]
        public string type { get; set; }
        [DisplayName("Название")]
        public string name { get; set; }
        [DisplayName("Значение")]
        [TypeConverter(typeof(CodeConverter))]
        public string code { get; set; }
        public void Execute()
        {
        }
        public Code()
        {
            type = EnumConverter.ALT1;
            code = CodeConverter.ATR1;
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

        class CodeConverter : TypeConverter
        {
            public const string ATR1 = "0x1";
            public const string ATR2 = "0x2";
            public const string ATR3 = "0x42";
            public const string ATR4 = "0x4";
            public const string ATR5 = "0x8";
            public const string ATR6 = "0x10";
            public const string ATR7 = "0x20";
            public const string ATR8 = "0x40";
            public const string ATR9 = "0x80";
            public const string ATR10 = "0x100";
            public const string ATR11 = "0x200";
            public const string ATR12 = "0x202";
            public const string ATR13 = "0x400";
            public const string ATR14 = "0x800";
            public const string ATR15 = "0x1000";
            public const string ATR16 = "0x2000";
            public const string ATR17 = "0x4000";
            public const string ATR18 = "0x8000";
            public const string ATR19 = "0x8200";
            public const string ATR20 = "0x10000";
            public const string ATR21 = "0x20000";
            public const string ATR22 = "0x40000";
            public const string ATR23 = "0x80000";
            public const string ATR24 = "0x100000";
            public const string ATR25 = "0x100040";
            public const string ATR26 = "0x200000";
            public const string ATR27 = "0x800000";
            public const string ATR28 = "0x1000000";
            public const string ATR29 = "0x4000000";
            public const string ATR30 = "0x8000000";
            public const string ATR31 = "0x10000000";
            public const string ATR32 = "0x20000000";
            public const string ATR33 = "0x40000000";
            
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new[] { ATR1, ATR2, ATR3, ATR4, ATR5, ATR6, ATR7, ATR8, ATR9, ATR10,
                    ATR11, ATR12, ATR13, ATR14, ATR15, ATR16, ATR17, ATR18, ATR19, ATR20, ATR21, ATR22, ATR23, ATR24,
                ATR25, ATR26, ATR27, ATR28, ATR29, ATR30, ATR31, ATR32, ATR33});
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
