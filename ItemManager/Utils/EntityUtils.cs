namespace ItemManager.Utils
{
    using Ensage;

    internal static class EntityUtils
    {
        public static bool IsTechiesMine(this Entity entity)
        {
            return entity.ClassId == ClassId.CDOTA_NPC_TechiesMines;
        }

        public static bool IsWard(this Entity entity)
        {
            return entity.ClassId == ClassId.CDOTA_NPC_Observer_Ward
                   || entity.ClassId == ClassId.CDOTA_NPC_Observer_Ward_TrueSight;
        }
    }
}