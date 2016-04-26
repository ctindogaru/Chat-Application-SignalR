using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatApplication
{
    public class ChatHub : Hub
    {
        public static Dictionary<string, string> ConnectedUsers;

        public void Send(string originatorUser, string message)
        {
            Clients.All.messageReceived(originatorUser, message);
        }

        public void Connect(string newUser)
        {
            if (ConnectedUsers == null)
                ConnectedUsers = new Dictionary<string, string>();

            ConnectedUsers.Add(Context.ConnectionId, newUser);

            List<string> temp = new List<string>();
            foreach (var pair in ConnectedUsers)
                temp.Add(pair.Value);

            Clients.Caller.getConnectedUsers(temp);
            Clients.Others.newUserAdded(newUser);
            
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string userName = null;
            foreach (var pair in ConnectedUsers)
                if (pair.Key == Context.ConnectionId)
                {
                    userName = pair.Value;
                    break;
                }

            ConnectedUsers.Remove(Context.ConnectionId);

            List<string> temp = new List<string>();
            foreach (var pair in ConnectedUsers)
                temp.Add(pair.Value);

            Clients.Others.getConnectedUsers(temp);

            return base.OnDisconnected(stopCalled);
        }
    }
}