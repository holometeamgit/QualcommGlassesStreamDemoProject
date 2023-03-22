using TMPro;
using UnityEngine;

public class DebugLogger : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI textLogText;

    private void Awake() {
        Application.logMessageReceived += Log;
    }

    public void Log(string condition, string stackTrace, LogType type) {
        textLogText.text += System.Environment.NewLine + condition;
    }
}
