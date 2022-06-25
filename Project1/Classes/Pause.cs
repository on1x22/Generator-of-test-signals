using System.ComponentModel;

namespace Project1
{
    class Pause : ICommand
    {
        [DisplayName("Команда")]
        public string Title
        {
            get { return "Пауза" + "=" + time; }
        }

        public const string Com_name = "Pause";
        [DisplayName("Время")]
        public double time { get; set; }
        public void Execute()
        {                      
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
