using System.Collections;
using ROSBridgeLib.std_msgs;
using SimpleJSON;

namespace ROSBridgeLib
{
    public class PubTest
    {
        
        public static string GetMessageTopic()
        {
            return "/SomeUnityValue";
        }

        public static string GetMessageType()
        {
            return "std_msgs/Float32";
        }

        public static string ToYAMLString(Float32Msg msg)
        {
            return msg.ToYAMLString();
        }
    }
}
