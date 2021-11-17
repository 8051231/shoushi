using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaMenPercentText : MonoBehaviour
{
   // private TextMesh TextMeshName;
    public static FaMenPercentText Instance;

    public Text theTextMesh;

    private void Awake()
    {
       
        Hide();
        Instance = this;
    }

    public void Show(string myText="")
    {
        this.gameObject.SetActive(true);
        // Transform findText = transform.Find("Title/TextName");
        // theTextMesh = findText.GetComponent<Text>();
        if (myText!="")
        {
            theTextMesh.text = myText;
        }
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
