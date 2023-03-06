using System;
using UnityEngine;
using UnityEngine.Video;

namespace HoloMeSDK
{
    public class HoloMe
    {
        public Action<string> OnPlaybackError;

        public bool IsPlaying => videoPlayer.isPlaying;
        public bool IsPaused => videoPlayer.isPaused;

        bool initialized;
        public bool Initialized
        {
            get => initialized;
            private set => initialized = value;
        }

        float positionOffset;
        public float PositionOffset
        {
            get => positionOffset;
            set
            {
                positionOffset = value;
                UpdatePositionOffset();
            }
        }

        bool loopVideo;
        public bool LoopVideo
        {
            get
            {
                return loopVideo;
            }
            set
            {
                loopVideo = value;
                if (Initialized)
                    videoPlayer.isLooping = loopVideo;
            }
        }

        public Vector3 HologramPosition { get => auxiliaryParent?.transform.position ?? Vector3.zero; }
        public Transform HologramTransform { get => auxiliaryParent?.transform ?? null; }

        HologramVisibilityActions hologramVisibilityActions;
        TransparencyHandler transparencyHandler;
        LookRotationOneAxis lookRotationOneAxis;
        ScaleByPinch scaleByPinch;
        GameObject parentGameObject;
        GameObject quad;
        GameObject auxiliaryParent;
        VideoPlayer videoPlayer;

        //Negative required for correct orientation
        readonly Vector3 DefaultScale = new Vector3(-1.2f, 2.4f, 1);

        /// <summary>
        /// Call this to begin using the system
        /// </summary>
        /// <param name="hologramTarget">This is the target the hologram will face it could be a camera or any other item</param>
        public void Init(Transform hologramTarget)
        {
            if (!initialized)
            {
                parentGameObject = new GameObject("HoloMeParent");
                scaleByPinch = parentGameObject.AddComponent<ScaleByPinch>();

                quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.name = "VideoQuad";
                quad.transform.SetParent(parentGameObject.transform);
                quad.transform.localScale = DefaultScale;
                hologramVisibilityActions = quad.AddComponent<HologramVisibilityActions>();
                lookRotationOneAxis = quad.AddComponent<LookRotationOneAxis>();
                quad.GetComponent<MeshCollider>().enabled = false;
                quad.GetComponent<Renderer>().material.shader = Shader.Find("HLM/Unlit/GreenscreenRemoval");
                //quad.GetComponent<Renderer>().material.SetFloat("_useAlphaFromMask", 1);

                transparencyHandler = quad.AddComponent<TransparencyHandler>();
                transparencyHandler.HologramMat = quad.GetComponent<MeshRenderer>().material;
                transparencyHandler.MakeTransparent(true);

                videoPlayer = quad.AddComponent<VideoPlayer>();
                videoPlayer.playOnAwake = false;
                videoPlayer.isLooping = loopVideo;
                videoPlayer.prepareCompleted += PlayOnPrepared;
                videoPlayer.errorReceived += OnVideoPlayerError;

                auxiliaryParent = new GameObject("AuxiliaryParent");
                auxiliaryParent.transform.localScale = new Vector3(-1, 1, 1);
                auxiliaryParent.transform.parent = quad.transform;

                Initialized = true;

                if (hologramTarget == null)
                {
                    if (Application.isEditor)

                        Debug.LogWarning("No target was assigned was this intended?");
                }
                else
                {
                    SetTarget(hologramTarget);
                }

                UpdatePositionOffset();
            }
        }

        public void SetTarget(Transform newTarget)
        {
            if (LogNotInitializedWarning())
            {
                return;
            }

            lookRotationOneAxis.lookTarget = newTarget;
            transparencyHandler.FadeTarget = newTarget;
        }

        public void SetOnVisibleFunction(Action function)
        {
            hologramVisibilityActions.OnBecameVisible = function;
        }

        public void SetOnInvisibleFunction(Action function)
        {
            hologramVisibilityActions.OnBecameInvisible = function;
        }

        bool LogNotInitializedWarning()
        {
            if (!initialized)
            {
                Debug.LogWarning("HoloMe wasn't initialised, call Init() before using");
            }
            return !initialized;
        }

        void OnVideoPlayerError(VideoPlayer videoPlayer, string error)
        {
            Debug.LogError(error);
            OnPlaybackError?.Invoke(error);
        }

