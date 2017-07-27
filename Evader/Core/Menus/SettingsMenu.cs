namespace Evader.Core.Menus
{
    using System.Collections.Generic;
    using System.Linq;

    using Common;

    using Data;

    using Ensage.Common.Menu;

    internal class SettingsMenu
    {
        private readonly MenuItem defaultPriority;

        private readonly MenuItem defaultToggler;

        public SettingsMenu(Menu rootMenu)
        {
            var menu = new Menu("Settings", "settings");

            defaultPriority = new MenuItem("defaultPriorityFix", "Default priority").SetValue(
                new PriorityChanger(
                    new List<string>
                    {
                        "item_sheepstick",
                        "item_cyclone",
                        "item_blink",
                        "centaur_stampede"
                    },
                    "defaultPriorityChangerFix"));
            menu.AddItem(defaultPriority);
            defaultPriority.ValueChanged += DefaultPriorityOnValueChanged;

            defaultToggler = new MenuItem("defaultTogglerFix", "Enabled priority").SetValue(
                new AbilityToggler(
                    new Dictionary<string, bool>
                    {
                        { "item_sheepstick", false },
                        { "item_cyclone", true },
                        { "item_blink", true },
                        { "centaur_stampede", true }
                    }));
            menu.AddItem(defaultToggler);
            defaultToggler.ValueChanged += DefaultTogglerOnValueChanged;

            var modifierAllyCounter = new MenuItem("modifierAllyCounter", "Modifier ally counter").SetValue(true)
                .SetTooltip("Will use abilities (shields, heals...) on allies");
            menu.AddItem(modifierAllyCounter);
            modifierAllyCounter.ValueChanged += (sender, args) => ModifierAllyCounter = args.GetNewValue<bool>();
            ModifierAllyCounter = modifierAllyCounter.IsActive();

            var modifierEnemyCounter = new MenuItem("modifierEnemyCounter", "Modifier enemy counter").SetValue(true)
                .SetTooltip("Will use abilities (euls, purges, stuns...) on enemies");
            menu.AddItem(modifierEnemyCounter);
            modifierEnemyCounter.ValueChanged += (sender, args) => ModifierEnemyCounter = args.GetNewValue<bool>();
            ModifierEnemyCounter = modifierEnemyCounter.IsActive();

            var pathfinderEffect = new MenuItem("pathfinderEffect", "Pathfinder effect").SetValue(true)
                .SetTooltip("Show particle effect when your hero is controlled by pathfinder");
            menu.AddItem(pathfinderEffect);
            pathfinderEffect.ValueChanged += (sender, args) => PathfinderEffect = args.GetNewValue<bool>();
            PathfinderEffect = pathfinderEffect.IsActive();

            var blockAbilityUsage = new MenuItem("blockPlayerAbilities", "Block ability usage").SetValue(true)
                .SetTooltip("Abilities will be blocked while evading");
            menu.AddItem(blockAbilityUsage);
            blockAbilityUsage.ValueChanged += (sender, args) => BlockAbilityUsage = args.GetNewValue<bool>();
            BlockAbilityUsage = blockAbilityUsage.IsActive();

            var blockPlayerInout = new MenuItem("blockPlayerInput", "Block player input").SetValue(true)
                .SetTooltip("Block actions from player when evading");
            menu.AddItem(blockPlayerInout);
            blockPlayerInout.ValueChanged += (sender, args) => BlockPlayerInput = args.GetNewValue<bool>();
            BlockPlayerInput = blockPlayerInout.IsActive();

            var blockAssemblyInout = new MenuItem("blockAssemblyInput", "Block script input").SetValue(true)
                .SetTooltip("Block actions from other assemblies when evading");
            menu.AddItem(blockAssemblyInout);
            blockAssemblyInout.ValueChanged += (sender, args) => BlockAssemblyInput = args.GetNewValue<bool>();
            BlockAssemblyInput = blockAssemblyInout.IsActive();

            var invisIgnore = new MenuItem("invisIgnore", "Ignore counter if invisible").SetValue(false)
                .SetTooltip("Don't counter enemy abilities if your hero is invisible");
            menu.AddItem(invisIgnore);
            invisIgnore.ValueChanged += (sender, args) => InvisIgnore = args.GetNewValue<bool>();
            InvisIgnore = invisIgnore.IsActive();

            var cancelAnimation = new MenuItem("cancelAnimation", "Cancel animation").SetValue(true)
                .SetTooltip("Cancel cast animation to evade stun ability");
            menu.AddItem(cancelAnimation);
            cancelAnimation.ValueChanged += (sender, args) => CancelAnimation = args.GetNewValue<bool>();
            CancelAnimation = cancelAnimation.IsActive();

            var changer = defaultPriority.GetValue<PriorityChanger>();

            foreach (var priority in changer.Dictionary.OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .Where(x => defaultToggler.GetValue<AbilityToggler>().IsEnabled(x)))
            {
                switch (priority)
                {
                    case "item_sheepstick":
                        DefaultPriority.Add(EvadePriority.Disable);
                        break;
                    case "item_cyclone":
                        DefaultPriority.Add(EvadePriority.Counter);
                        break;
                    case "item_blink":
                        DefaultPriority.Add(EvadePriority.Blink);
                        break;
                    case "centaur_stampede":
                        DefaultPriority.Add(EvadePriority.Walk);
                        break;
                }
            }

            //menu.AddItem(mouseEmulation = new MenuItem("mouseEmulation", "Mouse emulation").SetValue(false))
            //    .ValueChanged += (sender, args) =>
            //        {
            //            MouseEmulation = args.GetNewValue<bool>();
            //        };
            // MouseEmulation = mouseEmulation.IsActive();

            rootMenu.AddSubMenu(menu);
        }

        public bool BlockAbilityUsage { get; private set; }

        public bool BlockAssemblyInput { get; private set; }

        public bool BlockPlayerInput { get; private set; }

        public bool CancelAnimation { get; private set; }

        public List<EvadePriority> DefaultPriority { get; } = new List<EvadePriority>();

        //public bool MouseEmulation { get; private set; }

        public bool InvisIgnore { get; private set; }

        public bool ModifierAllyCounter { get; private set; }

        public bool ModifierEnemyCounter { get; private set; }

        public bool PathfinderEffect { get; private set; }

        private void DefaultPriorityOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            var changer = args.GetNewValue<PriorityChanger>();
            DefaultPriority.Clear();

            foreach (var item in changer.Dictionary.OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .Where(x => defaultToggler.GetValue<AbilityToggler>().IsEnabled(x)))
            {
                switch (item)
                {
                    case "item_sheepstick":
                        DefaultPriority.Add(EvadePriority.Disable);
                        break;
                    case "item_cyclone":
                        DefaultPriority.Add(EvadePriority.Counter);
                        break;
                    case "item_blink":
                        DefaultPriority.Add(EvadePriority.Blink);
                        break;
                    case "centaur_stampede":
                        DefaultPriority.Add(EvadePriority.Walk);
                        break;
                }
            }

            Debugger.Write("Priority changed: ");
            for (var i = 0; i < DefaultPriority.Count; i++)
            {
                Debugger.Write(DefaultPriority.ElementAt(i).ToString(), showType: false);

                if (DefaultPriority.Count - 1 > i)
                {
                    Debugger.Write(" => ", showType: false);
                }
            }

            Debugger.WriteLine(showType: false);
        }

        private void DefaultTogglerOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            var changer = defaultPriority.GetValue<PriorityChanger>();
            DefaultPriority.Clear();

            foreach (var item in changer.Dictionary.OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .Where(x => args.GetNewValue<AbilityToggler>().IsEnabled(x)))
            {
                switch (item)
                {
                    case "item_sheepstick":
                        DefaultPriority.Add(EvadePriority.Disable);
                        break;
                    case "item_cyclone":
                        DefaultPriority.Add(EvadePriority.Counter);
                        break;
                    case "item_blink":
                        DefaultPriority.Add(EvadePriority.Blink);
                        break;
                    case "centaur_stampede":
                        DefaultPriority.Add(EvadePriority.Walk);
                        break;
                }
            }

            Debugger.Write("Priority changed: ");
            for (var i = 0; i < DefaultPriority.Count; i++)
            {
                Debugger.Write(DefaultPriority.ElementAt(i).ToString(), showType: false);

                if (DefaultPriority.Count - 1 > i)
                {
                    Debugger.Write(" => ", showType: false);
                }
            }

            Debugger.WriteLine(showType: false);
        }

        //private readonly MenuItem mouseEmulation;
    }
}