using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;

namespace ProjectGuardian
{
    public class AIMove : MonoBehaviour
    {
        public Transform target;
        AIPath aiPath;
        Camera cam;
        Vector3 newMovePosition;


        // Start is called before the first frame update
        void Awake()
        {
            aiPath = GetComponent<AIPath>();
            cam = Camera.main;
            newMovePosition = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            // AI pathfinding with target object
            SetMovePosition(target.position);

            if (Input.GetMouseButtonDown(0) && false)
            {

                Debug.Log("Pressed primary button.");

                // For 2D
                //newMovePosition = cam.ScreenToWorldPoint(Input.mousePosition);
                //newMovePosition.z = 0;

                // For 3D
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Debug.Log("Mouse position: " + Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    newMovePosition = hit.point;
                    SetMovePosition(newMovePosition);
                    Debug.Log("New target position set by rayhit: " + newMovePosition);

                    //Transform objectHit = hit.transform;
                    // Do something with the object that was hit by the raycast.
                }

            }

            if (transform.eulerAngles.z != 0)
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            }


        }

        void SetMovePosition(Vector3 movePosition)
        {
            aiPath.destination = movePosition;
        }

        void OnCollisionEnter(Collision collision)
        {
            Debug.DrawRay(collision.contacts[0].point, collision.contacts[0].normal, Color.green, 2, false);
        }
    }
}

