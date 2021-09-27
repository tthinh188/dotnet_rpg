using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos.Fight;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        public FightService(DataContext context, IMapper mapper)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<ServiceResponse<FightResultDTO>> Fight(FightRequestDTO request)
        {
            var response = new ServiceResponse<FightResultDTO>
            {
                Data = new FightResultDTO()
            };

            try
            {
                var Characters = await context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;
                while (!defeated)
                {
                    foreach (var attacker in Characters)
                    {
                        var opponents = Characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, opponent);

                        }
                        else
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if (opponent.Hitpoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} win with {attacker.Hitpoints} HP left!");
                            break;

                        }
                    }
                }

                Characters.ForEach(c =>
                {
                    c.Fights++;
                    c.Hitpoints = 300;
                });

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDTO>> SkillAttack(SkillAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();

            try
            {
                var attacker = await context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill == null)
                {
                    response.Success = false;
                    response.Message = $"{attacker.Name} doesn't know this skill.";
                    return response;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);

                if (opponent.Hitpoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await context.SaveChangesAsync();

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.Hitpoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.Hitpoints,
                    Damage = damage
                };

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoSkillAttack(Character attacker, Character opponent, Skill skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.Hitpoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();

            try
            {
                var attacker = await context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);

                var opponent = await context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);

                if (opponent.Hitpoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                await context.SaveChangesAsync();

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    AttackerHP = attacker.Hitpoints,
                    Opponent = opponent.Name,
                    OpponentHP = opponent.Hitpoints,
                    Damage = damage
                };

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
                opponent.Hitpoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDTO>>> GetHighScore()
        {
            var characters = await context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            var response = new ServiceResponse<List<HighScoreDTO>>
            {
                Data = characters.Select(c => mapper.Map<HighScoreDTO>(c)).ToList()
            };

            return response;
        }
    }
}