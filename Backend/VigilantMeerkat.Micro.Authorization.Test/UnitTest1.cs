using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VigilantMeerkat.Micro.Authorization.Test.Micro;
using VigilantMeerkat.Micro.Base;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Test;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VigilantMeerkat.Micro.Central;
using System;
using VigilantMeerkat.Micro.ClientCreator;
using VigilantMeerkat.Micro.EmailNotifier;
using VigilantMeerkat.Db;
using System.Linq;
using VigilantMeerkat.Micro.Notifier;
using VigilantMeerkat.Micro.Presenter;

namespace VigilantMeerkat.Micro.Authorization.Test
{
    [TestClass]
    public class UnitTest1
    {

        private AmqpService _amqp = new AmqpService(new RabbitConnection(new Base.Vault.VaultService(new VaultClient(new VaultClientSettings("http://localhost:8201", new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000"))))));

        [ClassInitialize]
        public static async Task Init(TestContext context)
        {
            await TestMicroRunner.Run<AuthorizationMicroservice>();
            await TestMicroRunner.Run<SimpleMicroservice>();
            await TestMicroRunner.Run<ChainedMicroservice>();
            await TestMicroRunner.Run<CentralMicroservice>();
            await TestMicroRunner.Run<ClientCreatorMicroservice>();
            await TestMicroRunner.Run<EmailNotifierMicroservice>();
            await TestMicroRunner.Run<NotifierMicroservice>();
            await TestMicroRunner.Run<PresenterMicroservice>();
        }

        [TestMethod]
        public void VerifyUnauthorized()
        {
            var response = _amqp.Call("authorization", "verify", new MessageContext(), new CommonValue
            {
                Value = "Token"
            }.ToByteArray());

            Assert.AreEqual(BoolValue.Parser.ParseFrom(response.Body).Value, false);
        }

        [TestMethod]
        public void CreateAndVerify()
        {
            var tokenRes = _amqp.Call("authorization", "create");

            var token = CommonValue.Parser.ParseFrom(tokenRes.Body);

            Assert.AreEqual(tokenRes.Result, AmqpResult.Success);

            var response = _amqp.Call("authorization", "verify", new MessageContext(), new CommonValue
            {
                Value = token.Value
            }.ToByteArray());

            Assert.AreEqual(BoolValue.Parser.ParseFrom(response.Body).Value, true);
        }

        [TestMethod]
        public void UnauthorizedMicroserviceCall()
        {
            var response = _amqp.Call("simple", "get");

            Assert.AreEqual(response.Result, AmqpResult.Failed);
        }

        [TestMethod]
        public void AuthorizedMicroserviceCall()
        {
            var tokenRes = _amqp.Call("authorization", "create");

            var token = CommonValue.Parser.ParseFrom(tokenRes.Body);

            Assert.AreEqual(tokenRes.Result, AmqpResult.Success);

            var response = _amqp.Call("simple", "get", new MessageContext
            {
                Token = token.Value
            });

            Assert.AreEqual(response.Result, AmqpResult.Success);
            Assert.AreEqual(CommonValue.Parser.ParseFrom(response.Body).Value, "Test");
        }

        [TestMethod]
        public void ChainedAuth()
        {
            var tokenRes = _amqp.Call("authorization", "create", new MessageContext());

            var token = CommonValue.Parser.ParseFrom(tokenRes.Body);

            Assert.AreEqual(tokenRes.Result, AmqpResult.Success);

            var response = _amqp.Call("chained", "chain", new MessageContext
            {
                Token = token.Value
            });

            Assert.AreEqual(response.Result, AmqpResult.Success);
            Assert.AreEqual(CommonValue.Parser.ParseFrom(response.Body).Value, "Test");
        }

        [TestMethod]
        public void CentralTest()
        {
            var tokenRes = _amqp.Call("authorization", "create");

            var token = CommonValue.Parser.ParseFrom(tokenRes.Body);

            var res = _amqp.Call("central", "put", new MessageContext
            {
                Token = token.Value
            }, new MeerkatData
            {
                Cpu = "1",
                Ram = "12",
                Type = "INFO",
                Timestamp = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            }.ToByteArray());

            Assert.AreEqual(res.Result, AmqpResult.Success);
        }

        [TestMethod]
        public void ClientCreatorTest()
        {
            var res = _amqp.Call("clientcreator", "create", new MessageContext
            {
                Token = new JwtProvider().GenerateToken()
            }, new CommonValue
            {
                Value = "meerkat"
            }.ToByteArray());

            Assert.AreEqual(res.Result, AmqpResult.Success);
        }

        [TestMethod]
        public void EmailNotifierTest()
        {
            var res = _amqp.Call("emailnotifier", "send", new MessageContext(), new NotificationData
            {
                Content = "test",
                To = "kamilek1234567898@gmail.com"
            }.ToByteArray());

            Assert.AreEqual(res.Result, AmqpResult.Success);
        }

        [TestMethod]
        public void NotifierTest()
        {
            var appDb = new AppDbContext();
            var not = new Notification
            {
                MeerkatId = Guid.NewGuid().ToString(),
                Message = "Wiadomoœæ",
                Token = Guid.NewGuid().ToString(),
                Type = "LOG"
            };

            var adm = Guid.NewGuid();
            var id = Guid.NewGuid();

            var tokenInfo = appDb.Tokens.Add(new Db.Model.ClientToken
            {
                Id = id,
                AdminId = adm,
                Name = "name",
                Token = Guid.Parse(not.Token),
                UserId = Guid.NewGuid()
            });

            var clientConfig = appDb.ClientConfigs.Add(new Db.Model.ClientConfig
            {
                Id = Guid.NewGuid(),
                ClientId = id,
                TriggerLevel = "ERROR"
            });

            var admin = appDb.AdminConfigs.Add(new Db.Model.AdminConfig
            {
                Id = adm,
                DefaultNotificationType = "EMAIL",
                Email = "kamilek1234567898@gmail.com",
                PhoneNumber = "123"
            });

            appDb.SaveChanges();

            var res = _amqp.Call("notifier", "notify", new MessageContext(), not.ToByteArray());

            Assert.AreEqual(res.Result, AmqpResult.Success);
        }

        [TestMethod]
        public void PresenterTest()
        {
            var res = _amqp.Call("presenter", "clients", new MessageContext(), new CommonValue
            {
                Value = Guid.NewGuid().ToString()
            }.ToByteArray());

            Assert.AreEqual(res.Result, AmqpResult.Success);
        }
    }
}
