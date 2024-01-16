using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FungiScripts;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

public class FungusNetwork
{
    private Tilemap enviorement;
    private FungusCell rootCell;
    private ResourceNetwork resourceNetwork;
    private HashSet<FungusCell> network = new HashSet<FungusCell>();
    private HashSet<FungusCell> activeCells = new HashSet<FungusCell>();
    private HashSet<FungusCell> dyingCells = new HashSet<FungusCell>();
    public FungusNetwork(Tilemap enviorement, ResourceNetwork resourceNetwork)
    {
        this.enviorement = enviorement;
        this.resourceNetwork = resourceNetwork;
        rootCell = new CoreCell(0, 0);
        network.Add(rootCell);
        activeCells.Add(rootCell);
    }

    public HashSet<FungusCell> GetAllCells()
    {
        return network;
    }
    
    public HashSet<FungusCell> GetActiveCells()
    {
        return activeCells;
    }
    
    public HashSet<FungusCell> GetDyingCells()
    {
        return dyingCells;
    }

    public void SetEnviroment(Tilemap t)
    {
        enviorement = t;
    }
    
    private void DeactivateCell(FungusCell cell)
    {
        cell.Deactivate();
        activeCells.Remove(cell);
        dyingCells.Add(cell);
    }
    
    private void ActivateCell(FungusCell cell)
    {
        cell.Activate();
        network.Add(cell);
        activeCells.Add(cell);
        if (dyingCells.Contains(cell)) dyingCells.Remove(cell);
    }

    private void ExploreStep()
    {
        if (activeCells.Count == 0)
        {
            FeedStep();
            return;
        }
        DevelopCells(true);
    }
    
    private void GrowStep()
    {
        if (activeCells.Count == 0)
        {
            FeedStep();
            return;
        }
        DevelopCells(false);
    }

    private void DevelopCells(bool exploratory)
    {
        List<FungusCell> toDeactivate = new List<FungusCell>();
        List<FungusCell> toAdd = new List<FungusCell>();
        foreach (var cell in activeCells)
        {
            if (exploratory && cell is CoreCell) continue;
            if (!exploratory && cell is ExplorationCell) continue;
            var developenetInfo = cell.DecideDevelopement(enviorement);
            if (developenetInfo.Item1 == ExpansionDirection.None)
            {
                toDeactivate.Add(cell);
                continue;
            }
            var parentConnection = Helpers.ExpansionDirectionToFungiType(developenetInfo.Item1);
            var childConnection = Helpers.ExpansionDirectionToFungiType(Helpers.GetOppositeDirection(developenetInfo.Item1));
            var childCoords = DetermineChildPosition(cell, developenetInfo.Item1);
            var sameCell = toAdd.Find(c => c.GetCoordsAsVector() == childCoords);
            if (sameCell != null)
            {
                sameCell.AddConnection(childConnection);
                sameCell.AddResource(developenetInfo.Item2.GetResourceAmount());
                cell.AddConnection(parentConnection);
                continue;
            }
            developenetInfo.Item2.AddConnection(childConnection);
            cell.AddConnection(parentConnection);
            toAdd.Add(developenetInfo.Item2);
        }

        foreach (var cell in toDeactivate)
        {
            DeactivateCell(cell);
        }

        foreach (var cell in toAdd)
        {
            ActivateCell(cell);
        }
    }
    
