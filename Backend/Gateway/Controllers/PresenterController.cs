using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PresenterController : ControllerBase
    {
        private readonly AmqpService _amqp;

        public PresenterController(AmqpService amqp)
        {
            _amqp = amqp;
        }

        [HttpGet("getLogsForUser/{id}")]
        public IActionResult Index(string id)
        {
            var logs = _amqp.Call("presenter", "logs", new MessageContext(), new CommonValue
            {
                Value = id
            }.ToByteArray());

            return Ok(LogInfoList.Parser.ParseFrom(logs.Body));
        }

        [HttpGet("getLogsForMeerkat/{id}")]
        public IActionResult LogsForMeerkat(string id)
        {
            var logs = _amqp.Call("presenter", "logsformeerkat", new MessageContext(), new CommonValue
            {
                Value = id
            }.ToByteArray());

            return Ok(MeerkatLogList.Parser.ParseFrom(logs.Body));
        }

        [HttpGet("getMeerkatsByUser/{id}")]
        public IActionResult GetMeerkatsByUser(string id)
        {
            var logs = _amqp.Call("presenter", "meerkatsbyuser", new MessageContext(), new CommonValue
            {
                Value = id
            }.ToByteArray());

            return Ok(MeerkatInfoList.Parser.ParseFrom(logs.Body));
        }

        [HttpGet("getAdminClients/{id}")]
        public IActionResult GetAdminClients(string id)
        {
            var logs = _amqp.Call("presenter", "clients", new MessageContext(), new CommonValue
            {
                Value = id
            }.ToByteArray());

            return Ok(AdminClientList.Parser.ParseFrom(logs.Body));
        }
    }
}
