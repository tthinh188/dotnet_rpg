using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using dotnet_rpg.Services.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> Get()
        {
            return Ok(await CharacterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> GetSingle(int id)
        {
            var response = await CharacterService.GetCharacterById(id);
            if(response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> CreateCharacter(AddCharacterDTO character)
        {
            return Ok(await CharacterService.CreateCharacter(character));
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDTO>>> UpdateCharacter(int Id, [FromBody] UpdateCharacterDTO updateCharacter)
        {
            var response = await CharacterService.UpdateCharacter(Id, updateCharacter);
            if(response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDTO>>>> DeleteCharacter(int Id)
        {
            var response = await CharacterService.DeleteCharacter(Id);
            if(response.Data == null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}