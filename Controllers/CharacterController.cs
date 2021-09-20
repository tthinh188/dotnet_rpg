using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        public ICharacterService CharacterService;
        public CharacterController(ICharacterService characterService)
        {
            this.CharacterService = characterService;

        }
        [HttpGet]
        public async Task<ActionResult<List<Character>>> Get()
        {
            return Ok(await CharacterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetSingle(int id)
        {
            return Ok(await CharacterService.GetCharacterById(id));
        }

        [HttpPost]
        public async Task<ActionResult<Character>> CreateCharacter(Character character)
        {
            return Ok(await CharacterService.CreateCharacter(character));
        }
    }
}