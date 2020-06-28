using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    [HideInInspector]
    public float smoothing = 7f;
    RaycastHit  hit;
    List<Transform> selectedUnits = new List<Transform>();
    bool isDragging = false;
    Vector3 mousePosition;


    private void OnGUI() {

        if(isDragging){
        
        Rect rect = ScreenHelper.GetScreenRect(mousePosition,Input.mousePosition);
        ScreenHelper.DrawScreenRect(rect,new Color(0.8f,0.8f,0.95f,0.1f));
        ScreenHelper.DrawScreenRectBorder(rect,1,Color.white);
        }
        
    }


    Ray camRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    // Update is called once per frame
    void Update()
    {

    if(Input.GetMouseButtonDown(0)){
        mousePosition =  Input.mousePosition;
        isDraggingBool();
        if(Physics.Raycast(camRay, out hit)){
            Debug.Log(hit.transform.tag);
            if(hit.transform.CompareTag("Destination")){
                SelectUnit(hit.transform, Input.GetKey  (KeyCode.LeftShift));
            }
        
    }
    }
    if(Input.GetMouseButtonUp(0)){
        
        DeselectUnits();
        x();
        isDraggingBool();
        
        
    }

    if(Input.GetMouseButtonDown(1)){
        mousePosition = Input.mousePosition;

        if(Physics.Raycast(camRay, out hit)){
            Debug.Log(hit.transform.tag);
            if(hit.transform.CompareTag("Untagged")){
                Vector3 newTarget = hit.point + new Vector3(0,0.5f,0);
                StartCoroutine("Movement",newTarget);
            }
            else{
                isDragging = true;
            }          
        
    }
    }
    }

    void x(){

         foreach(var selectableObject in FindObjectsOfType<BoxCollider>())
        {
            if(isWithinSelectionBounds(selectableObject.transform))
            {
                Debug.Log("geldi");
                SelectUnit(selectableObject.transform, true);   
            }
        }
    }
    void isDraggingBool(){ isDragging = !isDragging; }
    private void SelectUnit(Transform unit, bool isMultiSelect = false){
        
    if(!isMultiSelect){
       DeselectUnits();
    }
    selectedUnits.Add(unit);
    unit.Find("Cube").gameObject.SetActive(true);
    
}

    private void DeselectUnits(){
        for(int i=0;i<selectedUnits.Count;i++){
            selectedUnits[i].Find("Cube").gameObject.SetActive(false);
        }
         selectedUnits.Clear();
    }
    private bool isWithinSelectionBounds(Transform transform){
        if(!isDragging){
            return false;
        }
        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera,mousePosition,Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }

    IEnumerator Movement(Vector3 target)
    {
        Debug.Log("log");

             for(int i=0;i<selectedUnits.Count;i++){
            selectedUnits[i].transform.position = Vector3.Lerp(transform.position,target,smoothing*Time.deltaTime);
            yield return null;
        }

    }

}

