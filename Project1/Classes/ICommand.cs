
namespace Project1
{
    public interface ICommand
    {
        string Com_name { get; }
        string Title { get; }
        void Execute();
    }
}
