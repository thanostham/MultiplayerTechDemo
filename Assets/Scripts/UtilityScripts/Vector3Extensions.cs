using UnityEngine;
///<summary>
///Helper extension for Vector3.
///</summary>
public static class Vector3Extensions
{
    ///<summary>
    ///Returns a new Vector3 with the same x and y as the original, but with the provided z value.
    ///</summary>
    ///<param name="v">Original vector.</param>
    ///<param name="newY">The z value to use in the returned vector.</param>
    ///<returns>A new Vector3 with z replaced by <paramref name="newZ"/>.</returns>
    public static Vector3 WithNewZ(this Vector3 v, float newZ)
    {
        return new Vector3(v.x, v.y, newZ);
        
    }
}