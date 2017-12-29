namespace SimpleSD
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings(string ownerName)
        {
            this.factory = MenuFactory.CreateWithTexture("SD Poison Spam", ownerName);
            this.Key = this.factory.Item("Key", new KeyBind('F'));
            this.DrawTargetParticle = this.factory.Item("Draw target particle", true);
            this.Attack = this.factory.Item("Attack", true);
            this.Move = this.factory.Item("Move", true);
            this.CancelAnimation = this.factory.Item("Cancel animation", true);
        }

        public MenuItem<bool> Attack { get; }

        public MenuItem<bool> CancelAnimation { get; }

        public MenuItem<bool> DrawTargetParticle { get; }

        public MenuItem<KeyBind> Key { get; }

        public MenuItem<bool> Move { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}