using System.Collections.Generic;
using dotnet_rpg.Dtos.Skill;
using dotnet_rpg.Dtos.Weapon;
using dotnet_rpg.Models;

namespace dotnet_rpg.Dtos.Character
{
    public class GetCharacterDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Frodo";
        public int Hitpoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public RpgClass Class { get; set; } = RpgClass.Knight;
        public GetWeaponDTO Weapon { get; set; }
        public List<GetSkillDTO> Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}