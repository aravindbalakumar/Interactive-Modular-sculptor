using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using System.Collections.Generic;
using TMPro;

public class SerialHandler : MonoBehaviour
{
    //[SerializeField] Dropdown dropdown;
    [Header("UI components")]
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] Button startButton;
    [SerializeField] GameObject audioVisualizerScreen;
    [SerializeField] GameObject portSelectionScreen;
    [SerializeField] TextMeshProUGUI baudRate;
    [SerializeField] TMP_InputField valuePrinter;
    [Header("References")]
    [SerializeField] SerialController controller;
    [SerializeField] AudioSource source;
    [Header("Data")]
    [SerializeField] List<RangeToMusicMap> audioMapping;
    List<TMP_Dropdown.OptionData> options;
    string message;
    int audioIndex=-1;
    int rangeValue = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        options = new List<TMP_Dropdown.OptionData>();
        dropdown.ClearOptions();
        foreach (string str in SerialPort.GetPortNames())
        {
            options.Add(new TMP_Dropdown.OptionData(str));
            
        }
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(OnDropdownSelected);
        startButton.onClick.AddListener(OnStartClicked);
    }
    void OnDropdownSelected(int optionIndex)
    {
        controller.portName = dropdown.options[optionIndex].text;
    }
    void OnStartClicked()
    {
        audioVisualizerScreen.SetActive(true);
        controller.baudRate = baudRate.text.parseToInt();
        portSelectionScreen.SetActive(false);
        controller.StartCaptureSerialData();
    }
    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
    }

    public void OnConnectionEvent(bool connectionState)
    {
        if (connectionState)
        {
            Debug.Log("CONNECTED");
        }
        else
        {
            Debug.Log("DISCONNECTED");
        }
    }


    public void OnMessageArrived(string message)
    {
        this.message = message;
        rangeValue = this.message.parseToInt();
        valuePrinter.text=$"{rangeValue}";
        int index = audioMapping.FindIndex(x=> rangeValue>=x.from && rangeValue<=x.to );
        if(index==-1)
        {
            Debug.LogWarning("PLAY NOTHING");
        }
        else
        {
            if (audioIndex != index)
            {
                source.resource = audioMapping[index].clip;
                source.Play();
                audioIndex = index;
            }
            else
            {
                if(!source.isPlaying)
                {
                    source.Play();
                }
            }
        }

    }
    [ContextMenu("Play random Test Music")]
    public void PlayRandomTestMusic()
    {
        source.resource= audioMapping[Random.Range(0, audioMapping.Count)].clip;
        source.Play();
    }
    [ContextMenu("Duplicate checker")]
    public void CheckForDuplicateAudioFiles()
    {
       for(int i=0;i<audioMapping.Count;i++)
        {
            int index = audioMapping.FindIndex(x => x.clip.name == audioMapping[i].clip.name);
            if (index != -1 && index != i) Debug.LogWarning("DUPLICATE FOUND AT INDEX "+ index);
        }
    }
}
[System.Serializable]
public struct RangeToMusicMap
{
    [SerializeField] public int from;
    [SerializeField] public int to;
    [SerializeField] public AudioClip clip;
}
public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static int Remap(this int value, int from1, int to1, int from2, int to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static int parseToInt(this string value)
    {
        int intValue = int.MinValue;
        if(int.TryParse(value, out intValue))
        {
            return intValue;
        }
        else
        {
            return int.MinValue;
        }
    }
}

