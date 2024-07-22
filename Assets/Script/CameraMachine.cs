using UnityEngine;

[ExecuteInEditMode]
public class CameraMachine : MonoBehaviour
{
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = target.transform.position + new Vector3(0, 0, -10);
    }
}
