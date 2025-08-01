using Fusion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    public int health { get; set; } 
    public Slider healthSlider;

    void Start()
    { 
    }

    private void OnHealthChanged()
    {
        healthSlider.value = health;
    }
    void  Update()
    {
        if (HasStateAuthority)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                health -= 10; 
                Debug.Log("-10");
            }
        }
}