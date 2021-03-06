﻿using Microsoft.AspNet.SignalR;
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
        //public static MessageContext MsgContext = new MessageContext();

        public void Send(string originatorUser, string message)
        {
            // add message to database
            /*MsgContext.Messages.Add(new Message()
            {
                Username = originatorUser,
                MessageText = message,
                Date = DateTime.Today.ToString("dd/MM/yyyy"),
                Time = DateTime.Now.ToString("HH:mm:ss")
            });
            MsgContext.SaveChanges();*/

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

            // delete database
            /*foreach (Message msg in MsgContext.Messages)
            {
                MsgContext.Messages.Remove(msg);
            }
            MsgContext.SaveChanges();*/

            // get last 50 messages from database
            /*List<Message> lastMessages = MsgContext.Messages.OrderByDescending(m => m.MessageId).Take(50).ToList();
            Clients.Caller.lastMessages(lastMessages); */
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