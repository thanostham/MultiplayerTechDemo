using UnityEngine;
/// <summary>
/// Helper extension methods for Unity's Vector3.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// Returns a new Vector3 with the same x and z as the original, but with the provided y value.
    /// </summary>
    /// <param name="v">Original vector.</param>
    /// <param name="newY">The y value to use in the returned vector.</param>
    /// <returns>A new Vector3 with y replaced by <paramref name="newY"/>.</returns>
    public static Vector3 WithNewZ(this Vector3 v, float newZ)
    {
        return new Vector3(v.x, v.y, newZ);
        
    }
}