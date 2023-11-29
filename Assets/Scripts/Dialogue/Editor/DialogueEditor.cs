using System;
using System.Runtime.CompilerServices;
using Codice.Client.Common.WebApi;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.MPE;
using UnityEngine;

namespace MyBrotherZ.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        Dialogue selectedDialogue = null;
        [NonSerialized]
        GUIStyle nodeStyle;
        [NonSerialized]
        GUIStyle playerNodeStyle;
        [NonSerialized]
        DialogueNode draggingNode = null;
        [NonSerialized]
        Vector2 draggingOffset;
        [NonSerialized]
        DialogueNode creatingNode = null;
        [NonSerialized]
        DialogueNode deletingNode = null;
        [NonSerialized]
        DialogueNode likingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized]
        bool draggingCanvas = false;
        [NonSerialized]
        Vector2 draggingCanvasOffset;

        private const float canvasSize = 4000;
        private const float backgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue != null)
            {
                ShowEditorWindow();
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChange;
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20,20,20,20);
            nodeStyle.border = new RectOffset(12,12,12,12);
            
            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20,20,20,20);
            playerNodeStyle.border = new RectOffset(12,12,12,12);
        }

        private void OnSelectionChange()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if (selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
            else
            {
                ProcessEvents();

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                Rect canvas = GUILayoutUtility.GetRect(canvasSize,canvasSize);
                Texture2D backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0,0 , canvasSize/backgroundSize, canvasSize/backgroundSize);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex ,texCoords);
                
                foreach (DialogueNode node in selectedDialogue.GetAllNodes()){
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes()){
                    DrawNode(node);
                }
            
                if (creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                EditorGUILayout.EndScrollView();
                if (deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                    
                }
            }
            
        }

       

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode  == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode  != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }

            }else if (Event.current.type == EventType.MouseDrag && draggingNode!=null)
            {
                
                draggingNode.SetRectPosition(Event.current.mousePosition + draggingOffset);

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;
                GUI.changed = true;
            }
            else if(Event.current.type == EventType.MouseUp && draggingNode!=null)
            {
                draggingNode = null;
            }else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 currentMousePosition)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(currentMousePosition))
                {
                    foundNode =  node;
                }
            }
            return foundNode;
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);
            
            EditorGUILayout.LabelField("Node:",EditorStyles.whiteLabel);
            
            node.SetText(EditorGUILayout.TextField(node.GetText()));
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete"))
            {
                deletingNode = node;
            }
            DrawLinkButtons(node);
            if (GUILayout.Button("Add"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (likingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    likingParentNode = node;
                }
            }else if (likingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    likingParentNode = null;
                }
            }else if (likingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Unlink"))
                {
                    likingParentNode.RemoveChild(node.name);
                    likingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("Child"))
                {
                    
                    likingParentNode.AddChild(node.name);
                    likingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new Vector2(node.GetRect().xMax , node.GetRect().center.y );
            foreach (DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPos = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPos - startPos;
                controlPointOffset.y = 0;
                controlPointOffset.x *= .8f;
                Handles.DrawBezier(
                    startPos,endPos,
                    startPos + controlPointOffset,
                    endPos - controlPointOffset,
                    Color.white, null,4f);
            }
        }
    }
}

