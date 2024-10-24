using System;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace JioXSDK.FOVSample
{
    [Serializable]
    public record ReportData
    {
        public Handedness Hand;
        public HandTrackingState TrackingState;
        public VS3 HandPosition;
        public VS3 EulerAnglesToHandAbs;
        public VS3 EulerAnglesToHandRelative;

        public string ToCSVString()
        {
            return $"{Hand};{TrackingState};{HandPosition};{EulerAnglesToHandAbs};{EulerAnglesToHandRelative}";
        }

        public static string ToCSVHeader()
        {
            return $"{nameof(Hand)};{nameof(TrackingState)};{nameof(HandPosition)};{nameof(EulerAnglesToHandAbs)};{nameof(EulerAnglesToHandRelative)}";
        }
    }

    [Serializable]
    public class ReportJson
    {
        public ReportData[] dataArray;

        public string GetJsonString()
        {
            return JsonUtility.ToJson(this);
        }
    }

    [Serializable]
    public struct VS3
    {
        public float x;
        public float y;
        public float z;

        public static VS3 FromVector3(Vector3 v)
        {
            return new VS3
            {
                x = v.x,
                y = v.y,
                z = v.z,
            };
        }

        public static VS3 FromVector2(Vector2 v)
        {
            return new VS3
            {
                x = v.x,
                y = v.y,
                z = 0,
            };
        }

        public override readonly string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public readonly Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}