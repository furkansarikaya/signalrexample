namespace SignalRExample.API.Hubs;

public interface IMyHub
{
    Task Error(string message);
    Task ReceiveName(string name);
    Task ReceiveNames(List<string> names);
    Task ReceiveNamesByGroup(object obj);
    Task ReceiveClientCount(int clientCount);
    Task ReceiveMessageByGroup(string name, int teamId);
}