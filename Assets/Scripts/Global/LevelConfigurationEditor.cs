using Data;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Global
{
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelConfiguration))]
    public class LevelConfigurationEditor : Editor
    {
        private LevelConfiguration _config;
        private const int CellSize = 24;

        private void OnEnable()
        {
            _config = (LevelConfiguration)target;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Auto Calculate Width & Height"))
            {
                _config.CalculateSize();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Row"))
            {
                AddRow();
            }
            if (GUILayout.Button("Remove Row"))
            {
                RemoveRow();
            }
            if (GUILayout.Button("Add Column"))
            {
                AddColumn();
            }
            if (GUILayout.Button("Remove Column"))
            {
                RemoveColumn();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            DrawGridEditor();

            EditorGUILayout.Space();
            //EditorGUILayout.LabelField("Other Properties", EditorStyles.boldLabel);
            DrawDefaultInspector();
        }

        private void DrawGridEditor()
        {
            if (_config.Tiles == null || _config.Tiles.Length == 0)
                return;

            int width = _config.Width;
            int height = _config.Height;

            GUILayout.Label("Grid Layout", EditorStyles.boldLabel);

            for (int y = height - 1; y >= 0; y--)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < width; x++)
                {
                    var tile = GetTileAtPosition(new Vector2Int(x, y));
                    if (tile != null)
                    {
                        GUI.color = GetColorForTile(tile.Type);
                        if (GUILayout.Button(tile.Type.ToString().Substring(0, 1), GUILayout.Width(CellSize), GUILayout.Height(CellSize)))
                        {
                            CycleTileType(tile);
                        }
                    }
                    else
                    {
                        GUI.color = Color.gray;
                        GUILayout.Box("", GUILayout.Width(CellSize), GUILayout.Height(CellSize));
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUI.color = Color.white;

            //EditorGUILayout.Space();
            //EditorGUILayout.LabelField("Tiles Details", EditorStyles.boldLabel);

            //for (int i = 0; i < _config.Tiles.Length; i++)
            //{
            //    var tile = _config.Tiles[i];
            //    EditorGUILayout.BeginVertical("box");
            //    EditorGUILayout.LabelField($"Tile {i}", EditorStyles.boldLabel);
            //    tile.Position = EditorGUILayout.Vector2IntField("Position", tile.Position);
            //    tile.Type = (TileType)EditorGUILayout.EnumPopup("Type", tile.Type);
            //    tile.InitialPiece = (PieceTypeSO)EditorGUILayout.ObjectField("Initial Piece", tile.InitialPiece, typeof(PieceTypeSO), false);
            //    tile.GravityDirection = EditorGUILayout.Vector2IntField("Gravity Direction", tile.GravityDirection);
            //    tile.Durability = EditorGUILayout.IntField("Durability", tile.Durability);
            //    EditorGUILayout.EndVertical();
            //}
        }

        private void AddRow()
        {
            var newTiles = _config.Tiles.ToList();
            int y = _config.Height;
            for (int x = 0; x < _config.Width; x++)
            {
                newTiles.Add(CreateDefaultTile(x, y));
            }

            _config.Tiles = newTiles.ToArray();
            _config.Height++;
            EditorUtility.SetDirty(_config);
        }

        private void RemoveRow()
        {
            int y = _config.Height - 1;
            _config.Tiles = _config.Tiles.Where(t => t.Position.y < y).ToArray();
            _config.Height = Mathf.Max(0, _config.Height - 1);
            EditorUtility.SetDirty(_config);
        }

        private void AddColumn()
        {
            var newTiles = _config.Tiles.ToList();
            int x = _config.Width;
            for (int y = 0; y < _config.Height; y++)
            {
                newTiles.Add(CreateDefaultTile(x, y));
            }

            _config.Tiles = newTiles.ToArray();
            _config.Width++;
            EditorUtility.SetDirty(_config);
        }

        private void RemoveColumn()
        {
            int x = _config.Width - 1;
            _config.Tiles = _config.Tiles.Where(t => t.Position.x < x).ToArray();
            _config.Width = Mathf.Max(0, _config.Width - 1);
            EditorUtility.SetDirty(_config);
        }

        private TileData CreateDefaultTile(int x, int y)
        {
            return new TileData
            {
                Position = new Vector2Int(x, y),
                Type = TileType.Available,
                GravityDirection = new Vector2Int(0, -1),
                Durability = 0,
                InitialPiece = _config.DefaultPieceType
            };
        }

        private TileData GetTileAtPosition(Vector2Int pos)
        {
            return _config.Tiles.FirstOrDefault(t => t.Position == pos);
        }

        private void CycleTileType(TileData tile)
        {
            tile.Type = tile.Type switch
            {
                TileType.Empty => TileType.Available,
                TileType.Available => TileType.Blocker,
                TileType.Blocker => TileType.Empty,
                _ => TileType.Empty,
            };

            EditorUtility.SetDirty(_config);
        }

        private Color GetColorForTile(TileType type)
        {
            return type switch
            {
                TileType.Empty => Color.blue,
                TileType.Available => Color.green,
                TileType.Blocker => Color.red,
                _ => Color.white
            };
        }
    }
#endif
}
