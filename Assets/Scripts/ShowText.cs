using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Msg
{
    public string message;
    public string day;
    public string time;

}

public class ShowText : MonoBehaviour {

    [SerializeField]
    GameObject inputObject; 

    [SerializeField]
    GameObject textObject;

    [SerializeField] 
    GameObject image;

    StreamReader sr;
    StreamWriter sw;

    string path;
    Msg msg;
    bool any; // if there was no input, do not overwrite json file 

    void Start () {

        path = Application.persistentDataPath + "\\" + "savetext.json";
        msg = new Msg();
        any = false;

        // get input field and assign its input event
        InputField inField = inputObject.GetComponent<InputField>();
        inField.onEndEdit.AddListener(delegate { InputEvent(inField); });

        // if the file exists, get last message, else message will be the default
        if (File.Exists(path))
        {
            sr = new StreamReader(path);
            msg = JsonUtility.FromJson<Msg>(sr.ReadLine());
            sr.Close();
        } else
        {
            msg.message = "No messages recieved yet.";
            msg.day = DateTime.Today.ToString("dd-MM-yyyy");
            msg.time = DateTime.Now.ToString("HH:mm");
        }

        // display text on screen
        textObject.GetComponent<Text>().text = msg.message + " DAY: " + msg.day + " TIME: " + msg.time;
    }

    private void InputEvent(InputField input)
    {

        // empty string
        if (input.text.Length == 0)
        {
            Debug.Log("Input Empty");
        }
        else if (input.text.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        // pinapple is not permitted -- show rejection image
        else if (input.text.Contains("pinapple"))
        {
            image.SetActive(true);
            textObject.GetComponent<Text>().text = "Sorry, no pinapples allowed. Try again.";
        }
        // display new message
        else
        {
            any = true;
            image.SetActive(false);
             
            msg.message = input.text;
            msg.day = DateTime.Today.ToString("dd-MM-yyyy");
            msg.time = DateTime.Now.ToString("HH:mm");

            textObject.GetComponent<Text>().text = msg.message + " DAY: " + msg.day + " TIME: " + msg.time;
        }
    }

    private void OnApplicationQuit()
    {
        // if any messages were inputted, write last message to file
        // if no messages were inputted, the last write remains (or no file exists) 
        if (any)
        {
            sw = new StreamWriter(path); 
            string json = JsonUtility.ToJson(msg);
            sw.WriteLine(json);

            sw.Close();
        }
    }
}
