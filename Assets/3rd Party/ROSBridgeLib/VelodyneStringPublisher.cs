using System.Collections;
using ROSBridgeLib.std_msgs;
using SimpleJSON;

namespace ROSBridgeLib {
	public class VelodyneStringPublisher {
		
		public static string GetMessageTopic() {
			return "velodynestring";
		}  
		
		public static string GetMessageType() {
			return "std_msgs/String";
		}

		public static string ToYAMLString(StringMsg msg) {
			return msg.ToYAMLString();

		}
	}
}
