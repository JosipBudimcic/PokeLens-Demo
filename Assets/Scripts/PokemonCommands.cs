using UnityEngine;
using System.Collections;

public class PokemonCommands : MonoBehaviour {

    Vector3 originalPosition;
    bool Set = false;
    // Use this for initialization
    void Start()
    {
        // Grab the original local position of the sphere when the app starts.
        originalPosition = this.transform.localPosition;
    }
    void OnSceneReset()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
