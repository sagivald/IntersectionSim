using System.Collections;
using ROSBridgeLib.sensor_msgs;
using SimpleJSON;
/**
 * This defines a publisher. There had better be a corresponding subscriber somewhere. This is really
 * just a holder for the message topic and message type.
 * 
 * Version History
 * 3.1 - changed methods to start with an upper case letter to be more consistent with c#
 * style.
 * 3.0 - modification from hand crafted version 2.0
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.1
 */

namespace ROSBridgeLib {
	public class VelodynePublisher {
		
		public static string GetMessageTopic() {
			return "velodyne/points";
		}  
		
		public static string GetMessageType() {
			return "sensor_msgs/PointCloud2";
		}

		public static string ToYAMLString(sensor_msgs.PointCloud2Msg msg) {
			return msg.ToYAMLString();

		}
	}
}
