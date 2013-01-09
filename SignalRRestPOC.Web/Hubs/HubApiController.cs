using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace SignalRRestPOC.Web.Hubs {

    public abstract class HubApiController<THub> : ApiController where THub : IHub {

        protected IHubContext HubContext {

            get {
                return GlobalHost.ConnectionManager.GetHubContext<THub>();
            }
        }
    }
}