        #region AmbientLighting Functions
        public bool IsAmbientLightingEnabled
        {
            get
            {
                return quad.GetComponent<Renderer>().material.IsKeywordEnabled("USE_AMBIENT_LIGHTING");
            }
        }

        public void EnableAmbientLighting()
        {
            if (LogNotInitializedWarning())
            {
                return;
            }
            quad.GetComponent<Renderer>().material.EnableKeyword("USE_AMBIENT_LIGHTING");
        }

        public void DisableAmbientLighting()
        {
            if (LogNotInitializedWarning())
            {
                return;
            }
            quad.GetComponent<Renderer>().material.DisableKeyword("USE_AMBIENT_LIGHTING");
        }
        #endregion

        public void UseAudioSource(AudioSource audioSource)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        public void MuteClip()
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }

        public void TogglePinchScaleFunctionality(bool enabled)
        {
            scaleByPinch.enabled = enabled;
        }

        public void SetPinchMinMaxScaleValues(float min, float max)
        {
            scaleByPinch.SetMinScale(min);
            scaleByPinch.SetMaxScale(max);
        }

        public void Disable()
        {
            parentGameObject?.SetActive(false);
        }

        public void ToggleVideoQuadRenderer(bool disable) {
            quad.GetComponent<MeshRenderer>().enabled = !disable ;
        }

        /// <summary>
        /// Place video and play if not already playing
        /// </summary>
        /// <param name="position"></param>
        /// <param name="playAfterPlace"></param>
        public void PlaceVideo(Vector3 position)
        {
            if (LogNotInitializedWarning())
            {
                return;
            }

            //Debug.Log($"Called {position}");

            parentGameObject.transform.position = position;

            //Debug.Log($"Called Here {position} vs {parentGameObject.transform.position}");

            //if (playAfterPlace && !videoPlayer.isPlaying)
            //{
            //    PlayVideo(videoPlayer.clip);
            //    //Debug.Log($"Called Here {position} vs {parentGameObject.transform.position}");
            //}
        }

        public void PauseVideo()
        {
            if (LogNotInitializedWarning())
            {
                return;
            }
            videoPlayer.Pause();
            //Debug.Log("PAUSE CALLED");
        }

        public void ResumeVideo()
        {
            if (LogNotInitializedWarning())
            {
                return;
            }

            if (videoPlayer.isPaused)
            {
                videoPlayer.Play();
            }
        }

        public void SetScale(float scaleFactor)
        {
            if (LogNotInitializedWarning())
            {
                return;
            }
            parentGameObject.transform.localScale = Vector3.one * scaleFactor;
            //Debug.Log(parentGameObject.transform.localScale);
            UpdatePositionOffset();
        }

        void UpdatePositionOffset()
        {
            if (LogNotInitializedWarning())
            {
                return;
            }
            quad.transform.localPosition = new Vector3(0, quad.transform.localScale.y / 2 + positionOffset, 0);
        }

        public void PlayVideo(string videoURL)
        {
            if (LogNotInitializedWarning())
            {
                return;
            }

            if (string.IsNullOrEmpty(videoURL))
            {
                Debug.LogError("Clip was null");
                return;
            }

            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = videoURL;
            Play();
        }

        public void PlayVideo(VideoClip clip)
        {
            if (LogNotInitializedWarning())
            {
                return;
            }

            if (clip == null)
            {
                Debug.LogError("Clip was null");
                return;
            }

            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = clip;
            Play();
        }

        void Play()
        {
            transparencyHandler.MakeTransparent(true);

            if (videoPlayer.isPrepared)
            {
                videoPlayer.Stop();
                videoPlayer.Prepare();
            }
            else
            {
                videoPlayer.Prepare();
            }
        }

        void PlayOnPrepared(VideoPlayer player)
        {
            player.Play();
            transparencyHandler.MakeTransparent(false);
        }

        public void StopVideo()
        {
            if (LogNotInitializedWarning())
            {
                return;
            }
            videoPlayer.Stop();
        }

        public void RestartVideo()
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }

        bool IsVideoObjectInView()
        {
            throw new NotImplementedException();
        }

        public void DeInit()
        {
            Initialized = false;
            videoPlayer.Stop();
            hologramVisibilityActions.ClearEvents();
            GameObject.Destroy(parentGameObject);
        }
    }
}