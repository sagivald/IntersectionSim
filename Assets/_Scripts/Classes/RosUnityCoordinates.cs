using UnityEngine;
public static class RosUnityCoordinates
{
    public static Vector3 RosToUnityPositionAxisConversion(Vector3 rosIn)
    {
        return new Vector3(-rosIn.y, rosIn.z, rosIn.x);
    }

    //Convert ROS quaternion to Unity Quaternion
    public static Quaternion RosToUnityQuaternionConversion(Quaternion rosIn)
    {
        return new Quaternion(rosIn.x, -rosIn.z, rosIn.y, rosIn.w);
    }
    public static Vector3 UnityToRosPositionAxisConversion(Vector3 rosIn)
    {
        return new Vector3(rosIn.z, -rosIn.x, rosIn.y);
    }

    public static Quaternion UnityToRosRotationAxisConversion(Quaternion qIn)
    {
        Quaternion temp = (new Quaternion(-qIn.z, qIn.x, -qIn.y, qIn.w)) * (new Quaternion(0, 0, 0, 1));
        return temp;
    }
}