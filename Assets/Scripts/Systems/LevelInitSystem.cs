using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class LevelInitSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<LevelConfiguration> _levelConfig = default;
        private readonly EcsCustomInject<GameConfig> _gameConfig = default;
        private readonly EcsCustomInject<GridData> _gridData = default;

        public void Init(IEcsSystems systems)
        {
            var grid = _gridData.Value;

            var pieceParent = new GameObject("GridPices");

            foreach (var tileData in _levelConfig.Value.Tiles)
            {
                int entity = _world.Value.NewEntity();

                EcsPackedEntity packedEntity = _world.Value.PackEntity(entity);
                grid.Entities[tileData.Position.x, tileData.Position.y] = packedEntity;

                ref var pos = ref _world.Value.GetPool<PositionComponent>().Add(entity);
                pos.Value = tileData.Position;

                GameObject pieceGO = null;
                var pieceType = tileData.InitialPiece;

                ref var piece = ref _world.Value.GetPool<PieceComponent>().Add(entity);

                if (tileData.Type == TileType.Available)
                {
                    ref var gravity = ref _world.Value.GetPool<GravityDirectionComponent>().Add(entity);
                    gravity.Direction = tileData.GravityDirection;


                    if (pieceType.TypeName == "Random")
                    {
                        pieceType = _gameConfig.Value.GetRandomPiece();
                    }

                    piece.Type = pieceType;

                }
                else if (tileData.Type == TileType.Blocker)
                {
                    _world.Value.GetPool<BlockerComponent>().Add(entity);

                    ref var durability = ref _world.Value.GetPool<DurabilityComponent>().Add(entity);
                    durability.Health = tileData.Durability;

                    ref var blockerProps = ref _world.Value.GetPool<BlockerPropertiesComponent>().Add(entity);

                    piece.Type = pieceType;

                }
                else if (tileData.Type == TileType.Empty)
                {
                    _world.Value.GetPool<EmptyMarkerComponent>().Add(entity);
                    piece.Type = pieceType;
                }

                pieceGO = Object.Instantiate(piece.Type.Prefab, pieceParent.transform);


                if (pieceGO != null)
                {
                    pieceGO.transform.position = new Vector3(pos.Value.x * _gameConfig.Value.CellSize, pos.Value.y * _gameConfig.Value.CellSize, 0);
                    ref var view = ref _world.Value.GetPool<ViewComponent>().Add(entity);
                    view.Transform = pieceGO.transform;
                }
            }
        }
    }
}
