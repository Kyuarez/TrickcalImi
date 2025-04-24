using UnityEngine;

public static class UnityHelper
{
    public static Transform FindRecursiveChild(this Transform root, string name)
    {
        foreach (Transform trans in root.transform)
        {
            if (trans.name == name)
            {
                return trans;
            }

            var ret = FindRecursiveChild(trans, name);
            if (ret != null)
            {
                return ret;
            }
        }

        return null;
    }

    public static bool IsFront(this Transform owner, Transform target)
    {
        Vector3 toTarget = target.position - owner.position;
        return Vector3.Dot(owner.forward, toTarget.normalized) > 0f;
    }
}
