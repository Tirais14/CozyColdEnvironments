namespace UTIRLib.UI
{
    public interface IOpenable
    {
        bool IsOpened { get; }

        void Open();

        void Close();
    }
}