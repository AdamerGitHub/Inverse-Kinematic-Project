using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InverseKinematicHand : MonoBehaviour
{
    public int chainLength = 2;

    public Transform target;
    public Transform pole;

    public int iterations = 10;

    public float delta = 0.001f;

    [Range(0, 1)]
    public float snapBackStrength;

    protected float[] bonesLength;
    protected float completeLength;
    protected Transform[] bones;
    protected Vector3[] positions;
    protected Vector3[] startDirectionSucc;
    protected Quaternion[] startRotationBone;
    protected Quaternion startRotationTarget;
    protected Quaternion startRotationRoot;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirectionSucc = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        //init fields
        if(target == null)
        {
            target = new GameObject(gameObject.name + " Target").transform;
            target.position = transform.position;
        }
        startRotationTarget = target.rotation;
        completeLength = 0;

        //init data
        Transform current = transform;
        for(int i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if(i == bones.Length - 1)
            {
                //leaf
                startDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                //mid bone
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = (bones[i + 1].position - current.position).magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
            return;

        if (bonesLength.Length != chainLength)
            Init();

        // Get positions
        for(int i = 0; i < bones.Length; i++)
        {
            positions[i] = bones[i].position;
        }

        Quaternion rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        Quaternion rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        //calculation
        if((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            // get target direction by first Joint
            Vector3 direction = (target.position - positions[0]).normalized;
            // set Joints position by target direction
            for(int i = 1; i < positions.Length; i++)
            {
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + rootRotDiff * startDirectionSucc[i], snapBackStrength);

            for(int iteration = 0; iteration < iterations; iteration++)
            {
                //back
                for(int i  = positions.Length - 1; i > 0; i--)
                {
                    if(i == positions.Length - 1)
                        positions[i] = target.position; // set it to target
                    else
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i]; // set in line on distance
                }
                    
                //forward
                for (int i = 1; i < positions.Length; i++)
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];

                //close
                if ((positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta)
                    break;
            }
        }

        //move towards pole
        if (pole != null)
        {
            for(int i = 1; i < positions.Length - 1; i++)
            {
                Plane plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                Vector3 projectedPole = plane.ClosestPointOnPlane(pole.position);
                Vector3 projectedBone = plane.ClosestPointOnPlane(positions[i]);
                float angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        // Set positions
        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
            bones[i].position = positions[i];
        }
    }

    void OnDrawGizmos()
    {
        Transform current = this.transform;
        for(int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            float scale = Vector3.Distance(current.position, current.parent.position) * 0.2f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
