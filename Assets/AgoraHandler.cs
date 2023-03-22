using agora_gaming_rtc;
using UnityEngine;

public class AgoraHandler : MonoBehaviour {

    private IRtcEngine mRtcEngine;

    public void Initialise() {
        mRtcEngine = TestHome.testHelloUnityVideo.GetEngine;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        MuteAudio(true);
        ActivateCamera(false);
    }

    public void MuteAudio(bool mute) {
        mRtcEngine.MuteLocalAudioStream(mute);
        if (mRtcEngine == null) {
            print("AUDIO REF WAS NULL!!!");
        }
    }

    public void ActivateCamera(bool enable) {
        mRtcEngine.EnableLocalVideo(enable);
    }
}
