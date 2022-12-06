using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrueAxion.FFAMinesweepers.Utilities
{
    public static class TransformUtilities
    {
        private const float destinationThreshold = 1;

        public static void MoveTransformToDestination(Transform targetTrans, Transform destinationTrans, float timeToTarget)
        {
            if (timeToTarget <= 0)
            {
                targetTrans.position = destinationTrans.position;
            }
            else if (!IsReachDestination(targetTrans, destinationTrans))
            {
                var t = Time.deltaTime / timeToTarget;
                t = t > 0 ? t : 0;
                targetTrans.position = Vector3.Lerp(targetTrans.position, destinationTrans.position, t);
            }
            else
            {
                targetTrans.position = destinationTrans.position;
            }
        }

        public static void MoveTransformToDestination(Transform targetTrans, Vector3 destination, float timeToTarget)
        {
            if (timeToTarget <= 0)
            {
                targetTrans.position = destination;
            }
            else if (!IsReachDestination(targetTrans, destination))
            {
                var t = Time.deltaTime / timeToTarget;
                t = t > 0 ? t : 0;
                targetTrans.position = Vector3.Lerp(targetTrans.position, destination, t);
            }
            else
            {
                targetTrans.position = destination;
            }
        }

        public static bool IsReachDestination(Transform targetTrans, Transform destinationTrans)
        {
            return (targetTrans.position - destinationTrans.position).sqrMagnitude <= destinationThreshold;
        }

        public static bool IsReachDestination(Transform targetTrans, Vector3 destination)
        {
            return (targetTrans.position - destination).sqrMagnitude <= destinationThreshold;
        }

        public static bool IsAllReachDestinations(Transform[] targetTrans, Transform[] destinationTrans)
        {
            for (int i = 0; i < targetTrans.Length; i++)
            {
                if (!IsReachDestination(targetTrans[i], destinationTrans[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsAllReachDestinations(Transform[] targetTrans, Vector3[] destination)
        {
            for (int i = 0; i < targetTrans.Length; i++)
            {
                if (!IsReachDestination(targetTrans[i], destination[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}