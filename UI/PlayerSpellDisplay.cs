using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellDisplay : MonoBehaviour
{
    public GameObject TextObject;

    private string SpellName;

    private void Update() {
        TextObject.GetComponent<TextMesh>().text = SpellName;    
    }

    public void SetSpellName(string name) {
        SpellName = name;
    }
}
