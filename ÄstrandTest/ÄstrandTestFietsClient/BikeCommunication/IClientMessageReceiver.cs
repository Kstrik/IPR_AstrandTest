namespace ÄstrandTestFietsClient.BikeCommunication
{
    public interface IClientMessageReceiver
    {
        void HandleClientMessage(ClientMessage clientMessage);
    }
}