using UnityEngine;
using Data;

namespace Global
{
#if UNITY_EDITOR
    [ExecuteAlways]
    public class LevelDebugDrawer : MonoBehaviour
    {
        public LevelConfiguration levelConfig;

        private void OnDrawGizmos()
        {
            if (levelConfig == null || levelConfig.Tiles == null)
                return;


            foreach (var tile in levelConfig.Tiles)
            {
                Vector3 pos = new Vector3(tile.Position.x, tile.Position.y, 0);
                switch (tile.Type)
                {
                    case TileType.Empty:
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
                        break;
                    case TileType.Available:
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
                        break;
                    case TileType.Blocker:
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
                        break;
                    default:
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(pos, Vector3.one * 0.9f);
                        break;
                }



#if UNITY_EDITOR
                UnityEditor.Handles.Label(TextPosition(pos, -0.45f, 0.7f), tile.InitialPiece.TypeName.ToString());
                UnityEditor.Handles.Label(TextPosition(pos, -0.45f, 0.5f), tile.Type.ToString());
#endif
            }
        }

        private static Vector3 TextPosition(Vector3 pos, float offsetX, float offsetY)
        {
            var position = new Vector3(pos.x + offsetX, pos.y + offsetY, pos.x);
            return position;
        }
    }
#endif
}
