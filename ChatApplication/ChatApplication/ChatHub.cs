using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication
{
    public class ChatHub : Hub
    {
        public static Dictionary<string, string> ConnectedUsers;
        public static MessageContext MsgContext = new MessageContext();

        public void Send(string originatorUser, string message)
        {

            if (MsgContext.Messages.Take(1).Count() == 0)
            {
                MsgContext.Messages.Add(new Message()
                {
                    Username = originatorUser,
                    MessageId = 0,
                    MessageText = message,
                    Date = DateTime.Today.ToString("dd/MM/yyyy"),
                    Time = DateTime.Now.ToString("HH:mm:ss")
                });

                MsgContext.SaveChanges();
            }
            else
            {
                var lastMessage = MsgContext.Messages.Take(1).First();
                MsgContext.Messages.Add(new Message()
                {
                    Username = originatorUser,
                    MessageId = lastMessage.MessageId + 1,
                    MessageText = message,
                    Date = DateTime.Today.ToString("dd/MM/yyyy"),
                    Time = DateTime.Now.ToString("HH:mm:ss")
                });

                MsgContext.SaveChanges();
            }

            Clients.All.messageReceived(originatorUser, message, DateTime.Now.ToString("HH:mm:ss"));
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

            // stergere baza de date
            /*foreach (Message msg in MsgContext.Messages)
            {
                MsgContext.Messages.Remove(msg);
            }*/

            List<Message> lastMessages = MsgContext.Messages.Take(50).ToList();
            List<string> users = new List<string>();
            List<string> messages = new List<string>();
            List<string> dates = new List<string>();
            List<string> times = new List<string>();

            foreach (Message msg in lastMessages)
            {
                users.Add(msg.Username);
                messages.Add(msg.MessageText);
                dates.Add(msg.Date);
                times.Add(msg.Time);
            }

            Clients.Caller.lastMessages(users, messages, dates, times);
            
            
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