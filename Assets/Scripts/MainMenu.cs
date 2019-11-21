using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    List<GameObject> inputFields;
    List<Color> availableColors;
    List<Color> takenColors;

    public GameObject panelOptions;

    public InputField inputFieldSeed;
    public InputField inputFieldSize;
    public Toggle toggleRoundMap;
    public Toggle toggleMountains;
    public Toggle toggleDragonMadness;

    public void Start()
    {
        inputFields = new List<GameObject>();
        takenColors = new List<Color>();
        
        // The more colors, the more possible players 
        availableColors = new List<Color> {Color.red, Color.blue, Color.green,
            Color.yellow, Color.black, Color.grey, Color.white, Color.cyan, Color.magenta};

        // Add first input field
        AddInputField();

        // Fetch the options panel
        panelOptions = GameObject.Find("PanelOptions");

        // Fetch option fields
        inputFieldSeed = GameObject.Find("Option Seed").transform.Find("InputField").GetComponent<InputField>();
        inputFieldSize = GameObject.Find("Option Size").transform.Find("InputField").GetComponent<InputField>();
        toggleRoundMap = GameObject.Find("Option Round Map").transform.Find("Toggle").GetComponent<Toggle>();
        toggleMountains = GameObject.Find("Option Mountains").transform.Find("Toggle").GetComponent<Toggle>();
        toggleDragonMadness = GameObject.Find("Option Dragon Madness").transform.Find("Toggle").GetComponent<Toggle>();

        // initialize options
        inputFieldSeed.text = UnityEngine.Random.Range(0, 99999).ToString();

        // Hide the options
        HideOptions();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            GameObject lastInputField = inputFields[inputFields.Count - 1].transform.Find("InputField").gameObject;

            if (lastInputField.Equals(EventSystem.current.currentSelectedGameObject))
            {
                if (lastInputField.GetComponent<InputField>().text == null || lastInputField.GetComponent<InputField>().text == "")
                {
                    if (takenColors.Count > 1)
                        RemoveInputField();
                }
                else
                {
                    if (availableColors.Count > 0)
                        AddInputField();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            GameObject lastInputField = inputFields[inputFields.Count - 1].transform.Find("InputField").gameObject;

            if (lastInputField.GetComponent<InputField>().text == null || lastInputField.GetComponent<InputField>().text == "")
            {
                if (takenColors.Count > 1)
                    RemoveInputField();
            }
        }
    }

    public void AddInputField()
    {
        int currentPlayers = inputFields.Count;
        GameObject scrollView = GameObject.Find("Scroll View");
        GameObject scrollViewContent = GameObject.Find("Content");

        // Spawn custom input field
        GameObject customInputField = Instantiate(Resources.Load(Constants.UIPrefabFolder + "Custom Input") as GameObject, Vector3.zero, Quaternion.identity);

        // Set position in scrollViewContent
        customInputField.transform.SetParent(scrollViewContent.transform, false);
        Vector3 pos = customInputField.GetComponent<RectTransform>().position;
        pos.y -= 75 + currentPlayers * 125;
        customInputField.GetComponent<RectTransform>().position = pos;

        // Set player label
        customInputField.transform.Find("Text Left Label").GetComponent<Text>().text = "Player " + (currentPlayers + 1);

        // Attach event listeners to the input field and the color field
        customInputField.GetComponentInChildren<InputField>().onEndEdit.AddListener(EnterText);
        customInputField.GetComponentInChildren<Button>().onClick.AddListener(ChangeColor);

        // Resize scrollview content and scroll to end
        scrollViewContent.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, (currentPlayers + 1) * 125);
        scrollView.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);

        // Set focus to new input
        customInputField.GetComponentInChildren<InputField>().Select();
        customInputField.GetComponentInChildren<InputField>().ActivateInputField();

        // Assign a color to the player
        customInputField.transform.Find("Panel Player Color").GetComponent<Image>().color = availableColors[0];
        takenColors.Add(availableColors[0]);
        availableColors.Remove(availableColors[0]);

        // Add to list for later use
        inputFields.Add(customInputField);
    }

    public void RemoveInputField()
    {
        // Remove from list
        GameObject customInputFieldToDestroy = inputFields[inputFields.Count - 1];
        inputFields.Remove(customInputFieldToDestroy);

        // Move color from taken to available colors
        availableColors.Add(customInputFieldToDestroy.transform.Find("Panel Player Color").GetComponent<Image>().color);
        takenColors.Remove(customInputFieldToDestroy.transform.Find("Panel Player Color").GetComponent<Image>().color);

        // Destroy the gameObject
        Destroy(customInputFieldToDestroy);
        customInputFieldToDestroy = null;

        // Resize scrollview content and scroll to end
        GameObject.Find("Content").transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 125 + inputFields.Count * 125);
        GameObject.Find("Scroll View").GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);

        // Set focus to last input
        inputFields[inputFields.Count - 1].GetComponentInChildren<InputField>().Select();
        inputFields[inputFields.Count - 1].GetComponentInChildren<InputField>().ActivateInputField();
    }

    public void ChangeColor()
    {
        GameObject caller = EventSystem.current.currentSelectedGameObject;

        // Move callers color from taken to available
        availableColors.Add(caller.GetComponent<Image>().color);
        takenColors.Remove(caller.GetComponent<Image>().color);

        // Assign new color to caller
        caller.GetComponent<Image>().color = availableColors[0];

        // Move new color from available to taken
        availableColors.Remove(caller.GetComponent<Image>().color);
        takenColors.Add(caller.GetComponent<Image>().color);
    }

    public void EnterText(string text)
    {
        // Unused
    }

    public void ShowOptions()
    {
        panelOptions.SetActive(true);
    }

    // Called from UI button & from this
    public void HideOptions()
    {
        panelOptions.SetActive(false);
    }

    // Called from UI button
    public void StartGame()
    {
        GameManager.Instance.Players.Clear();

        foreach (GameObject inputField in inputFields)
        {
            string name = inputField.GetComponentInChildren<InputField>().text;
            Color color = inputField.transform.Find("Panel Player Color").GetComponent<Image>().color;

            if (name != "" && name != null)
                GameManager.Instance.Players.Add(new Player(name, color));
        }

        try
        {
            GameManager.Instance.Seed = int.Parse(inputFieldSeed.text);
        }
        catch (Exception e)
        {
            Debug.Log("Parsing seed failed" + e.Message);
            GameManager.Instance.Seed = UnityEngine.Random.Range(0, 99999);
        }

        try
        {
            GameManager.Instance.BoardSizeX = int.Parse(inputFieldSize.text);
        }
        catch (Exception e)
        {
            Debug.Log("Parsing field size failed" + e.Message);
            GameManager.Instance.BoardSizeX = UnityEngine.Random.Range(25, 35);
        }

        GameManager.Instance.RoundMap = toggleRoundMap.isOn;
        GameManager.Instance.Mountains = toggleMountains.isOn;
        GameManager.Instance.DragonMadness = toggleDragonMadness.isOn;

        SceneManager.LoadScene("GameScene");
    }
}
