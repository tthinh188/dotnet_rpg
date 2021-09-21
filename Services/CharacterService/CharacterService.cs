using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> CreateCharacter(AddCharacterDTO character)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            Character newCharacter = mapper.Map<Character>(character);
            // newCharacter.Id = characters.Max(character => character.Id) + 1;
            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();
            ServiceResponse.Data = await context.Characters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int Id)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {
                Character character = await context.Characters.FirstAsync(c => c.Id == Id);
                context.Characters.Remove(character);
                await context.SaveChangesAsync();
                ServiceResponse.Data = context.Characters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            }
            catch (Exception ex)
            {
                ServiceResponse.Success = false;
                ServiceResponse.Message = ex.Message;
            }

            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var dbCharacters = await context.Characters.ToListAsync();
            ServiceResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDTO>();
            try {
                var dbCharacter = await context.Characters.FirstAsync(c => c.Id == id);
                ServiceResponse.Data = mapper.Map<GetCharacterDTO>(dbCharacter);
            } catch (Exception ex){
                ServiceResponse.Success = false;
                ServiceResponse.Message = ex.Message;
            }
           
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(int Id, UpdateCharacterDTO updatedCharacter)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                Character character = await context.Characters.FirstOrDefaultAsync(c => c.Id == Id);

                character.Name = updatedCharacter.Name;
                character.Hitpoints = updatedCharacter.Hitpoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = character.Class;

                await context.SaveChangesAsync();

                ServiceResponse.Data = mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                ServiceResponse.Success = false;
                ServiceResponse.Message = ex.Message;
            }
            return ServiceResponse;
        }
    }
}