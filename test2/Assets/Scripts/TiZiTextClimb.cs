using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiZiTextClimb : MonoBehaviour
{
    public static TiZiTextClimb Instance;

    public Text TiZitexts;
 //   public TextMesh TiZitexts;
    private void Awake()
    {
        TiZitexts = this.GetComponent<Text>();
        Hide();
        Instance = this;
    }

    public void Show(string TText = "")
    {
        if (TText != "")
        {
            TiZitexts.text = "";
            TiZitexts.text = TText;
        }
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
