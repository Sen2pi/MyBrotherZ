using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyBrotherZ.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]private bool isPlayerSpeaking = false;
        [SerializeField]private string text;
        [SerializeField]private List<string> children = new List<string>();
        [SerializeField]private Rect rect = new Rect(0,0,200,100);
        [SerializeField] private string OnEnterAction;
        [SerializeField] private string OnExitAction;
        [SerializeField] private Condition condition;
        
        public string GetText()
        {
            return text;
        }
        public bool Checkcondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }
        public List<string> GetChildren()
        {
            return children;
        }
        public Rect GetRect()
        {
            return rect;
        }

        public string GetOnEnterAction()
        {
            return OnEnterAction;
            
        }

        public string GetOnExitAction()
        {
            return OnExitAction;
            
        }
        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }
#if UNITY_EDITOR

        public void SetRectPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this,"Move Dialogue Node");
            this.rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if (newText !=text)
            {
                Undo.RecordObject(this, "Update Dialogue Text");  
                text = newText;
            }
            EditorUtility.SetDirty(this);
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Add Dialogue Link");
            children.Add(childID);
            EditorUtility.SetDirty(this);   
        }

        public void RemoveChild(string childID)
        {

            Undo.RecordObject(this, "Remove Dialogue Link");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
#endif
        public void SetPlayerSpeaking(bool newIsPlayerSpeaking)
        {
            Undo.RecordObject(this, "Set Player Speaking");
            isPlayerSpeaking = newIsPlayerSpeaking;
            EditorUtility.SetDirty(this);
        }

        
    }
}

