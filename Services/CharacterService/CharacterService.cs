using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.mapper = mapper;
        }

        private int GetUserId() => int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDTO>>> CreateCharacter(AddCharacterDTO character)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            Character newCharacter = mapper.Map<Character>(character);
            newCharacter.User = await context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            context.Characters.Add(newCharacter);
            await context.SaveChangesAsync();
            ServiceResponse.Data = await context.Characters
                .Where(c=> c.User.Id == GetUserId())
                .Select(c => mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int Id)
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            try
            {
                Character character = await context.Characters.FirstAsync(c => c.Id == Id && c.User.Id == GetUserId());
                if(character != null)
                {
                    context.Characters.Remove(character);
                    await context.SaveChangesAsync();
                    ServiceResponse.Data = context.Characters
                        .Where(c => c.User.Id == GetUserId())
                        .Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
                }               
                else {
                    ServiceResponse.Success = false;
                    ServiceResponse.Message = "Character Not found.";
                }
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
            var dbCharacters = await context.Characters.Where(c => c.User.Id == GetUserId()).ToListAsync();
            ServiceResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var dbCharacter = await context.Characters.FirstAsync(c => c.Id == id && c.User.Id == GetUserId());
                ServiceResponse.Data = mapper.Map<GetCharacterDTO>(dbCharacter);
            }
            catch (Exception ex)
            {
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