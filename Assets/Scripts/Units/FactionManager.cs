using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public int factionID = 0;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetRendererColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFaction(int newFactionID)
    {
        factionID = newFactionID;
        SetRendererColor();
    }

    private void SetRendererColor()
    {
        Renderer renderer = GetComponent<MeshRenderer>();
        Color newColor = Color.red;

        switch (factionID)
        {
            case 1:
                newColor = Color.blue;
                break;
            default:
                break;
        }

        renderer.material.color = newColor;
    }
}
