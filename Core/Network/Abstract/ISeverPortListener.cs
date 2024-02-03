namespace Core.Network.Abstract
{
    public interface ISeverPortListener<Type>
    {
        void InitializeServerPortListener(int port);
    }
}
