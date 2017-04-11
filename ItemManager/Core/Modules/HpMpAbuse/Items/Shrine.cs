namespace ItemManager.Core.Modules.HpMpAbuse.Items
{
    using System.Linq;

    using Ensage;
    using Ensage.Common.Extensions;

    using Utils;

    internal class Shrine : UsableItem
    {
        private readonly float duration;

        private readonly float hpGrow;

        private readonly float hpSec;

        private readonly float mpGrow;

        private readonly float mpSec;

        public Shrine(string name)
            : base(name)
        {
            var abilityData = Ability.GetAbilityDataByName(Name).AbilitySpecialData.ToList();

            hpSec = abilityData.First(x => x.Name == "hp_heal").Value;
            hpGrow = abilityData.First(x => x.Name == "hp_heal_growth").Value;
            mpSec = abilityData.First(x => x.Name == "mp_heal").Value;
            mpGrow = abilityData.First(x => x.Name == "mp_heal_growth").Value;
            duration = abilityData.First(x => x.Name == "duration").Value;
        }

        protected override float HealthRestore => (hpSec + Game.RawGameTime / 60 * hpGrow) * duration;

        protected override float ManaRestore => (mpSec + Game.RawGameTime / 60 * mpGrow) * duration;

        public override bool CanBeCasted()
        {
            return !Sleeper.Sleeping(Name) && Hero.HasModifier(ModifierUtils.ShrineRegeneration);
        }

        public override void FindItem()
        {
        }

        public override Attribute GetPowerTreadsAttribute()
        {
            var stats = GetDropItemStats();
            var attribute = Attribute.Invalid;

            if (stats.HasFlag(ItemUtils.Stats.Mana))
            {
                attribute = Hero.PrimaryAttribute == Attribute.Agility ? Attribute.Agility : Attribute.Strength;
            }
            else if (stats.HasFlag(ItemUtils.Stats.Health))
            {
                attribute = Hero.PrimaryAttribute == Attribute.Intelligence
                                ? Attribute.Intelligence
                                : Attribute.Agility;
            }
            else if (stats.HasFlag(ItemUtils.Stats.Any))
            {
                attribute = Attribute.Agility;
            }

            return attribute;
        }

        public override void Use(bool queue = true)
        {
            Sleeper.Sleep(duration * 1000 + 500, Name);
        }
    }
}