using UnityEngine;

/// <summary>
/// Utility class for retrieving LayerMasks that simulate Unity’s collision matrix behavior.
/// Useful for making raycasts or physics queries behave as if they originate from a specific layer,
/// respecting which layers it would or wouldn’t collide with.
/// </summary>
public static class LayerCollisionUtils
{
    public static LayerMask GetCollisionMaskForLayer(int sourceLayer)
    {
        LayerMask mask = 0;

        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(sourceLayer, i))
            {
                mask |= (1 << i);
            }
        }

        return mask;
    }

    public static LayerMask GetCollisionMaskForLayer(string sourceLayerName)
    {
        int layer = LayerMask.NameToLayer(sourceLayerName);
        if (layer == -1)
        {
            Debug.LogWarning($"Layer '{sourceLayerName}' no existe.");
            return 0;
        }

        return GetCollisionMaskForLayer(layer);
    }
}