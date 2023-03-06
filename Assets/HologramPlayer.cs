using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class HologramPlayer : MonoBehaviour {


    [SerializeField]
    private VideoClip[] videoClips;
    [SerializeField]
    private TextMeshProUGUI txtVideoClipName;

    private VideoPlayer videoPlayer;
    int currentIndex = 0;
    Coroutine delayRoutine;
    const float offset = 1000;
    float originalY;

    private void Start() {
        txtVideoClipName.text = videoClips[currentIndex].name;
    }

    public void PlayPreviousClip() {
        GetResources();
        currentIndex = currentIndex <= 0 ? (videoClips.Length - 1) : --currentIndex;
        videoPlayer.transform.localPosition = new Vector3(videoPlayer.transform.localPosition.x, offset, videoPlayer.transform.localPosition.z);
        PlayClip();
    }

    public void PlayNextClip() {
        GetResources();

        //videoPlayer.Stop();
        currentIndex = currentIndex >= (videoClips.Length - 1) ? 0 : ++currentIndex;
        videoPlayer.transform.localPosition = new Vector3(videoPlayer.transform.localPosition.x, offset, videoPlayer.transform.localPosition.z);
        PlayClip();
    }

    private void GetResources() {
        if (videoPlayer == null) {
            videoPlayer = FindObjectOfType<VideoPlayer>();
            originalY = videoPlayer.transform.localPosition.y;
        }
    }

    private void PlayClip() {
        videoPlayer.clip = videoClips[currentIndex];
        videoPlayer.Play();

        if (delayRoutine != null) {
            StopCoroutine(delayRoutine);
        }
        delayRoutine = StartCoroutine(DelayPlay());

        txtVideoClipName.text = videoPlayer.clip.name;
    }

    public IEnumerator DelayPlay() {
        yield return new WaitForSeconds(1);
        videoPlayer.transform.localPosition = new Vector3(videoPlayer.transform.localPosition.x, originalY, videoPlayer.transform.localPosition.z);
    }
}
