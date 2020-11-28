using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirewallCtlUI.DB;
using FirewallCtlUI.DTO;
using FirewallCtlUI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FirewallCtlUI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly CaptureService _captureService;
        private readonly FWContext _context;

        public ApiController(ILogger<ApiController> logger, CaptureService captureService,FWContext context)
        {
            _logger = logger;
            _captureService = captureService;
            _context = context;
        }

        [HttpPost("GetDevices")]
        public IEnumerable<CaptureDevice> GetDevices()
        {
            return _captureService.GetDevices();
        }

        [HttpPost("GetSettings")]
        public IActionResult GetSettings()
        {
            var s=_context.Parameters.FirstOrDefault(i => i.Name == "Settings");

            if (s == null)
                return Ok(new Settings());

            return Ok(JsonConvert.DeserializeObject<Settings>(s.Value));
        }

        [HttpPost("SaveSettings")]
        public IActionResult SaveSettings([FromBody]Settings settings)
        {
            var s = _context.Parameters.FirstOrDefault(i => i.Name == "Settings");

            if (s == null)
            {
                s = new Parameter() { Name = "Settings" };
                _context.Parameters.Add(s);
            }

            s.Value = JsonConvert.SerializeObject(settings);
            _context.SaveChanges();
            _captureService.SetDevice(settings.Device);
            return Ok();
        }
    }
}
