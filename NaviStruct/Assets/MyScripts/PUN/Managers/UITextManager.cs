using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Manager/UITextManager")]
public class UITextManager : ScriptableObject
{
    [SerializeField]
    private string _screenText;
    public string ScreenText { get { return _screenText; } set { _screenText = value; } }
}
