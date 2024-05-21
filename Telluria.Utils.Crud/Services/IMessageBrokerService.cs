using System.Collections.Generic;
using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;

namespace Topcon.Tech.Domain.Interfaces.Services;

public interface IMessageBrokerService
{
  Task SendMessageAsync(string queueOrTopicName, string body);
  Task SendIntegrationMessageAsync(IntegrationMessage integrationMessage);
  Task SendIntegrationMessageAsync(List<IntegrationMessage> integrationMessages);
}
