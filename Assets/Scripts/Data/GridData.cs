using Leopotam.EcsLite;

namespace Data
{
    public class GridData
    {
        public EcsPackedEntity[,] Entities;
        public int Width;
        public int Height;

        public GridData(int width, int height)
        {
            Width = width;
            Height = height;
            Entities = new EcsPackedEntity[width, height];
        }
    }
}
