namespace PredictedCreepsLocation.Core
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using Data;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;

    using CreepWave = Creeps.CreepWave;

    [Export(typeof(IWaveManager))]
    internal class WaveManager : IWaveManager
    {
        private readonly LanePaths lanePaths;

        private readonly Team myTeam;

        [ImportingConstructor]
        public WaveManager([Import] IServiceContext context)
        {
            myTeam = context.Owner.Team;
            lanePaths = new LanePaths(myTeam);
        }

        public List<CreepWave> CreepWaves { get; } = new List<CreepWave>();

        public bool IsActive { get; private set; }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;

            Entity.OnBoolPropertyChange += OnBoolPropertyChange;
            Entity.OnInt32PropertyChange += OnInt32PropertyChange;

            EntityManager<Creep>.EntityAdded += OnEntityAdded;
            EntityManager<Creep>.EntityRemoved += OnEntityRemoved;
        }

        public void Dispose()
        {
            IsActive = false;

            Entity.OnBoolPropertyChange -= OnBoolPropertyChange;
            Entity.OnInt32PropertyChange -= OnInt32PropertyChange;

            EntityManager<Creep>.EntityAdded -= OnEntityAdded;
            EntityManager<Creep>.EntityRemoved -= OnEntityRemoved;

            CreepWaves.Clear();
        }

        private void OnBoolPropertyChange(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (args.OldValue == args.NewValue || !args.OldValue && args.NewValue || args.PropertyName != "m_bIsWaitingToSpawn")
            {
                return;
            }

            var creep = sender as Creep;
            if (creep == null || !creep.IsValid || creep.Team != myTeam || creep.UnitType != 1152)
            {
                return;
            }

            SpawnCreepWaves();
        }

        private void OnEntityAdded(object sender, Creep creep)
        {
            if (!creep.IsValid || creep.Team == myTeam || creep.UnitType != 1152)
            {
                return;
            }

            var wave = CreepWaves.FirstOrDefault(x => !x.IsSpawned && x.Creeps.Any(z => z.Distance2D(creep) < 300));

            if (wave != null)
            {
                wave.Creeps.Add(creep);
            }
            else
            {
                var laneData = lanePaths.GetLaneData(creep.Position);
                if (laneData == null)
                {
                    return;
                }

                var newWave = new CreepWave(laneData.Value, creep.Team);
                newWave.Creeps.Add(creep);

                CreepWaves.Add(newWave);
            }
        }

        private void OnEntityRemoved(object sender, Creep creep)
        {
            if (!creep.IsValid || creep.Team == myTeam || creep.UnitType != 1152)
            {
                return;
            }

            var wave = CreepWaves.FirstOrDefault(x => x.Creeps.Contains(creep));
            if (wave == null)
            {
                return;
            }

            wave.Creeps.Remove(creep);

            if (wave.Creeps.Any(x => x.IsValid))
            {
                return;
            }

            CreepWaves.Remove(wave);
        }

        private void OnInt32PropertyChange(Entity sender, Int32PropertyChangeEventArgs args)
        {
            if (args.NewValue > 0 || args.NewValue == args.OldValue || args.PropertyName != "m_iHealth")
            {
                return;
            }

            var creep = sender as Creep;
            if (creep == null)
            {
                return;
            }

            OnEntityRemoved(null, creep);
        }

        private void SpawnCreepWaves()
        {
            foreach (var creepWave in CreepWaves.Where(x => !x.IsSpawned))
            {
                creepWave.Spawn();
            }
        }
    }
}