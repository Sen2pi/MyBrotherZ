using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.IO;
using UnityEngine.EventSystems;

namespace Managers.Scene_Management
{
    public class Menu : MonoBehaviour
    {
        [Header("First Selected Button")]
        [SerializeField] private GameObject firstSelectedButton;

        protected virtual void OnEnable()
        {
            StartCoroutine(SetFirstSelectedRoutine(firstSelectedButton));
        }

        public IEnumerator SetFirstSelectedRoutine(GameObject firstSelected)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }
}