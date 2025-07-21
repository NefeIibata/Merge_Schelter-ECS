using Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class BonusSpawnSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsFilterInject<Inc<SpawnBonusRequestComponent>> _filter = default;

        public void Run(IEcsSystems systems)
        {
            var requestPool = _filter.Pools.Inc1;
            var piecePool = _world.Value.GetPool<PieceComponent>();

            foreach (var entity in _filter.Value)
            {
                ref var piece = ref piecePool.Get(entity);
                ref var request = ref requestPool.Get(entity);

                piece.Type = request.BonusType;

                if (_world.Value.GetPool<ViewComponent>().Has(entity))
                {
                    ref var view = ref _world.Value.GetPool<ViewComponent>().Get(entity);
                    Object.Destroy(view.Transform.gameObject);
                    var newViewGO = Object.Instantiate(piece.Type.Prefab, view.Transform.position, Quaternion.identity);
                    view.Transform = newViewGO.transform;
                }

                requestPool.Del(entity);
            }
        }
    }
}
