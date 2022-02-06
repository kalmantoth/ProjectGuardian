using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace ProjectGuardian
{
    public enum ORIENTATION
    {
        LEFT,
        UP,
        RIGHT,
        DOWN
    }


    public static class Util
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

        public static float CalculateDistance(Transform from, Transform to)
        {
            return Vector2.Distance(from.transform.position, to.transform.position);
        }

        public static Transform FindClosestTransform(Transform from, List<Transform> toTransforms)
        {
            float max = int.MaxValue;
            Transform closestTransform = null;

            foreach (Transform t in toTransforms)
            {
                float calculatedDistance = CalculateDistance(from, t);
                if (calculatedDistance < max)
                {
                    max = calculatedDistance;
                    closestTransform = t;
                }

            }
            return closestTransform;
        }

        public static List<Transform> GetObjectsOnSceneByComponentType(System.Type classType)
        {
            Transform[] allObjects = UnityEngine.Object.FindObjectsOfType<Transform>();
            List<Transform> returnList = new List<Transform>();
            foreach (Transform transform in allObjects)
            {
                if (transform.transform.GetComponent(classType) != null)
                {
                    returnList.Add(transform);
                }
            }

            return returnList;
        }

        public static Transform FindClosestTransformByComponentType(Transform from, System.Type classType)
        {
            List<Transform> AllTransformWithComponents = GetObjectsOnSceneByComponentType(classType);
            return FindClosestTransform(from, AllTransformWithComponents);

        }
    }

}
