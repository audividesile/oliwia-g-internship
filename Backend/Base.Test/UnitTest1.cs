using System.Threading;
using System.Threading.Tasks;
using VigilantMeerkat.Micro.Base.Attribute;
using VigilantMeerkat.Micro.Base.Config;
using VigilantMeerkat.Micro.Base.Test.Micro;
using Google.Protobuf;
using VigilantMeerkat.Micro.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VigilantMeerkat.Micro.Test;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace VigilantMeerkat.Micro.Base.Test
{
    [TestClass]
    public class UnitTest1
    {
        private AmqpService _amqp = new AmqpService(new RabbitConnection(new Base.Vault.VaultService(new VaultClient(new VaultClientSettings("http://localhost:8201", new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000"))))));

        [ClassInitialize]
        public static async Task Init(TestContext context)
        {
            await TestMicroRunner.Run<StringMicroservice>();
            await TestMicroRunner.Run<ComplexMicroservice>();
            await TestMicroRunner.Run<ConcateMicroservice>();
            await TestMicroRunner.Run<NumberMicroservice>();
            await TestMicroRunner.Run<ExceptionMicroservice>();
            await TestMicroRunner.Run<ComplexExceptionMicroservice>();
            await TestMicroRunner.Run<ProtectedMicroservice>();
            await TestMicroRunner.Run<AuthorizationMicroservice>();
        }
        
        [TestMethod]
        public void SimpleMicroCall()
        {
            var response = _amqp.Call("string", "get");
            
            Assert.AreEqual(CommonValue.Parser.ParseFrom(response.Body).Value, "Test");
        }

        [TestMethod]
        public void ComplexMicroCall()
        {
            var data = new CommonValue
            {
                Value = "test"
            }.ToByteArray();

            var response = _amqp.Call("complex", "get", null, data);
            
            Assert.AreEqual(CommonValue.Parser.ParseFrom(response.Body).Value, "120Testtest");
        }

        [TestMethod]
        public void SimpleException()
        {
            var response = _amqp.Call("exception", "get");
            
            Assert.AreEqual(response.Result, AmqpResult.Failed);
        }
        
        [TestMethod]
        public void ComplexException()
        {
            var response = _amqp.Call("complexexception", "get");
            
            Assert.AreEqual(response.Result, AmqpResult.Failed);
        }

        [TestMethod]
        public void AuthExecute()
        {
            var response = _amqp.Call("protected", "get");
            
            Assert.AreEqual(response.Result, AmqpResult.Failed);
        }
    }
}