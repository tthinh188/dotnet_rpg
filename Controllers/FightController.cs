using System.Threading.Tasks;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using dotnet_rpg.Services.FightService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService fightService;
        public FightController(IFightService fightService)
        {
            this.fightService = fightService;
        }

        [HttpPost("Weapon")]
        public async Task<ActionResult<ServiceResponse<AttackResultDTO>>> WeaponAttack(WeaponAttackDTO request)
        {
            return Ok(await fightService.WeaponAttack(request));
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<AttackResultDTO>>> WeaponAttack(SkillAttackDTO request)
        {
            return Ok(await fightService.SkillAttack(request));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDTO>>> WeaponAttack(FightRequestDTO request)
        {
            return Ok(await fightService.Fight(request));
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<HighScoreDTO>>> GetHighScore()
        {
            return Ok(await fightService.GetHighScore());
        }
    }
}