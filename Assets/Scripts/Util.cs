using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ORIENTATION
{
    LEFT,
    UP,
    RIGHT,
    DOWN
}

public class Util
{
    public static float AnimationLength(string name, Animator animator)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                time = ac.animationClips[i].length;

        return time;
    }

    public static float CalculateDistance(GameObject from, GameObject to)
    {
        return Vector3.Distance(from.transform.position, to.transform.position);
    }

    public static GameObject FindClosestGameobject(GameObject from, List<GameObject> toGameObjects)
    {
        float max = int.MaxValue;
        GameObject closestGO = null;

        foreach (GameObject GO in toGameObjects)
        {
            float calculatedDistance = CalculateDistance(from, GO);
            if (calculatedDistance < max)
            {
                max = calculatedDistance;
                closestGO = GO;
            }

        }
        return closestGO;
    }
}
