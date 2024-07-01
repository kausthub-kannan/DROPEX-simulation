using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject SettingsMenu;
    public GameObject HumanoidClusters;
    public GameObject CapsuleClusters;

    private void Start()
    {
        SettingsMenu.SetActive(false);
        HumanoidClusters.SetActive(true);
        CapsuleClusters.SetActive(false);
    }

    public void SettingsClick()
    {
        SettingsMenu.SetActive(!SettingsMenu.activeSelf);
    }

    public void Toggle()
    {
        HumanoidClusters.SetActive(!HumanoidClusters.activeSelf);
        CapsuleClusters.SetActive(!CapsuleClusters.activeSelf);
    }
}
