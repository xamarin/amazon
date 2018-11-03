﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Pollster.PollVoter.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "PollVoter";
        }
    }
}
