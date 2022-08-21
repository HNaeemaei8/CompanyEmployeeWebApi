using Contracts.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

            private ILoggerManager _logger;
            private IRepositoryManager _repositoryManager;
            public WeatherForecastController(ILoggerManager logger, IRepositoryManager repositoryManager)
            {
                _logger = logger;
            _repositoryManager = repositoryManager;
            }
            [HttpGet]
            public IEnumerable<string> Get()
            {
                _logger.LogInfo("Here is info message from our values controller.");
                _logger.LogDebug("Here is debug message from our values controller.");
                _logger.LogWarn("Here is warn message from our values controller.");
                _logger.LogError("Here is an error message from our values controller.");
                
            return new string[] { "value1", "value2" };
            }
        }
    }

