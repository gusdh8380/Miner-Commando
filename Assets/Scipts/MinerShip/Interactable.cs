using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable { 

    void ShowUI();
    void HideUI();
    void Interact(GameObject subject);
    GameObject gameObject { get; }  // GameObject 속성 추가
}
