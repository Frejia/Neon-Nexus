using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsInView : MonoBehaviour
{
    private Camera camera;
    private MeshRenderer renderer;
    Plane[] cameraFrustum;
    private Collider collider;
    [SerializeField] private int range;
    
    public delegate void EnemyInScreen(GameObject enemy);
    public static event EnemyInScreen OnEnemyInScreen;
    public static event EnemyInScreen OnEnemyNotInScreen;
    
    private void Start()
    {
        camera = Camera.main;
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    void OnEnemyVisible()
    {
        var bounds = collider.bounds;
        cameraFrustum = GeometryUtility.CalculateFrustumPlanes(camera);
        if (GeometryUtility.TestPlanesAABB(cameraFrustum, bounds))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (Vector3.Distance(player.transform.position, this.gameObject.transform.position) < range)
            {
                Debug.Log(gameObject.name + " is visible");
               // renderer.material.color = Color.green;
               if(OnEnemyNotInScreen != null)
                OnEnemyInScreen(this.gameObject);
            }
            else
            {
                //renderer.material.color = Color.red;
                if(OnEnemyNotInScreen != null)
                OnEnemyNotInScreen(this.gameObject);
            }
        }
        else
        {
            //renderer.material.color = Color.red;
            OnEnemyNotInScreen(this.gameObject);
        }
    }

    private void Update()
    {
        OnEnemyVisible();
    }

}
