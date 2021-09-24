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
            var dbCharacters = await context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => c.User.Id == GetUserId()).ToListAsync();
            ServiceResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDTO>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                var dbCharacter = await context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstAsync(c => c.Id == id && c.User.Id == GetUserId());
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
                Character character = await context.Characters
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == Id);
                if(character.User.Id == GetUserId())
                {
                    character.Name = updatedCharacter.Name;
                    character.Hitpoints = updatedCharacter.Hitpoints;
                    character.Strength = updatedCharacter.Strength;
                    character.Defense = updatedCharacter.Defense;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.Class = character.Class;

                    await context.SaveChangesAsync();

                    ServiceResponse.Data = mapper.Map<GetCharacterDTO>(character);
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

        public async Task<ServiceResponse<GetCharacterDTO>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try {
                var character = await context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());

                if(character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var skill = await context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if(skill == null)
                {
                    response.Success = false;
                    response.Message = "Skill not found.";
                    return response;
                }

                character.Skills.Add(skill);
                await context.SaveChangesAsync();

                response.Data = mapper.Map<GetCharacterDTO>(character);
            } catch (Exception ex) {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}