using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems
{
    public class ClearMatchedSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<MatchComponent>> _matchFilter = default;

        public void Run(IEcsSystems systems)
        {
            var destroyPool = _world.Value.GetPool<DestroyComponent>();
            var bonusPool = _world.Value.GetPool<BonusComponent>();
            var bonusRequestPool = _world.Value.GetPool<SpawnBonusRequestComponent>();

            foreach (int entity in _matchFilter.Value)
            {
                if (bonusPool.Has(entity) || bonusRequestPool.Has(entity))
                    continue;

                if (!destroyPool.Has(entity))
                    destroyPool.Add(entity);
            }
        }
    }
}
