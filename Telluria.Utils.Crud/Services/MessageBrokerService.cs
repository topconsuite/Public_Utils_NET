using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Telluria.Utils.Crud.Entities;
using Topcon.Tech.Domain.Interfaces.Services;

namespace Telluria.Utils.Crud.Services;

public class MessageBrokerService : IMessageBrokerService
{
  private readonly string _connectionString;
  private readonly string _integrationTopic;

  public MessageBrokerService(string connectionString, string integrationTopic)
  {
    _connectionString = connectionString;
    _integrationTopic = integrationTopic;
  }

  public async Task SendMessageAsync(string queueOrTopicName, string content)
  {
    await using var client = new ServiceBusClient(_connectionString);

    var sender = client.CreateSender(queueOrTopicName);

    await sender.SendMessageAsync(new ServiceBusMessage(content));
  }

  public async Task SendIntegrationMessageAsync(IntegrationMessage content)
  {
    await using var client = new ServiceBusClient(_connectionString);

    var sender = client.CreateSender(_integrationTopic);

    var messageBody = JsonSerializer.Serialize(content);

    var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
    {
      MessageId = Guid.NewGuid().ToString(),
      ContentType = "application/json",
    };

    message.ApplicationProperties.Add("TenantId", content.TenantId);
    message.ApplicationProperties.Add("MessageType", content.Entity);

    await sender.SendMessageAsync(message);
  }
}
