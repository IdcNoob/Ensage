namespace Evader.EvadableAbilities.Modifiers.Special
{
    using System.Linq;

    using Ensage;

    internal class OmnislashModifier : EvadableModifier
    {
        private readonly float[] modifierDuration = new float[3];

        private readonly Ability omnislash;

        public OmnislashModifier(
            Team team,
            GetHeroType type,
            Ability ability,
            float maxRemainingTime = 0,
            int minStacks = 0,
            float maxDistanceToSource = 300)
            : base(team, type, maxRemainingTime, minStacks, maxDistanceToSource)
        {
            omnislash = ability;

            var interval = omnislash.AbilitySpecialData.First(x => x.Name == "omni_slash_bounce_tick").Value;
            for (var i = 0u; i < 3; i++)
            {
                modifierDuration[i] =
                    interval * (omnislash.AbilitySpecialData.First(x => x.Name == "omni_slash_jumps").GetValue(i) - 1);
            }
        }

        public override float GetRemainingTime()
        {
            return modifierDuration[omnislash.Level - 1] - Modifier.ElapsedTime;
        }
    }
}