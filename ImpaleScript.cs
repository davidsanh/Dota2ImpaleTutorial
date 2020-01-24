using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpaleScript : MonoBehaviour
{
    //Visualization
    public bool Debug;

    //Particle system to create upon hitting an enemy
    public GameObject impaleHitFX;
    //The maximum amount of impale objects that will be created
    public int maximumLength;
    //The separation in between impale objects
    public float separation;
    //The delay before spawning our impale objects
    public float spawnDelay;
    //The delay before we call our damaging function
    public float damageDelay;
    //The height from were our raycast will start
    public float height;
    //The radius of our damaging function
    public float radius;
    //Force upwards pass
    public float force;
    //Offset fix for our object
    public float yOffset;
    //The layer for our raycast
    public LayerMask layerMask;

    //The parent gameobject that will contain all the impale objects
    [HideInInspector]
    public GameObject fxParent;
    //The current value of our impale object
    [HideInInspector]
    public int currentLength;
    //Bools required to stop loops and destroy our parent object
    private bool isLast = false, hasSpawnedNext = false, hasDamaged;
    //The particle system we created
    private ParticleSystem PS;
    //Our delay timers
    private float spawnDelayTimer, damageDelayTimer;

    private void Start()
    {
        //We check if this is the first impale object and set it as the main parent to destroy later
        if (currentLength == 0) { fxParent = gameObject; }

        //Get reference to our particle system
        PS = GetComponent<ParticleSystem>();

        //Set the delays
        spawnDelayTimer = spawnDelay;
        damageDelayTimer = damageDelay;
    }

    private void Update()
    {
        //If delay timer is smaller or equal to zero and it has not spawned next OBJ then we launch our creater impale function.
        if (spawnDelayTimer <= 0 && !hasSpawnedNext) { CreateImpaleOBJ(); }

        //If delay timer is bigger than 0 then we lower it.
        if (spawnDelayTimer > 0) { spawnDelayTimer -= Time.deltaTime; }

        //If delay timer is bigger than 0 then we lower it.
        if (damageDelayTimer > 0) { damageDelayTimer -= Time.deltaTime; }

        //If delay timer is smaller or equal to zero and it has not spawned next OBJ then we launch our creater impale function.
        if (damageDelayTimer <= 0 && !hasDamaged) { AOEDamage(); }

        //If this is the last impale object and our particle has finished playing then we destroy the parent object and all impale objects with it.
        if (!PS.isPlaying && isLast) { Destroy(fxParent); }
    }

    void CreateImpaleOBJ()
    {
        //We first check if our current obj is not bigger than the maximum amount of impale objects permitted
        if (currentLength < maximumLength)
        {
            //Get spawn position
            var raycastPosition = transform.position + transform.forward * separation;
            //Add our height value so that the raycast is not created on the floor
            raycastPosition.y += height;
            RaycastHit hit;
            if (Physics.Raycast(raycastPosition, Vector3.down, out hit, height + 1, layerMask))
            {
                //Use name or tag to check floor
                if (hit.transform != transform)
                {
                    //Get the spawn hit location
                    var spawnLoc = hit.point;
                    //Add extra units to our Y offset to correct visuals
                    spawnLoc.y += yOffset;
                    //Set our bool to avoid looping and creating more than the required impale objects
                    hasSpawnedNext = true;
                    //Create next impale object and set it as a child of this transform
                    var obj = Instantiate(gameObject, transform);
                    //Fix its location
                    obj.transform.position = spawnLoc;
                    obj.transform.rotation = transform.rotation;
                    //Set our temporary impale variable
                    var impale = obj.GetComponent<ImpaleObject>();
                    //Add one to the current lenght
                    impale.currentLength = currentLength + 1;
                    //We make sure our maximum lenght is correct
                    impale.maximumLength = maximumLength;
                    //Set the main parent to later destroy it
                    impale.fxParent = fxParent;
                }
                else { isLast = true; }    //If we did not hit anything using our tags and names then this is the last impale obj
            }
            else { isLast = true; }                //If our raycast did not hit anything then this is the last impale obj
        }
        else { isLast = true; }                    //If this current impale object value equals the maximum lenght then this is the last impale obj
    }

    void AOEDamage()
    {
        //Set damage bool to avoid damaging unit endlessly
        hasDamaged = true;

        //Create a sphere around location using our radius
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);

        //We get all colliders that overlap our sphere cast
        foreach (Collider col in objectsInRange)
        {
            //We get the enemies within range that contain a rigidbody
            Rigidbody enemy = col.GetComponent<Rigidbody>();

            //We check if enemy has been found
            if (enemy != null)
            {
                //We add a force upwards to the rigidbody
                enemy.AddForce(Vector3.up * force, ForceMode.VelocityChange);

                //We create our impale fx hit
                var fx = Instantiate(impaleHitFX, enemy.transform.position, Quaternion.identity);

                //We destroy the fx on a delay depending on the duration of our fx
                Destroy(fx, 2);

                //You can also call your damaging script here
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Debug)
        {
            //Draw visualization of our damaging area
            if (hasDamaged)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position, radius);
            }
        }
    }
}
