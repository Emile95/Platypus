namespace Core.Network
{
    public interface ISeverPortListener<Type>
    {
        void InitializeServerPortListener(int port);
    }
}
