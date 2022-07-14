using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        try
        {
            var cp = GameObject.Find("MixedRealityPlayspace").GetComponent<TCPClientForLocalization>();
            Debug.Log(cp.PORT_NUM);
            cp.SendCurrentMessage("TEST");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
        }

    }

    public void OnClick2()
    {
        Application.Quit();
    }
}
