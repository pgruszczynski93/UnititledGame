using UnityEngine;
using System.Collections;

public class TypeProxy : MonoBehaviour {
    public GameObject typeWriter;

    public void StartMessage()
    {
        typeWriter.GetComponent<TypeWriter.Typewriter>().StartMesage();
    }
}
