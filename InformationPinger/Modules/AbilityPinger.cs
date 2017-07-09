namespace InformationPinger.Modules
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Service;

    using Interfaces;

    using PingTypes;

    [Export(typeof(IModule))]
    internal class AbilityPinger : IModule
    {
        private readonly AbilityId[] abilities =
        {
            AbilityId.tidehunter_ravage,
            AbilityId.enigma_black_hole,
            AbilityId.skeleton_king_reincarnation,
            AbilityId.ancient_apparition_ice_blast,
            AbilityId.batrider_flaming_lasso,
            AbilityId.beastmaster_primal_roar,
            AbilityId.bloodseeker_rupture,
            AbilityId.rattletrap_hookshot,
            AbilityId.doom_bringer_doom,
            AbilityId.earthshaker_echo_slam,
            AbilityId.faceless_void_chronosphere,
            AbilityId.legion_commander_duel,
            AbilityId.lina_laguna_blade,
            AbilityId.lion_finger_of_death,
            AbilityId.luna_eclipse,
            AbilityId.magnataur_reverse_polarity,
            AbilityId.nyx_assassin_vendetta,
            AbilityId.silencer_global_silence,
            AbilityId.spectre_haunt,
            AbilityId.warlock_rain_of_chaos,
            AbilityId.zuus_thundergods_wrath
        };

        private readonly Team enemyTeam;

        private readonly IInformationPinger informationPinger;

        private readonly Unit myHero;

        private readonly IMenuManager rootMenu;

        private MenuItem<bool> doublePing;

        private MenuItem<bool> enabled;

        private MenuItem<bool> enemyCheck;

        private bool processing;

        private Queue<Ability> queue;

        private MenuItem<bool> rubickDisables;

        private MenuItem<bool> rubickUltimates;

        [ImportingConstructor]
        public AbilityPinger(
            [Import] IServiceContext context,
            [Import] IMenuManager menu,
            [Import] IInformationPinger pinger)
        {
            myHero = context.Owner;
            enemyTeam = myHero.GetEnemyTeam();
            rootMenu = menu;
            informationPinger = pinger;
        }

        public bool IsActive { get; private set; }

        public void Activate()
        {
            CreateMenu();

            queue = new Queue<Ability>();

            if (enabled)
            {
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
                EntityManager<Ability>.EntityAdded += OnEntityAdded;
            }
            enabled.Item.ValueChanged += ItemOnValueChanged;
        }

        public void Dispose()
        {
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
            EntityManager<Ability>.EntityAdded -= OnEntityAdded;
            enabled.Item.ValueChanged -= ItemOnValueChanged;
        }

        private bool AbilityPingCheck()
        {
            if (!enemyCheck || !myHero.IsAlive)
            {
                return true;
            }

            return !EntityManager<Unit>.Entities.Any(
                       x => x.IsValid && x.IsAlive && x.IsRealUnit() && x.Team == enemyTeam
                            && x.Distance2D(myHero) <= 700);
        }

        private void AddPingAbility(Ability ability)
        {
            queue.Enqueue(ability);

            if (processing)
            {
                return;
            }

            processing = true;
            UpdateManager.BeginInvoke(ProceedQueue);
        }

        private void CreateMenu()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            var menu = rootMenu.MenuFactory.Menu("Abilities");
            enabled = menu.Item("Enabled", true);
            enabled.Item.SetTooltip("Ping dangerous enemy ultimates");
            doublePing = menu.Item("Double ping", false);
            doublePing.Item.SetTooltip("Will ping ability 2 times");
            rubickUltimates = menu.Item("Rubick stolen ultimates", true);
            rubickDisables = menu.Item("Rubick stolen disables", false);
            enemyCheck = menu.Item("Check enemies", false);
            enemyCheck.Item.SetTooltip("If there is any enemy hero/creep near you it won't ping");
        }

        private void ItemOnValueChanged(object sender, OnValueChangeEventArgs args)
        {
            if (args.GetNewValue<bool>())
            {
                Entity.OnInt32PropertyChange += OnInt32PropertyChange;
                EntityManager<Ability>.EntityAdded += OnEntityAdded;
            }
            else
            {
                Entity.OnInt32PropertyChange -= OnInt32PropertyChange;
                EntityManager<Ability>.EntityAdded -= OnEntityAdded;
            }
        }

        private void OnEntityAdded(object sender, Ability ability)
        {
            if (!ability.IsValid || ability.Id < AbilityId.antimage_mana_break
                || ability.AbilitySlot != AbilitySlot.Slot_4)
            {
                return;
            }

            var owner = ability.Owner as Hero;
            if (owner == null || !owner.IsValid || owner.IsIllusion || owner.Team != enemyTeam
                || owner.HeroId != HeroId.npc_dota_hero_rubick)
            {
                return;
            }

            if (ability.AbilityType == AbilityType.Ultimate && rubickUltimates
                || (ability.IsDisable() || ability.IsSilence()) && rubickDisables)
            {
                AddPingAbility(ability);
            }
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue <= 0 || args.NewValue > 1 || args.NewValue == args.OldValue
                || args.PropertyName != "m_iLevel")
            {
                return;
            }

            var ability = sender as Ability;
            if (ability == null || !ability.IsValid || ability.Id < AbilityId.antimage_mana_break)
            {
                return;
            }

            var owner = ability.Owner as Hero;
            if (owner == null || !owner.IsValid || owner.IsIllusion || owner.Team != enemyTeam
                || owner.HeroId == HeroId.npc_dota_hero_rubick)
            {
                return;
            }

            if (abilities.Contains(ability.Id))
            {
                AddPingAbility(ability);
            }
        }

        private async void ProceedQueue()
        {
            while (queue.Any())
            {
                while (!AbilityPingCheck())
                {
                    await Task.Delay(300);
                }

                informationPinger.AddPing(new AbilityPing(queue.Dequeue(), doublePing));

                await Task.Delay(1000);
            }

            processing = false;
        }
    }
}