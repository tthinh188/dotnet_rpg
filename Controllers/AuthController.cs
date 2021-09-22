using System.Threading.Tasks;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.User;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepo;
        public AuthController(IAuthRepository authRepo)
        {
            this.authRepo = authRepo;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDTO request)
        {
            //   "userName": "tthinh123",
            //   "password": "abcdefgh"
            var response = await authRepo.Register(
                new User { Username = request.UserName },
                request.Password
            );

            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDTO request)
        {
            var response = await authRepo.Login(request.UserName, request.Password);

            if(!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}