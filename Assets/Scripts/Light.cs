using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : MonoBehaviour
{
    public Material offMat;
    public Material onMat;

    public MeshRenderer meshRenderer;
    IEnumerator startLight;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        startLight = StartLight();
        StartCoroutine(startLight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartLight()
    {
        float time = 0f;

        while (true)
        {
            if (time >= 1f)
            {
                int random = Random.Range(0, 100);
                if(random <= 20)
                {
                    meshRenderer.materials[0] = onMat;
                }
                while (true)
                {
                    if(time >= 1.1f)
                    {
                        meshRenderer.materials[0] = offMat;
                    }

                    if(time >= 1.5f)
                    {
                        if (random <= 40)
                        {
                            meshRenderer.materials[0] = onMat;
                        }
                        while (true)
                        {
                            if(time >= 1.75f)
                            {
                                meshRenderer.materials[0] = offMat;
                            }

                            if(time >= 3f)
                            {
                                if (random <= 90)
                                {
                                    meshRenderer.materials[0] = onMat;
                                }
                                while (true)
                                {
                                    if(time >= 3.3f)
                                    {
                                        meshRenderer.materials[0] = offMat;
                                    }
                                    if (time >= 3.45f)
                                    {
                                        meshRenderer.materials[0] = onMat;
                                    }
                                    StopCoroutine(startLight);
                                }
                                
                            }
                            yield return null;
                        }
                        
                    }
                    yield return null;
                }
                
            }
            yield return null;
        }
    }
}
