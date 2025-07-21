using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Systems
{
    public class ClearMatchedSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<MatchComponent>> _matchFilter = default;

        public void Run(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var destroyPool = world.GetPool<DestroyComponent>();
            foreach (int entity in _matchFilter.Value)
            {
                if (!destroyPool.Has(entity))
                {
                    destroyPool.Add(entity);
                }
            }
        }
    }
}
