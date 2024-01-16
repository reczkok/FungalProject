using System;
using System.Collections.Generic;
using FungiScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TextureAssigner : MonoBehaviour
{
    private Dictionary<string, Tile> strToTile = new Dictionary<string, Tile>();
    private Tile obstacleTile;

    private void Awake()
    {
        LoadAllTiles();
    }

    private void LoadAllTiles()
    {
        obstacleTile = Resources.Load<Tile>("FungiAssets/ObstacleTile");
        var tiles = Resources.LoadAll<Tile>("FungiAssets/FungiPartial/Core/Tiles");
        foreach (var tile in tiles)
        {
            strToTile.Add(tile.name, tile);
        }
        tiles = Resources.LoadAll<Tile>("FungiAssets/FungiPartial/Exploratory/Tiles");
        foreach (var tile in tiles)
        {
            strToTile.Add(tile.name, tile);
        }
    }

    private string ConvertFungiTypeToPath(FungusCell cell)
    {
        var types = cell.GetConnections();
        if (types.Contains(FungiType.RootCell)) return "LRTlTrBlBr";
        var result = "";
        if(types.Contains(FungiType.Left)) result += "L";
        if(types.Contains(FungiType.Right)) result += "R";
        if(types.Contains(FungiType.TopLeft)) result += "Tl";
        if(types.Contains(FungiType.TopRight)) result += "Tr";
        if(types.Contains(FungiType.BottomLeft)) result += "Bl";
        if(types.Contains(FungiType.BottomRight)) result += "Br";
        return result;
    }
    
    public Tile GetObstacleTile()
    {
        return obstacleTile;
    }
    
    public Tile GetTileForCell(FungusCell cell)
    {
        var path = ConvertFungiTypeToPath(cell);
        return cell switch
        {
            CoreCell => strToTile[path],
            ExplorationCell => strToTile["E" + path],
            _ => null
        };
    }
}
