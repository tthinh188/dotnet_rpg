using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Character;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }
        private int GetUserId() => int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try {
                var character = await context.Characters
                    .FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User.Id == GetUserId());
                if(character == null)
                {
                    response.Success = false;
                    response.Message = "Character not found.";
                    return response;
                }

                var weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };

                context.Weapons.Add(weapon);
                await context.SaveChangesAsync();

                response.Data = mapper.Map<GetCharacterDTO>(character);
            } catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}