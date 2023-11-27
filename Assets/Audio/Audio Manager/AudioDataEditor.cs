#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioData))]
public class AudioDataEditor : Editor
{
    [SerializeField] private AudioSource _previewer;
    [SerializeField] private bool _loop;
    [SerializeField] private bool _play;

    
    public void OnEnable()
    {
        _previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        EditorApplication.update += EditorUpdate;
    }

    public void OnDisable()
    {
        DestroyImmediate(_previewer.gameObject);
        EditorApplication.update -= EditorUpdate;
    }

    void EditorUpdate()
    {
        if (_play && !_previewer.isPlaying)
        {
            if (_loop)
            {
                AudioData data = (AudioData) target;
                data.Setup(_previewer, true);
                _previewer.Play();
            }
            else
            {
                _play = false;
                EditorUtility.SetDirty(target);
            }
        }
    }

    public override void OnInspectorGUI()
    {       
        DrawDefaultInspector();

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
        GUILayout.Space(40);
        GUILayout.Label("Preview");
        _loop = GUILayout.Toggle(_loop, "Loop");
        if (!_play)
        {
            if (GUILayout.Button("Play"))
            {
                _play = true;
                AudioData data = (AudioData) target;
                data.Setup(_previewer, true);
                _previewer.Play();
            }
        }
        else
        {
            if (GUILayout.Button("Stop"))
            {
                _play = false;
                _previewer.Stop();
            }
        }
        EditorGUI.EndDisabledGroup();
    }
}
#endif