using Components;
using Data;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Systems
{
    public class InputSystem : IEcsRunSystem
    {
        private readonly EcsWorldInject _world = default;
        private readonly EcsCustomInject<GridData> _grid = default;
        private readonly EcsCustomInject<GameConfig> _config = default;
        private readonly EcsFilterInject<Inc<SelectedComponent>> _selectedFilter = default;

        public void Run(IEcsSystems systems)
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            var world = systems.GetWorld();
            var selectedPool = world.GetPool<SelectedComponent>();
            var swapPool = world.GetPool<SwapRequestComponent>();

            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var gridPos = new Vector2Int(
                Mathf.RoundToInt(mousePos.x / _config.Value.CellSize),
                Mathf.RoundToInt(mousePos.y / _config.Value.CellSize)
            );

            if (gridPos.x < 0 || gridPos.x >= _grid.Value.Width ||
                gridPos.y < 0 || gridPos.y >= _grid.Value.Height)
                return;

            if (!_grid.Value.Entities[gridPos.x, gridPos.y].Unpack(_world.Value, out int clickedEntity))
                return;

            if (_selectedFilter.Value.GetEntitiesCount() == 0)
            {
                selectedPool.Add(clickedEntity);
            }
            else
            {
                int selectedEntity = _selectedFilter.Value.GetRawEntities()[0];

                selectedPool.Del(selectedEntity);

                if (selectedEntity != clickedEntity)
                {
                    ref var request = ref swapPool.Has(selectedEntity)
                        ? ref swapPool.Get(selectedEntity)
                        : ref swapPool.Add(selectedEntity);

                    request.Target = _grid.Value.Entities[gridPos.x, gridPos.y];
                }
            }
        }
    }
}
