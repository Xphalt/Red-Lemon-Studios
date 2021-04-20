using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : MonoBehaviour
{    
    private float explosionTimer;
    private float growthRate;
    private float currentRadius;
    private float colourRate;
    private float colourPhase;
    public float startRadius;
    public float endRadius;
    public float explosionDuration;

    internal Transform parent;

    public Color startColour;
    public Color endColour;
    public MeshRenderer myMesh;

    private void Awake()
    {
        if (myMesh == null) myMesh = GetComponent<MeshRenderer>();

        colourRate = 1 / explosionDuration;
        growthRate = (endRadius - startRadius) / explosionDuration;
        currentRadius = startRadius;
    }

    void Update()
    {
        currentRadius += Time.deltaTime * growthRate;
        transform.localScale = Vector3.one * currentRadius;

        colourPhase += Time.deltaTime * colourRate;
        myMesh.materials[0].color = Color.Lerp(startColour, endColour, colourPhase);

        if (colourPhase > 1 || currentRadius > endRadius)
        {
            transform.SetParent(parent);
            gameObject.SetActive(false);
        }
    }
}
