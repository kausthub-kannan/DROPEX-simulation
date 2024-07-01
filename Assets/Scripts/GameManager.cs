using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameManager : MonoBehaviour
{
    public InputField urlInputField;
    public InputField uploadTimeIntervalInputField;
    
    public PayloadDroneController payloadDroneController;
    public DroneController droneController;
    public Camera droneCamera;

    public Button landButton;
    public TextMeshProUGUI startInstruction;
    public TextMeshProUGUI controlInstruction;
    
    void Start()
    {
        landButton.onClick.AddListener(EventOnClickLandButton);
        landButton.gameObject.SetActive(false);
    }

    void Update()
    {
        DisplayUI();

        float speedX = Input.GetAxis("Horizontal");
        float speedZ = Input.GetAxis("Vertical");
        float speedY = 0;

        if ((Input.GetKey(KeyCode.Return)))
        {
            if (droneController.IsIdle())
            {
                droneController.TakeOff();
            }
            else
            {
                speedY = 1;
            }
        }
        else if ((Input.GetKey(KeyCode.RightShift)))
        {
            speedY = -1;
        }
        else
        {
            speedY = 0;
        }

        if ((Input.GetKey(KeyCode.Space)))
        {
            droneController.Land();
        }

        droneController.Move(speedX, speedY, speedZ);

        DateTime now = DateTime.Now;
        int seconds = now.Second;
        
        if ((seconds+1) % int.Parse(uploadTimeIntervalInputField.text) == 0 && droneController.IsFlying())
        {
            StartCoroutine(TakeSnapshotCoroutine());
        }
    }

    void EventOnClickLandButton()
    {
        droneController.Land();
    }

    void DisplayUI()
    {
        if (droneController.IsIdle())
        {
            landButton.gameObject.SetActive(false);
            controlInstruction.gameObject.SetActive(false);
            startInstruction.gameObject.SetActive(true);

        }
        else
        {
            landButton.gameObject.SetActive(true);
            controlInstruction.gameObject.SetActive(true);
            startInstruction.gameObject.SetActive(false);
        }
    }

    private IEnumerator TakeSnapshotCoroutine()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(droneCamera.pixelWidth, droneCamera.pixelHeight, 24);
        droneCamera.targetTexture = renderTexture;
        droneCamera.Render();
        
        RenderTexture.active = droneCamera.targetTexture;

        Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        snapshot.Apply();
        
        byte[] bytes = snapshot.EncodeToPNG();
        string base64Snapshot = Convert.ToBase64String(bytes);

        String serverUrl = urlInputField.text;
        UnityWebRequest www = new UnityWebRequest( serverUrl, "POST");

        WWWForm form = new WWWForm();
        form.AddField("snapshot", base64Snapshot);
        form.AddField("time", DateTime.Now.ToString("o"));

        www.uploadHandler = new UploadHandlerRaw(form.data);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            string jsonResult = www.downloadHandler.text;
            Dictionary<string, object> resultDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResult);
            bool status = bool.Parse(resultDict["status"].ToString());
            
            if (status)
            {
                if (!payloadDroneController.IsFlying())
                {
                    payloadDroneController.TakeOff();
                }
                payloadDroneController.Move(droneController.transform.position.x, droneController.transform.position.y, droneController.transform.position.z);
            }
        }
    }

}