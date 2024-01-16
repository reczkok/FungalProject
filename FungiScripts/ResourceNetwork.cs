using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FungiScripts;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ResourceNetwork
{
    private Tilemap enviorement;
    private List<ResourceCell> _resourceCells = new List<ResourceCell>();
    private List<ResourceCell> _activeCells = new List<ResourceCell>();
    
    public ResourceNetwork(Tilemap enviorement)
    {
        this.enviorement = enviorement;
        GenerateResourceCells();
    }
    
    public List<ResourceCell> GetAllCells()
    {
        return _resourceCells;
    }
    
    public List<ResourceCell> GetActiveCells()
    {
        return _activeCells;
    }
    
    public void SetEnviroment(Tilemap t)
    {
        enviorement = t;
    }
    
    public void RegisterCellDepletion(ResourceCell cell)
    {
        cell.Deactivate();
        _activeCells.Remove(cell);
        enviorement.SetTile(cell.GetCoordsAsVector(), null);
    }
    
    public ResourceCell GetCellAt(Vector3Int pos)
    {
        return _resourceCells.FirstOrDefault(cell => cell.GetCoordsAsVector() == pos);
    }

    private void GenerateResourceCells()
    {
        for (int i = 0; i < ResourceParameters.concentrations; i++)
        {
            var x = Random.Range(-ResourceParameters.xMax, ResourceParameters.xMax);
            var y = Random.Range(-ResourceParameters.yMax, ResourceParameters.yMax);

            var radius = Random.Range(1, ResourceParameters.maxRadius);
            for (int j = x - radius; j < x + radius; j++)
            {
                for (int k = y - radius; k < y + radius; k++)
                {
                        float resourceAmount = Random.Range(0, ResourceParameters.maxResourceAmount);
                        var cell = new ResourceCell(j, k, resourceAmount);
                        _resourceCells.Add(cell);
                }
            }
        }
        _activeCells = _resourceCells;
    }
}
