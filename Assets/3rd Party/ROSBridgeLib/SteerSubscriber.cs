using System.Collections;
using SimpleJSON;
using ROSBridgeLib;
using ROSBridgeLib.std_msgs;
using UnityEngine;
/**
 * This defines a subscriber. Subscribers listen to publishers in ROS. Now if we could have inheritance
 * on static classes then we could do this differently. But basically, you have to make up one of these
 * for every subscriber you need.
 * 
 * Subscribers require a ROSBridgePacket to subscribe to (its type). They need the name of
 * the message, and they need something to draw it. 
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
	public class SteerSubscriber:ROSBridgeSubscriber {

		public new static string GetMessageTopic() {
			return "/Unity/Throttle";
		}  

		public new static string GetMessageType() {
			return "Float32";
		}

		public new static ROSBridgeMsg ParseMessage(JSONNode msg) {
			return new Float32Msg(msg);
		}

		public new static void CallBack(ROSBridgeMsg msg) {
			GameObject robot=GameObject.Find("HEMTT2");
		}
	}
}

