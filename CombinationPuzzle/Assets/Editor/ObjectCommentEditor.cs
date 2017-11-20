using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectComment))]
public class ObjectCommentEditor : Editor
{

    private string option = "";
    private bool isInputOption = false;

    //インスペクタ上の表示設定
    public override void OnInspectorGUI()
    {
        if (isInputOption)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("年月日"))
            {
                option = System.DateTime.Now.ToString("yyyy-MM-dd");
            }
            if (GUILayout.Button("年月日時"))
            {
                option = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("######"))
                option = "############################";

            if (GUILayout.Button("------"))
                option = "----------------------------";
            if (GUILayout.Button("======"))
                option = "=============================";
            if (GUILayout.Button("******"))
                option = "*****************************";

            GUILayout.EndVertical();

            option = EditorGUILayout.TextArea(option);
            EditorGUILayout.Space();
        }
        else
        {
            if (GUILayout.Button("入力補助", GUILayout.Height(30)))
                isInputOption = true;

        }

        ObjectComment myMemo = (ObjectComment)target;

        GUILayout.Label("コメント：");
        myMemo.comment = EditorGUILayout.TextArea(myMemo.comment);
    }



    [MenuItem("Component/etc/Add ObjectComment Component")]
    [MenuItem("GameObject/Create Other/Add ObjectComment Component", false, 200)]
    private static void AddObjectCommentComponent()
    {
        if (Selection.activeTransform != null)
        {
            Selection.activeGameObject.AddComponent<ObjectComment>();
        }
    }
}