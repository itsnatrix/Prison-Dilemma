using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine.InputSystem;

public class DecisionTreeManager : MonoBehaviour{

    [SerializeField] private TMP_InputField playerInput;

    //Loading in examples for the ai
    public string examplesResource = "ai";
    public string toBeSaved = "ai.json";
    

    //Setting game properties
    public Transform player;

    public int question = 0;
    public string theQuestion = " ";
    //public int guiltyCount = 0; 
    public TMP_Text questionText;

    private ExampleList data;
    private Node tree;
    private readonly List<string> attributes = new List<string> {"contradicts", "detail", "selfMentioned", "confess"};
    
    private string SavePath => Path.Combine(Application.persistentDataPath,toBeSaved);

    void Start(){
        Debug.Log("Save file: " + SavePath);
        data = LoadExamples();
        RebuildTree();
        questionSelector();
    }

    ExampleList LoadExamples(){
        if (File.Exists(SavePath))
            return JsonUtility.FromJson<ExampleList>(File.ReadAllText(SavePath));

        TextAsset start = Resources.Load<TextAsset>(examplesResource);
        if (start != null){
            return JsonUtility.FromJson<ExampleList>(start.text);
        }

        Debug.LogWarning("No starting data found, so the AI starts empty.");
        return new ExampleList();
    }

    void SaveExamples(){
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    void RebuildTree(){
        tree = data.examples.Count > 0 ? ID3.Build(data.examples, attributes) : null;
    }

    SavedExample CurrentSituation(){
        string playerStatement = playerInput.text;
        string statement = " " + playerStatement.ToLower() + " ";
        statement = statement.Replace("-", " ");
        statement = statement.Replace(".", " ");
        statement = statement.Replace(",", " ");
        statement = statement.Replace("!", " ");
        statement = statement.Replace("?", " ");

        return new SavedExample{
            contradicts = (statement.Contains(" actually ") || statement.Contains(" but ")) ? "yes" : "no",
            detail = statement.Length > 32 ? "high" : "low",
            selfMentioned = statement.Contains(" i ") || statement.Contains(" me ") || statement.Contains(" my ") ? "yes" : "no", 
            confess = statement.Contains(" i did it ") || statement.Contains(" it was me ") || statement.Contains(" i'm guilty ") || statement.Contains(" i confess ") ? "yes" : "no"
            //sense
        };
    }

    public string Decide(){
        return tree == null ? "innocent" : tree.Classify(CurrentSituation());
    }

    public void Learn(SavedExample situation, string resultSaved){
        situation.result = resultSaved;
        data.examples.Add(situation);
        RebuildTree();
    }


    // Update is called once per frame
    void Update(){

        if (Keyboard.current.enterKey.wasPressedThisFrame){
            Debug.Log("AI decides: " + Decide());
            questionSelector();
        }
    }


    void Teach(string actionLearned){
        Learn(CurrentSituation(), actionLearned);
        Debug.Log($"Learned: {actionLearned} ({data.examples.Count} examples)");
        questionSelector();
    }

    void OnApplicationQuit(){
        SaveExamples();
    }

    void questionSelector(){
        if(question==3){
            question = 0;
        }
        question += 1;
        theQuestion = " ";
        if(question==1){
            theQuestion = "Where were you when the crime happened?";
        }
        else if(question==2){
            theQuestion = "How well did you know the victim?";
        }
        else{
            theQuestion = "How do we know you're being honest?";
        }
        questionText.text = theQuestion;
    }
}
