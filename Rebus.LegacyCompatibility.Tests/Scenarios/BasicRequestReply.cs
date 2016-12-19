using System;
using System.Threading;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.Legacy;
using Rebus.Logging;
using Rebus.Routing;
using Rebus.Routing.TypeBased;
using Rebus.Transport.InMem;
#pragma warning disable 1998

namespace Rebus.LegacyCompatibility.Tests.Scenarios
{
    [TestFixture]
    public class BasicRequestReply : FixtureBase
    {
        readonly InMemNetwork _network = new InMemNetwork(outputEventsToConsole: true);
        BuiltinHandlerActivator _requestor;
        BuiltinHandlerActivator _replier;

        [SetUp]
        public void SetUp()
        {
            _network.Reset();

            _requestor = CreateBus("requestor", r => r.TypeBased().Map<string>("replier"));
            _replier = CreateBus("replier");
        }

        [Test]
        public void GreatName()
        {
            var gotReply = new ManualResetEvent(false);

            _replier.Handle<string>(async (bus, request) =>
            {
                var reply = $"Got message {request}";

                Console.WriteLine($"Received request '{request}' - returning reply '{reply}'", request, reply);

                await bus.Reply(reply);
            });

            _requestor.Handle<string>(async reply =>
            {
                Console.WriteLine($"Received reply '{reply}'", reply);

                gotReply.Set();
            });

            _requestor.Bus.Send("HEJ MED DIG").Wait();

            if (!gotReply.WaitOne(TimeSpan.FromSeconds(2)))
            {
                throw new AssertionException($"Did not receive reply withing 2 s timeout");
            }
        }

        BuiltinHandlerActivator CreateBus(string queueName, Action<StandardConfigurer<IRouter>> routerConfigurer = null)
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            Configure.With(activator)
                .Logging(l => l.Console(LogLevel.Warn))
                .Transport(t => t.UseInMemoryTransport(_network, queueName))
                .Routing(r => routerConfigurer?.Invoke(r))
                .Options(o => o.EnableLegacyCompatibility())
                .Start();

            return activator;
        }
    }
}