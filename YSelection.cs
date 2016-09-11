using UnityEngine;
using System.Collections;

public class YSelection : MonoBehaviour
{    
    public LayerMask touchMask;
    private RaycastHit hit;
    public Camera cam;
    public float speed;
    public Vector3 u;
    public float qfac;    

    void Start()
    {
        cam = this.GetComponent<Camera>();
        u = new Vector3(-1, -1, -1);
        speed = 20f;
        qfac = -1;    
    }    

    public CbScript backspaceFunctionScript;
    
    void Update()
    {
        // Saving an Update Cycle if this is in cbcycle..now changed to here
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backspaceFunctionScript.ZShowQuit(1);            
        }

#if UNITY_EDITOR
            if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)
            && (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())            
            )
        {            
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);            
            if (Physics.Raycast(ray, out hit, touchMask))
            {                
                GameObject receiver = hit.transform.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    u = hit.point;                    
                }
                else if (Input.GetMouseButton(0))
                {
                    //Vector3 v = Input.mousePosition;
                    //float ext = qfac * speed * Time.deltaTime;
                    //Vector3 result = new Vector3(1,1,1);
                    //transform.Translate( (v.x-u.x) * ext , 0 , (v.y - u.y) * ext );                    
                    //transform.Rotate( 0 , 1 , 0);
                    //u = v ;
                }
                else// Up
                {
                    if (u == hit.point)//Same place released
                    {
                        receiver.SendMessage("onClick", hit.point, SendMessageOptions.DontRequireReceiver);
                    }
                    //else Drag
                }
            }
        }
#endif

        if (Input.touchCount == 1 )//changed from > 0 to ==1
        {
            foreach (Touch touch in Input.touches)
            {
                if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject( touch.fingerId ))
                {

                    Ray ray = cam.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out hit, touchMask))
                    {
                        GameObject receiver = hit.transform.gameObject;
                        /*
                        if touch began and ended at the same point + or - some offset => CLICK

                        if touch began and stationary => moving tends to zero
                        if touch began and moving => drag if one finger and rotate if 2 fingers and adjust height

                        if touch canceled then DO NOTHING

                        */
                        if (touch.phase == TouchPhase.Began)
                        {
                            u = hit.point;
                        }
                        else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                        {
                            //receiver.SendMessage("onTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);                        
                            //transform.Translate(1 * Time.deltaTime, 0, 0);
                            //DRAG
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            if (u == hit.point)//Same place released
                            {
                                receiver.SendMessage("onClick", hit.point, SendMessageOptions.DontRequireReceiver);
                            }
                            //else Drag
                        }
                        else
                        {
                            //receiver.SendMessage("onTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
                        }
                    }                    
                }
            }
        }
    }
}
