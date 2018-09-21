using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIRef : MonoBehaviour {
    public Text ammoCount;
    public Slider reloading;
    public Slider healthBar;
    public Text health;

    void Awake() {
        health = healthBar.gameObject.GetComponentInChildren<Text>();
    }
}
