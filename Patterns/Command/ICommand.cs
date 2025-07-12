namespace UTIRLib.Patterns.Command
{
#nullable enable

    public interface ICommand
    {
        void Execute();

        void Undo();
    }
}