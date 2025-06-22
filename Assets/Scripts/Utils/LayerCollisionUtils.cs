using UnityEngine;

/// <summary>
/// Utility class for retrieving LayerMasks that simulate Unity’s collision matrix behavior.
/// Useful for making raycasts or physics queries behave as if they originate from a specific layer,
/// respecting which layers it would or wouldn’t collide with.
/// </summary>
public static class LayerCollisionUtils
{
    public static LayerMask GetCollisionMaskForLayer(LayerMask sourceLayerMask)
    {
        if (sourceLayerMask.value == 0)
        {
            Debug.LogWarning("Source LayerMask is empty.");
            return 0;
        }

        // Obtain index
        int sourceLayerIndex = Mathf.RoundToInt(Mathf.Log(sourceLayerMask.value, 2));

        LayerMask mask = 0;

        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(sourceLayerIndex, i))
            {
                mask |= (1 << i);
            }
        }

        return mask;
    }
}