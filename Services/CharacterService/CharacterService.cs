using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Dtos.Character;
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
        private readonly IMapper mapper;

        public CharacterService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> CreateCharacter(AddCharacterDTO character)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            
            Character newCharacter = mapper.Map<Character>(character);

            newCharacter.Id = characters.Max(character => character.Id) + 1;
            
            characters.Add(newCharacter);
            ServiceResponse.Data = characters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int Id)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {
                Character character = characters.First(c => c.Id == Id);
                characters.Remove(character);
                ServiceResponse.Data = characters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            } catch (Exception ex) {
                ServiceResponse.Success = false;
                ServiceResponse.Message = ex.Message;
            }

            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            ServiceResponse.Data = characters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDTO>();
            ServiceResponse.Data = mapper.Map<GetCharacterDTO>(characters.FirstOrDefault(c => c.Id == id));
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(int Id, UpdateCharacterDTO updatedCharacter)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDTO>();
            try {
                Character character = characters.FirstOrDefault(c => c.Id == Id);

                character.Name = updatedCharacter.Name;
                character.Hitpoints = updatedCharacter.Hitpoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = character.Class;

                ServiceResponse.Data = mapper.Map<GetCharacterDTO>(character);
            } catch (Exception ex){
                ServiceResponse.Success = false;
                ServiceResponse.Message = ex.Message;
            }
            return ServiceResponse;
        }
    }
}