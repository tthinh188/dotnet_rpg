using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Models;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>()
        {
            new Character(),
            new Character { Id = 1, Name = "Sam"}
        };

        public async Task<ServiceResponse<List<Character>>> CreateCharacter(Character character)
        {
            var ServiceResponse = new ServiceResponse<List<Character>>();
            characters.Add(character);
            ServiceResponse.Data = characters;
            return ServiceResponse;        
        }

        public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
        {
            var ServiceResponse = new ServiceResponse<List<Character>>();
            ServiceResponse.Data = characters;
            return ServiceResponse;
        }

        public async Task<ServiceResponse<Character>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<Character>();
            ServiceResponse.Data = characters.FirstOrDefault(c => c.Id == id);
            return ServiceResponse;
        }
    }
}