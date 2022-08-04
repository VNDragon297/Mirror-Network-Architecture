using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSlotUI : MonoBehaviour
{
    public Image PFP;
    public TMP_Text DisplayName;

    public bool isAvailable = false;
    public bool isOccupied = false;

    public void SetDisplayName(string name) => DisplayName.SetText((string.IsNullOrEmpty(name)) ? "Available" : name);
}
