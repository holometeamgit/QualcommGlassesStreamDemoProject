using agora_gaming_rtc;
using HoloMeSDK;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

public class BeemScript : MonoBehaviour {
    [SerializeField]
    private Transform targetCameraTransform;

    private AudioSource audioSource;

    [SerializeField]
    private VideoClip videoClip;

    [SerializeField]
    private GameObject liveQuadPrefabRef;

    private HoloMe holoMe;

    void Awake() {
        holoMe = new HoloMe();
        holoMe.Init(targetCameraTransform);
        holoMe.PositionOffset = -0.2f;
        //holoMe.DisableAmbientLighting();
        holoMe.LoopVideo = true;
        holoMe.UseAudioSource(holoMe.HologramTransform.gameObject.AddComponent<AudioSource>());
        //holoMe.PlaceVideo(new Vector3(0, -1.25f, 4));
        //holoMe.PlayVideo(videoClip);

        //holoMe.HologramTransform.parent.gameObject.AddComponent<rk>()           
    }

    public void ActivateHologram(Vector3 position) {
        if (!holoMe.IsPlaying) {
            holoMe.PlayVideo(videoClip);
        }

        if (holoMe.HologramTransform.GetComponent<ARAnchor>() == null) {
            holoMe.HologramTransform.gameObject.AddComponent<ARAnchor>();
        }

        MoveHologram(position);
    }

    public void MoveHologram(Vector3 position) {
        holoMe.PlaceVideo(position);
    }

    public VideoSurface CreateStreamQuad(string name) {
        holoMe.StopVideo();
        holoMe.ToggleVideoQuadRenderer(true);

        GameObject liveQuadObject = GameObject.Instantiate(liveQuadPrefabRef);
        liveQuadObject.name = name;

        // configure videoSurface
        VideoSurface videoSurface = liveQuadObject.AddComponent<VideoSurface>();
        videoSurface.SetVideoSurfaceType(AgoraVideoSurfaceType.Renderer);

        int activeChildCount = 0;
        for (int i = 0; i < holoMe.HologramTransform.childCount; i++) {
            if (holoMe.HologramTransform.GetChild(i).gameObject.activeInHierarchy) {
                activeChildCount++;
            }
        }

        liveQuadObject.transform.parent = holoMe.HologramTransform;

        //gameobjectAsChild.transform.eulerAngles = Vector3.zero; //new Vector3(90, 0.0f, 0.0f);
        liveQuadObject.transform.localRotation = Quaternion.identity;
        liveQuadObject.transform.Rotate(90, 0.0f, 0.0f);

        //gameobjectAsChild.transform.localScale = new Vector3(0.83f, 0.49f, 1);
        //gameobjectAsChild.transform.localScale = new Vector3(2, -3.4f, 1f);
        liveQuadObject.transform.localScale = new Vector3(0.141f, 1f, -0.141f * 1.7f);
        liveQuadObject.transform.localPosition = new Vector3(activeChildCount * 1.4f, 0, 0); //Vector3.zero;
        liveQuadObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("HLM/Unlit/GreenscreenRemoval");
        liveQuadObject.GetComponent<MeshRenderer>().material.SetFloat("_useAlphaFromMask", 1);

        return videoSurface;
    }

    public void ToggleVideoQuad(string UID, bool enable) {
        print("Toggling video " + UID + " enable = " + enable);

        for (int i = 0; i < holoMe.HologramTransform.childCount; i++) {

            Transform quad = holoMe.HologramTransform.GetChild(i);

            if (quad.name == UID) {
                quad.gameObject.SetActive(enable);
            }
        }
    }

    public void DeleteQuad(string UID) {
        for (int i = holoMe.HologramTransform.childCount - 1; i >= 0; i--) {

            Transform quad = holoMe.HologramTransform.GetChild(i);

            if (quad.name == UID) {
                GameObject.Destroy(quad.gameObject);
            }
        }
    }

}
