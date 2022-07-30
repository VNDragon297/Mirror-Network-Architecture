using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotUI : MonoBehaviour
{
    public Image PFP;
    public TMP_Text DisplayName;

    public void SetDisplayName(string name) => DisplayName.SetText((string.IsNullOrEmpty(name)) ? "Available" : name);
}
