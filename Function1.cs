using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FunctionAppTest
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IConfiguration _configuration;
        private const string RawTopicNameSetting = "%RawTopicName%";
        private const string RawSubscriptionNameSetting = "%RawSubscriptionName%";
        private const string ServiceBusConnectionSetting = "ServiceBusConnection";
        //private const string ServiceBusConnectionSetting = "Ingestion:ServiceBusConn:ReadWrite";

        public Function1(ILogger<Function1> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [Function(nameof(Function1))]
        public async Task Run(
            [ServiceBusTrigger(RawTopicNameSetting, RawSubscriptionNameSetting, Connection =ServiceBusConnectionSetting)]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Function app started");
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
