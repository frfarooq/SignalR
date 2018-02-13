using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR.Hubs
{
    public class MyNewHub : Hub
    {
        public static void Send()
        {
            IHubContext context =  GlobalHost.ConnectionManager.GetHubContext<MyNewHub>();
            context.Clients.All.displayStatus();
        }
    }
}