using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using VigilantMeerkat.Micro.Base;
using Microsoft.Net.Http.Headers;

namespace VigilantMeerkat.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientCreatorController : ControllerBase
    {
        private readonly AmqpService _amqp;

        public ClientCreatorController(AmqpService amqp)
        {
            _amqp = amqp;
        }

        [HttpGet("create/{id}")]
        public IActionResult Index(string id)
        {
            var jwt = Request.Headers[HeaderNames.Authorization].ToString();

            jwt = jwt.Replace("Bearer ", "");

            var msg = _amqp.Call("clientcreator", "create", new MessageContext
            {
                Token = jwt
            }, new CommonValue
            {
                Value = id
            }.ToByteArray());

            return Ok(CommonValue.Parser.ParseFrom(msg.Body).Value);
        }
    }
}
