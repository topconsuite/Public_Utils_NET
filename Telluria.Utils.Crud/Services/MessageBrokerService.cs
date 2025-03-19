using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Telluria.Utils.Crud.Entities;
using Topcon.Tech.Domain.Interfaces.Services;

namespace Telluria.Utils.Crud.Services;

public class MessageBrokerService : IMessageBrokerService, IDisposable
{
  private readonly ServiceBusClient _client;
  private static readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new ConcurrentDictionary<string, ServiceBusSender>();
  private static readonly ConcurrentDictionary<string, SemaphoreSlim> _senderLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
  private readonly string _integrationTopic;
  private bool _disposed;

  public MessageBrokerService(string connectionString, string integrationTopic)
  {
    _integrationTopic = integrationTopic;
    _client = new ServiceBusClient(connectionString);
  }

  private async Task<ServiceBusSender> GetOrCreateSenderAsync(string queueOrTopicName)
  {
    if (_senders.TryGetValue(queueOrTopicName, out var existingSender))
    {
      return existingSender;
    }

    var semaphore = _senderLocks.GetOrAdd(queueOrTopicName, _ => new SemaphoreSlim(1, 1));

    await semaphore.WaitAsync();
    try
    {
      if (!_senders.ContainsKey(queueOrTopicName))
      {
        _senders[queueOrTopicName] = _client.CreateSender(queueOrTopicName);
      }

      return _senders[queueOrTopicName];
    }
    finally
    {
      semaphore.Release();
    }
  }

  public async Task SendMessageAsync(string queueOrTopicName, string body, Dictionary<string, string> properts = null)
  {

    var sender = await GetOrCreateSenderAsync(queueOrTopicName);

    var brokerMessage = new ServiceBusMessage(body);

    if (properts != null && properts.Any())
    {
      foreach (var prop in properts)
      {
        brokerMessage.ApplicationProperties.Add(prop.Key, prop.Value);
      }
    }

    await sender.SendMessageAsync(brokerMessage);
  }

  public async Task SendIntegrationMessageAsync(IntegrationMessage integrationMessage)
  {
    var sender = await GetOrCreateSenderAsync(_integrationTopic);

    var options = new JsonSerializerOptions
    {
      ReferenceHandler = ReferenceHandler.IgnoreCycles,
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
    var sender = await GetOrCreateSenderAsync(_integrationTopic);

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

  public void Dispose()
  {
    if (!_disposed)
    {
      foreach (var sender in _senders.Values)
      {
        sender.DisposeAsync().AsTask().Wait();
      }

      _client.DisposeAsync().AsTask().Wait();
      _disposed = true;
    }
  }
}