    private void FeedStep()
    {
        var starvingCells = new List<FungusCell>();
        var depletedResources = new List<ResourceCell>();

        
        var toRevive = new List<FungusCell>();
        foreach (var cell in network)
        {
            // TODO: EXTREMELY taxing when there is a large food source
            // TODO: Find a better way to check for surrounding resources
            var dir = cell.Scout(enviorement);
            if (dir != ExpansionDirection.None)
            {
                var resourceCoords = DetermineChildPosition(cell, dir);
                resourceCoords.z = 1;
                var resourceCell = resourceNetwork.GetCellAt(resourceCoords);
                var depleted = cell.HarvestResource(resourceCell);
                if (depleted) depletedResources.Add(resourceCell);
            }

            var survived = cell.Consume();
            if (!survived) starvingCells.Add(cell);

            if (cell is ExplorationCell)
            {
                // TODO: Find why this seems not to work sometimes
                if (cell.GetConnections().Count == 1 && cell.GetResourceAmount() > 0.5f && !cell.IsLandlocked())
                {
                    toRevive.Add(cell);
                }
            }
            if (cell.GetResourceAmount() > 5f) toRevive.Add(cell);
        }
        
        foreach (var cell in starvingCells)
        {
            DeactivateCell(cell);
        }
        
        foreach (var cell in depletedResources)
        {
            resourceNetwork.RegisterCellDepletion(cell);
        }
        
        foreach (var cell in toRevive) ActivateCell(cell);
        
        foreach (var cell in activeCells)
        {
            // TODO: Make this prioritise exploratory cells if available
            // TODO: Make exploratory cells instantly pass on resources to the opposite neighbour if available
            if(cell is ExplorationCell && cell.GetConnections().Count == 1 && !cell.IsLandlocked()) continue;
            var cellNeighbours = cell.GetConnections();
            var shareAmount = cell.Share();
            if (shareAmount == 0) continue;
            var sharePerNeighbour = shareAmount / cellNeighbours.Count;
            var neighbours = new List<FungusCell>();
            foreach (var neighbour in cellNeighbours)
            {
                var dir = Helpers.FungiTypeToExpansionDirection(neighbour);
                var neighbourCoords = DetermineChildPosition(cell, dir);
                var neighbourCell = network.FirstOrDefault(c => c.GetCoordsAsVector() == neighbourCoords);
                neighbours.Add(neighbourCell);
            }

            if (cell is CoreCell)
            {
                var exploratoryNeighbours = neighbours.Where(n => n is ExplorationCell).ToList();
                if (exploratoryNeighbours.Count > 0)
                {
                    var exploratoryShare = shareAmount / exploratoryNeighbours.Count;
                    foreach (var neighbour in exploratoryNeighbours)
                    {
                        neighbour.AddResource(exploratoryShare);
                    }
                    continue;
                }
                foreach (var neighbour in neighbours)
                {
                    neighbour.AddResource(sharePerNeighbour);
                }
            }
            else
            {
                foreach (var neighbour in neighbours)
                {
                    neighbour.AddResource(sharePerNeighbour);
                }
            }
            
        }
    }
    
    public void FungiStep(int currentStep)
    {
        switch (currentStep % 5)
        {
            case 0:
                GrowStep();
                break;
            case 1:
                FeedStep();
                break;
            default:
                ExploreStep();
                break;
        }
    }
    
    private FungusCell MergeCells(List<FungusCell> cells)
    {
        var mergedCell = cells[0];
        for (int i = 1; i < cells.Count; i++)
        {
            var cell = cells[i];
            foreach (var connection in cell.GetConnections())
            {
                mergedCell.AddConnection(connection);
            }
            network.Remove(cell);
        }
        return mergedCell;
    }

    
    public static Vector3Int DetermineChildPosition(FungusCell parentCell, ExpansionDirection direction)
    {
        var parentPos = parentCell.GetCoordsAsVector();
        if (direction == ExpansionDirection.TopLeft)
        {
            if (Math.Abs(parentPos.y) % 2 == 1) return parentPos + ExpansionDirToVec(direction);
            var additionalOffset = new Vector3Int(-1, 0);
            return parentPos + additionalOffset + ExpansionDirToVec(direction);
        }
        if (direction == ExpansionDirection.TopRight)
        {
            if (Math.Abs(parentPos.y) % 2 == 0) return parentPos + ExpansionDirToVec(direction);
            var additionalOffset = new Vector3Int(1, 0);
            return parentPos + additionalOffset + ExpansionDirToVec(direction);
        }
        if (direction == ExpansionDirection.BottomLeft)
        {
            if (Math.Abs(parentPos.y) % 2 == 1) return parentPos + ExpansionDirToVec(direction);
            var additionalOffset = new Vector3Int(-1, 0);
            return parentPos + additionalOffset + ExpansionDirToVec(direction);
        }
        if (direction == ExpansionDirection.BottomRight)
        {
            if (Math.Abs(parentPos.y) % 2 == 0) return parentPos + ExpansionDirToVec(direction);
            var additionalOffset = new Vector3Int(1, 0);
            return parentPos + additionalOffset + ExpansionDirToVec(direction);
        }
        return parentPos + ExpansionDirToVec(direction);
    }

    public static Vector3Int ExpansionDirToVec(ExpansionDirection ed)
    {
        return ed switch
        {
            ExpansionDirection.Right => new Vector3Int(1, 0),
            ExpansionDirection.Left => new Vector3Int(-1, 0),
            ExpansionDirection.TopRight => new Vector3Int(0, 1),
            ExpansionDirection.BottomLeft => new Vector3Int(0, -1),
            ExpansionDirection.TopLeft => new Vector3Int(0, 1),
            ExpansionDirection.BottomRight => new Vector3Int(0, -1),
            _ => new Vector3Int(0, 0)
        };
    }

    public FungusCell GetCellAt(Vector3Int cellPos)
    {
        return network.FirstOrDefault(cell => cell.GetCoordsAsVector() == cellPos);
    }
}
