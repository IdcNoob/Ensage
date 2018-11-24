namespace ItemManager.Utils
{
    using Ensage;

    internal static class EntityUtils
    {
        public static float HealthPercentage(this Entity entity)
        {
            return ((float)entity.Health / entity.MaximumHealth) * 100;
        }

        public static bool IsSentryWard(this Entity entity)
        {
            return entity.NetworkName == "CDOTA_NPC_Observer_Ward_TrueSight";
        }

        public static bool IsTechiesMine(this Entity entity)
        {
            return entity.NetworkName == "CDOTA_NPC_TechiesMines";
        }

        public static bool IsWard(this Entity entity)
        {
            return entity.NetworkName == "CDOTA_NPC_Observer_Ward" || entity.NetworkName == "CDOTA_NPC_Observer_Ward_TrueSight";
        }
    }
}