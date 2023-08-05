using System.Net.Http;
using System.Net.WebSockets;
using LCT.Application.Teams.Events.Actions;
using LCT.Infrastructure.MessageBrokers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using StackExchange.Redis;

namespace LCT.Infrastructure.ClientCommunication.Hubs
{
    public class TournamentHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly ConnectionMultiplexer _connection;
        public TournamentHub(IMediator mediator, RedisSettings redisSettings)
        {
            _mediator = mediator;
            //_connection = ConnectionMultiplexer.Connect(redisSettings.ConnectionString, options =>
            //{
            //    options.Password = redisSettings.Password;
            //});
        }
        public async Task TeamClicked(TeamClickedAction action)
        {
            await _mediator.Publish(action);
        }

        public override async Task OnConnectedAsync()
        {
            // tournament id should be sent from client
            IHttpContextFeature hcf = (IHttpContextFeature)this.Context.Features[typeof(IHttpContextFeature)];
            HttpContext hc = hcf.HttpContext;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await base.OnDisconnectedAsync(ex);
        }

    }
}
