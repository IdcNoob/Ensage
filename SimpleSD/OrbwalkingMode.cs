namespace SimpleSD
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Geometry;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Prediction;
    using Ensage.SDK.Renderer.Particle;
    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;

    using SharpDX;

    internal class OrbwalkingMode : KeyPressOrbwalkingModeAsync
    {
        private readonly IParticleManager particleManager;

        private readonly Poison poison;

        private readonly Settings settings;

        private readonly ITargetSelectorManager targetSelector;

        private Vector3 poisonCastPosition;

        private float poisonStartCastTime;

        private IUpdateHandler poisonUpdateHandler;

        private Unit target;

        private IUpdateHandler targetParticleUpdateHandler;

        public OrbwalkingMode(IServiceContext context, Settings settings)
            : base(context, settings.Key)
        {
            this.settings = settings;
            this.particleManager = this.Context.Particle;
            this.targetSelector = context.TargetSelector;

            this.poison = new Poison(this.Owner.GetAbilityById(AbilityId.shadow_demon_shadow_poison));
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                this.target = this.targetSelector.Active.GetTargets()
                    .FirstOrDefault(x => x.Distance2D(this.Owner) <= this.poison.CastRange);

                if (this.target == null)
                {
                    if (this.settings.Move)
                    {
                        this.Orbwalker.OrbwalkTo(null);
                    }

                    return;
                }

                if (this.poison.CanBeCasted && this.poison.CanHit(this.target))
                {
                    var input = this.poison.GetPredictionInput(this.target);
                    var output = this.poison.GetPredictionOutput(input);

                    if (this.ShouldCastPoison(output))
                    {
                        this.poisonCastPosition = output.UnitPosition;
                        this.poison.UseAbility(this.poisonCastPosition);
                        await Task.Delay(this.poison.GetCastDelay(this.poisonCastPosition), token);
                    }
                }

                if (this.settings.Attack && this.Owner.IsInAttackRange(this.target))
                {
                    this.Orbwalker.OrbwalkTo(this.target);
                }
                else if (this.settings.Move)
                {
                    this.Orbwalker.OrbwalkTo(null);
                }
            }
            catch
            {
                // yolo
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Context.Inventory.Attach(this);
            this.MenuKey.PropertyChanged += this.MenuKeyOnPropertyChanged;
            this.poisonUpdateHandler = UpdateManager.Subscribe(this.PoisonHitCheck, 0, false);
            this.targetParticleUpdateHandler = UpdateManager.Subscribe(this.UpdateTargetParticle, 0, false);
            this.settings.CancelAnimation.PropertyChanged += this.CancelAnimationOnPropertyChanged;
            this.CancelAnimationOnPropertyChanged(null, null);
        }

        protected override void OnDeactivate()
        {
            this.Context.Inventory.Detach(this);
            this.MenuKey.PropertyChanged -= this.MenuKeyOnPropertyChanged;
            this.settings.CancelAnimation.PropertyChanged -= this.CancelAnimationOnPropertyChanged;
            Entity.OnBoolPropertyChange -= this.EntityOnBoolPropertyChange;
            UpdateManager.Unsubscribe(this.poisonUpdateHandler);
            UpdateManager.Unsubscribe(this.targetParticleUpdateHandler);
            base.OnDeactivate();
        }

        private void CancelAnimationOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.settings.CancelAnimation)
            {
                Entity.OnBoolPropertyChange += this.EntityOnBoolPropertyChange;
            }
            else
            {
                Entity.OnBoolPropertyChange -= this.EntityOnBoolPropertyChange;
            }
        }

        private void EntityOnBoolPropertyChange(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (!this.CanExecute)
            {
                return;
            }

            if (sender != this.poison || args.NewValue == args.OldValue || args.PropertyName != "m_bInAbilityPhase")
            {
                return;
            }

            if (args.NewValue)
            {
                this.poisonStartCastTime = Game.RawGameTime;
                this.poisonUpdateHandler.IsEnabled = true;
            }
            else
            {
                this.poisonUpdateHandler.IsEnabled = false;
            }
        }

        private void MenuKeyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.MenuKey)
            {
                if (this.settings.DrawTargetParticle)
                {
                    this.targetParticleUpdateHandler.IsEnabled = true;
                }
            }
            else
            {
                if (this.settings.DrawTargetParticle)
                {
                    this.particleManager.Remove("poisonTarget");
                    this.targetParticleUpdateHandler.IsEnabled = false;
                }
            }
        }

        private void PoisonHitCheck()
        {
            if (this.target == null || !this.target.IsValid || !this.target.IsVisible)
            {
                return;
            }

            var input = this.poison.GetPredictionInput(this.target);
            input.Delay = Math.Max((this.poisonStartCastTime - Game.RawGameTime) + this.poison.CastPoint, 0);
            var output = this.poison.GetPredictionOutput(input);

            if (this.poisonCastPosition.Distance2D(output.UnitPosition) > this.poison.Radius)
            {
                this.Owner.Stop();
                this.Cancel();
                this.poisonUpdateHandler.IsEnabled = false;
            }
        }

        private bool ShouldCastPoison(PredictionOutput output)
        {
            if (output.HitChance == HitChance.OutOfRange || output.HitChance == HitChance.Impossible)
            {
                return false;
            }

            if (output.HitChance == HitChance.Collision)
            {
                return false;
            }

            return true;
        }

        private void UpdateTargetParticle()
        {
            if (this.target == null || !this.target.IsValid || !this.target.IsVisible)
            {
                this.particleManager.Remove("poisonTarget");
                return;
            }

            this.particleManager.DrawTargetLine(this.Owner, "poisonTarget", this.target.Position, Color.Blue);
        }
    }
}