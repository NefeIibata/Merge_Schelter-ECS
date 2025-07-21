using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "Level Config", menuName = "Match-3/Level Configuration")]
    public class LevelConfiguration : ScriptableObject
    {
        [HideInInspector] public int Width = 9;
        [HideInInspector] public int Height = 9;

        [Tooltip("Press calculate to save size")]
        public TileData[] Tiles;

        [Tooltip("Default PieceType used for new tiles")]
        public PieceTypeSO DefaultPieceType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            CalculateSize();
        }

        [ContextMenu("Auto Calculate Width & Height")]
        public void CalculateSize()
        {
            if (Tiles == null || Tiles.Length == 0)
            {
                Width = 0;
                Height = 0;
                return;
            }

            Width = Tiles.Max(t => t.Position.x) + 1;
            Height = Tiles.Max(t => t.Position.y) + 1;

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
