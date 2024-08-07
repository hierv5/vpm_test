using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using System;
using System.IO;

#if UNITY_EDITOR
public class AutoFbxExporter : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private bool TrackingEnabled = false;

    #region Buttons
    
    private Button InputSelectButton => rootVisualElement.Q<Button>("InputSelect");
    private Button OutputSelectButton => rootVisualElement.Q<Button>("OutputSelect");
    private Button ScriptSelectButton => rootVisualElement.Q<Button>("ScriptSelect");
    private Button TrackingButton => rootVisualElement.Q<Button>("trackingButton");

    #endregion

    #region TextFields

    private TextField InputPathTextField => rootVisualElement.Q<TextField>("InputText");
    private TextField OutputPathTextField => rootVisualElement.Q<TextField>("OutputText");
    private TextField ScriptPathTextField => rootVisualElement.Q<TextField>("ScriptText");

    #endregion

    private FileSystemWatcher Watcher = new FileSystemWatcher();

    #region Options

    private TextField CollectionNameText => rootVisualElement.Q<TextField>("CollectionNameText");

    #endregion

    [MenuItem("Tools/Hierv5Tools/AutoFbxExporter")]
    public static void ShowExample()
    {
        AutoFbxExporter wnd = GetWindow<AutoFbxExporter>();
        wnd.titleContent = new GUIContent("AutoFbxExporter");
    }

    /// <summary>
    /// UIを作成する
    /// </summary>
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Auto FBX Export Tool");
        root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        InputSelectButton.clicked += InputSelectButton_clicked;
        OutputSelectButton.clicked += OutputSelectButton_clicked;
        ScriptSelectButton.clicked += ScriptSelectButton_clicked;

        TrackingButton.clicked += TrackingButton_clicked;

        UpdateButtonState();
    }

    private void TrackingButton_clicked()
    {
        if(TrackingEnabled)
            StopTracking();
        else
            StartTracking();
    }

    private void StartTracking()
    {
        Watcher.Path = Path.GetDirectoryName(InputPathTextField.value);
        Watcher.Filter = Path.GetFileName(InputPathTextField.value);
        Watcher.Changed += Watcher_Changed;
        Watcher.EnableRaisingEvents = true;

        TrackingButton.text = "Stop Tracking";

        TrackingEnabled = true;
    }

    private void Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        ExportFbx();
    }

    private void StopTracking()
    {
        Watcher.Changed -= Watcher_Changed;
        Watcher.EnableRaisingEvents = false;

        TrackingButton.text = "Start Tracking";

        TrackingEnabled = false;
    }

    private void UpdateButtonState()
    {
        if (TrackingEnabled)
        {
            InputSelectButton.SetEnabled(false);
            OutputSelectButton.SetEnabled(false);
            ScriptSelectButton.SetEnabled(false);
            TrackingButton.SetEnabled(true);
        }
        else
        {
            InputSelectButton.SetEnabled(true);
            OutputSelectButton.SetEnabled(true);
            ScriptSelectButton.SetEnabled(true);

            bool inputFieldFilled = !string.IsNullOrEmpty(InputPathTextField.value);
            bool outputFieldFilled = !string.IsNullOrEmpty (OutputPathTextField.value);
            bool scriptFileldFilled = !string.IsNullOrEmpty(ScriptPathTextField.value);
            TrackingButton.SetEnabled(inputFieldFilled && outputFieldFilled && scriptFileldFilled);
        }
    }

    private void InputSelectButton_clicked()
    {
        var path = EditorUtility.OpenFilePanel("Open blend", "", "blend");
        if(string.IsNullOrEmpty(path)) return;

        InputPathTextField.value = path;

        UpdateButtonState();
    }

    private void OutputSelectButton_clicked()
    {
        var path = EditorUtility.SaveFilePanel("Save fbx", Application.dataPath, "untitled", "fbx");
        if(string.IsNullOrEmpty(path)) return;

        OutputPathTextField.value = path;

        UpdateButtonState();
    }

    private void ScriptSelectButton_clicked()
    {
        var defaultDir = Path.Join(Application.dataPath, "hierv5\\Editor\\AutoFbxExporter");
        var path = EditorUtility.OpenFilePanel("Open python script", defaultDir, "py");
        if(string.IsNullOrEmpty(path)) return;

        ScriptPathTextField.value = path;

        UpdateButtonState();
    }

    private void ExportFbx()
    {
        var input = InputPathTextField.value;
        var output = OutputPathTextField.value;
        var script = ScriptPathTextField.value;

        var collection = CollectionNameText.value;

        var process = new System.Diagnostics.ProcessStartInfo();
        process.FileName = "blender";
        process.Arguments = $"--factory-startup -b -P {script} -- --input {input} --output {output} --collection \"{collection}\"";

        System.Diagnostics.Process.Start(process).WaitForExit();
    }
}

#endif
