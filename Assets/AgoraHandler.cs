using agora_gaming_rtc;
using UnityEngine;

public class AgoraHandler : MonoBehaviour {
    
    private IRtcEngine mRtcEngine;

    void Start() {
        mRtcEngine = TestHome.testHelloUnityVideo.GetEngine;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        MuteAudio(true);
        mRtcEngine.EnableLocalVideo(false);
    }

    public void MuteAudio(bool mute) {
        mRtcEngine.MuteLocalAudioStream(mute);
        if(mRtcEngine == null) {
            print("AUDIO REF WAS NULL!!!");
        }
    }
}
