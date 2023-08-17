using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldCollision : MonoBehaviour, IPointerClickHandler
{
   private Material _material;
   [SerializeField] private bool isUI;
   
    void Start()
   {
       if (isUI)
      {
          _material = GetComponent<Image>().material;
      }
      else
      {
          _material = GetComponent<MeshRenderer>().material;
      }
   }
   
    void OnCollisionEnter(Collision co)
   {
       Debug.Log("Sphere Pos change");
       _material.SetFloat("_Hardness", 0);
       _material.SetVector("_SpherePos", co.transform.position);
       // Give me code to wait for two seconds
       StartCoroutine(Wait());
   }
    
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        _material.SetVector("_SpherePos", new Vector3(0, 0, 0));
        if (isUI)
        {
            _material.SetVector("_SpherePos", new Vector3(575.78f, 0, 0)); 
        }
        _material.SetFloat("_Hardness", 1);
    }

    private void Update()
    {
        /*if (Input.GetMouseButtonDown(0) && isUI)
        {
            CastClickRay();
        }*/
    }
    
    private void CastClickRay()
    {
        var mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y,
            Camera.main.nearClipPlane));

        if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject == gameObject)
        {
            Debug.Log("Click on obj");
            _material.SetFloat("_Hardness", 0);
            _material.SetVector("_SpherePos", hit.point);
            // Give me code to wait for two seconds
            StartCoroutine(Wait());
            this.gameObject.GetComponent<Dissolve>().isdissolving = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 clickPosition = eventData.position;

        // You can now use the clickPosition to perform actions based on the click position
        // For example, you can convert it to world space if needed:
        //Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        
       Debug.Log("Click on obj");
       _material.SetFloat("_Hardness", 0);
       _material.SetVector("_SpherePos", clickPosition);
       // Give me code to wait for two seconds
       StartCoroutine(Wait());

    }
}
