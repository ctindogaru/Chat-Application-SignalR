using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ChatApplication
{
    public class MessageContext : DbContext
    {
        public MessageContext() : base("name=MessageDbConnectionString") { }

        public DbSet<Message> Messages { get; set; }
    }
}