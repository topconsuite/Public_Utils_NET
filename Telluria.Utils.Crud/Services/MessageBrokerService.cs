using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

  public async Task SendMessageAsync(string queueOrTopicName, string body)
  {
    await using var client = new ServiceBusClient(_connectionString);

    var sender = client.CreateSender(queueOrTopicName);

    var brokerMessage = new ServiceBusMessage(body);

    await sender.SendMessageAsync(brokerMessage);
  }

  public async Task SendIntegrationMessageAsync(IntegrationMessage integrationMessage)
  {
    await using var client = new ServiceBusClient(_connectionString);

    var sender = client.CreateSender(_integrationTopic);

    var options = new JsonSerializerOptions
    {
      ReferenceHandler = ReferenceHandler.Preserve,
      WriteIndented = true
    };

    var messageBody = JsonSerializer.Serialize(integrationMessage, options);

    var brokerMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
    {
      MessageId = Guid.NewGuid().ToString(),
      ContentType = "application/json",
    };

    brokerMessage.ApplicationProperties.Add("TenantId", integrationMessage.TenantId);
    brokerMessage.ApplicationProperties.Add("MessageType", integrationMessage.Entity);

    await sender.SendMessageAsync(brokerMessage);
  }

  public async Task SendIntegrationMessageAsync(List<IntegrationMessage> integrationMessages)
  {
    await using var client = new ServiceBusClient(_connectionString);

    var sender = client.CreateSender(_integrationTopic);

    var options = new JsonSerializerOptions
    {
      ReferenceHandler = ReferenceHandler.Preserve,
      WriteIndented = true
    };

    var brokerMessages = new List<ServiceBusMessage>();

    foreach (var integrationMessage in integrationMessages)
    {
      var messageBody = JsonSerializer.Serialize(integrationMessage, options);

      var brokerMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))
      {
        MessageId = Guid.NewGuid().ToString(),
        ContentType = "application/json",
      };

      brokerMessage.ApplicationProperties.Add("TenantId", integrationMessage.TenantId);
      brokerMessage.ApplicationProperties.Add("MessageType", integrationMessage.Entity);

      brokerMessages.Add(brokerMessage);
    }

    await sender.SendMessagesAsync(brokerMessages);
  }
}
