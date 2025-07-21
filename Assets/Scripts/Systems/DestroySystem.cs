using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class DestroySystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<DestroyComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            var viewPool = _world.Value.GetPool<ViewComponent>();

            foreach (int entity in _filter.Value)
            {
                if (viewPool.Has(entity))
                {
                    Object.Destroy(viewPool.Get(entity).Transform.gameObject);
                }
                _world.Value.DelEntity(entity);
            }
        }
    }
}
