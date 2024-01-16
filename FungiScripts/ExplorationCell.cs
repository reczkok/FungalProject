using System;
using System.Collections;
using System.Collections.Generic;
using FungiScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class ExplorationCell : FungusCell
{
    public ExplorationCell(int x, int y, FungiType? initialType = null) : base(x, y, initialType) { }
    
    public ExplorationCell(int x, int y, float initResource, FungiType? initialType = null) : base(x, y, initResource, initialType) { }

    public override float Share()
    {
        var share = resourceAmount;
        resourceAmount -= share;
        return share;
    }
    
    public override Tuple<ExpansionDirection, FungusCell> DecideDevelopement(Tilemap enviorement)
    {
        if (isLandlocked) return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);
        
        var resourceDir = SeesResource(enviorement);
        if (resourceDir != ExpansionDirection.None)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, resourceDir);
            if (!enviorement.HasTile(dir))
            {
                var childResourceDeploy = resourceAmount;
                resourceAmount -= childResourceDeploy;
                return new Tuple<ExpansionDirection, FungusCell>(resourceDir, new CoreCell(dir.x, dir.y, childResourceDeploy));
            }
        }
        
        bool hasLeft = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left));
        bool hasRight = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right));
        bool hasTopRight = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight));
        bool hasTopLeft = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft));
        bool hasBottomRight = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight));
        bool hasBottomLeft = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft));

        int neighbourCount = 0;
        float deactivationProb = 0f;
        float leftProb = 0f;
        float rightProb = 0f;
        float topRightProb = 0f;
        float topLeftProb = 0f;
        float bottomRightProb = 0f;
        float bottomLeftProb = 0f;

        if (hasLeft)
        {
            rightProb += FungiParameters.explorationDirectOppositeChance;
            topRightProb += FungiParameters.explorationBroadOppositeChance;
            bottomRightProb += FungiParameters.explorationBroadOppositeChance;
            neighbourCount++;
        }
        if (hasRight)
        {
            leftProb += FungiParameters.explorationDirectOppositeChance;
            topLeftProb += FungiParameters.explorationBroadOppositeChance;
            bottomLeftProb += FungiParameters.explorationBroadOppositeChance;
            neighbourCount++;
        }
        if (hasTopRight)
        {
            bottomLeftProb += FungiParameters.explorationDirectOppositeChance;
            leftProb += FungiParameters.explorationBroadOppositeChance;
            bottomRightProb += FungiParameters.explorationBroadOppositeChance;
            neighbourCount++;
        }
        if (hasTopLeft)
        {
            bottomRightProb += FungiParameters.explorationDirectOppositeChance;
            rightProb += FungiParameters.explorationBroadOppositeChance;
            bottomLeftProb += FungiParameters.explorationBroadOppositeChance;
            neighbourCount++;
        }
        if (hasBottomRight)
        {
            topLeftProb += FungiParameters.explorationDirectOppositeChance;
            leftProb += FungiParameters.explorationBroadOppositeChance;
            topRightProb += FungiParameters.explorationBroadOppositeChance;
            neighbourCount++;
        }
        if (hasBottomLeft)
        {
            topRightProb += FungiParameters.explorationDirectOppositeChance;
            rightProb += FungiParameters.explorationBroadOppositeChance;
            topLeftProb += FungiParameters.explorationBroadOppositeChance;
            neighbourCount++;
        }
        
        if (neighbourCount > 3) isLandlocked = true;
        deactivationProb = neighbourCount * 0.1f;
        if (deactivationProb > FungiParameters.explorationMaxNeighborBeforeStarvation) deactivationProb = 0.95f;
        if (deactivationProb <= FungiParameters.explorationSafeNeighbor) deactivationProb = 0.01f;
        if (Random.Range(0f, 1f) < deactivationProb) return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);

        if (hasRight)
        {
            rightProb = 0f;
        }
        if (hasLeft)
        {
            leftProb = 0f;
        }
        if (hasTopRight)
        {
            topRightProb = 0f;
        }
        if (hasTopLeft)
        {
            topLeftProb = 0f;
        }
        if (hasBottomRight)
        {
            bottomRightProb = 0f;
        }
        if (hasBottomLeft)
        {
            bottomLeftProb = 0f;
        }

        float total = leftProb + rightProb + topRightProb + topLeftProb + bottomRightProb + bottomLeftProb;

        if (total < 0.1f)
        {
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);
        }
        
        var childResource = resourceAmount;
        resourceAmount -= childResource;
        
        float random = Random.Range(0f, total);

        if (random < leftProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Left, new ExplorationCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Right, new ExplorationCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopRight, new ExplorationCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb + topLeftProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopLeft, new ExplorationCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb + topLeftProb + bottomRightProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomRight, new ExplorationCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb + topLeftProb + bottomRightProb + bottomLeftProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomLeft, new ExplorationCell(dir.x, dir.y, childResource));
        }
        return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);
    }
}
