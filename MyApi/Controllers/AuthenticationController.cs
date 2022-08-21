using AutoMapper;
using Contracts.IServices;
using Entities;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Infrastructure.ActionFilters;

namespace WebApi.Controllers
{
    [Route("api/Authentication")]
    public class AuthenticationController : ControllerBase
    {

        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IAuthenticationManager _authManager;

        public AuthenticationController(ILoggerManager logger, IMapper
        mapper, UserManager<User> userManager, IAuthenticationManager authManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _authManager = authManager;

        }

        [HttpPost("login")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> AuthenticateAsync([FromBody]
        UserForAuthenticationDto user)
        {
            if (!await _authManager.ValidateUserAsync(user))
            {
                _logger.LogWarn($"{nameof(AuthenticateAsync)}: Authentication failed.Wrong user name or password.");
            return Unauthorized();
            }
            return Ok(new { Token = await _authManager.CreateTokenAsync() });
        }



        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUserAsync([FromBody] UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user,
            userForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code,
                    error.Description);
                }
                return BadRequest(ModelState);
            }
            await _userManager.AddToRolesAsync(user,
            userForRegistration.Roles);
            return StatusCode(201);
        }




    }

}
        

    