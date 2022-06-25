using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Project1
{  
    public class Signal : IComparable<Signal>
    {
        [DisplayName("Название")]
        public string name { get; set; }
        [TypeConverter(typeof(EnumConverter))]
        [DisplayName("Тип")]
        public string Type { get; set; } 
        [DisplayName("Номер")]
        public int id { get; set; }
        [DisplayName("Начальное значение")]
        public double start_value { get; set; }
        [Browsable(false)]
        [DisplayName("Значение")]
        [ReadOnly(true)]
        public double value { get; set; }
        [Browsable(false)]
        [DisplayName("Последнее значение")]
        [ReadOnly(true)]
        public double last_value { get; set; }
        [TypeConverter(typeof(CodeConverter))]
        [DisplayName("Код передачи")]
        public string code { get; set; }
        [TypeConverter(typeof(KindConverter))]
        [DisplayName("Передача")]
        public string kind { get; set; }       
        [DisplayName("Время")]
        public double prop { get; set; }
        [Browsable(false)]
        [DisplayName("Последняя команда")]
        public string last_command { get; set; }
        [Browsable(false)]
        [DisplayName("Минимальное значение")]
        public double value_min { get; set; }
        [Browsable(false)]
        [DisplayName("Максимальное значение")]
        public double value_max { get; set; }
        [Browsable(false)]
        [DisplayName("Период")]
        public double period { get; set; }
        [Browsable(false)]
        [DisplayName("Текущее время")]
        public double tek_time { get; set; }        
        [Browsable(false)]
        [DisplayName("Средина периода")]
        public double middle { get; set; }
        [Browsable(false)]
        [DisplayName("Текущее значение")]
        public double tek_value { get; set; }
        [Browsable(false)]
        [DisplayName("Стартовый код передачи")]
        public string start_code { get; set; }
        [Browsable(false)]
        [DisplayName("Последний код передачи")]
        public string last_code { get; set; }

        public int CompareTo(Signal other)
        {
            return id.CompareTo(other.id);
        }
        
        public Signal()
        {
            Type = EnumConverter.ALT1;
            code = CodeConverter.ATR1;
            kind = KindConverter.ALK1;
        }

        class KindConverter : TypeConverter
        {
            public const string ALK1 = "Циклическая";
            public const string ALK2 = "По изменению";

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new[] { ALK1, ALK2 });
            }
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
            public const string ATR3 ="0x42";
            public const string ATR4 ="0x4";
            public const string ATR5 ="0x8";
            public const string ATR6 ="0x10";
            public const string ATR7 ="0x20";
            public const string ATR8 ="0x40";
            public const string ATR9 ="0x80";
            public const string ATR10 ="0x100";
            public const string ATR11 ="0x200";
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
            public const string ATR27 ="0x800000";
            public const string ATR28 ="0x1000000";
            public const string ATR29 ="0x4000000";
            public const string ATR30 ="0x8000000";
            public const string ATR31 ="0x10000000";
            public const string ATR32 = "0x20000000";
            public const string ATR33 ="0x40000000";
            
            
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(new[] { ATR1, ATR2, ATR3, ATR4, ATR5, ATR6, ATR7, ATR8, ATR9, ATR10, 
                    ATR11, ATR12, ATR13, ATR14, ATR15, ATR16, ATR17, ATR18, ATR19, ATR20, ATR21, ATR22, ATR23, ATR24,
                ATR25, ATR26, ATR27, ATR28, ATR29, ATR30, ATR31, ATR32, ATR33});
            }
        }
    }
}
