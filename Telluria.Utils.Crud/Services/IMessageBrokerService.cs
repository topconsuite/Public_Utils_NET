using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;

namespace Topcon.Tech.Domain.Interfaces.Services;

public interface IMessageBrokerService
{
  Task SendMessageAsync(string queueOrTopicName, string message);
  Task SendIntegrationMessageAsync(IntegrationMessage content);
}
