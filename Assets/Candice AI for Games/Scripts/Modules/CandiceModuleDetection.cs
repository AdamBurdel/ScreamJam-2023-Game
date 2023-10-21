using CandiceAIforGames.AI.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CandiceAIforGames.AI.Utils.Enums;

namespace CandiceAIforGames.AI
{
    public class CandiceModuleDetection:CandiceBaseModule
    {
        Transform transform;
        public Action<CandiceDetectionResults> objectDetectedCallback;
        List<GameObject> objects;
        int direction = 0; //0=left,1=right

        public CandiceModuleDetection(Transform transform, Action<CandiceDetectionResults> _objectDetectedCallback, string moduleName = "CandiceModuleDetection") : base(moduleName)
        {
            this.transform = transform;
            objectDetectedCallback = _objectDetectedCallback;
            Utils.Utils.LogClassInitialisation(this);
        }

        public void ScanForObjects(CandiceDetectionRequest request)
        {
            Vector3 center = transform.position;

            bool is3D = request.is3D;
            float radius = request.radius;
            float height = request.height;
            float lineOfSight = request.lineOfSight;
            SensorType type = request.type;
            //Array that will store all collided objects
            //Collider[] hitColliders = Physics.OverlapSphere(center, radius);

            Vector3 halfExtents = new Vector3(radius, height, radius);
            Collider[] hitColliders = null;
            if (type == SensorType.Sphere)
            {
                hitColliders = Physics.OverlapSphere(center, radius);
            }

            Dictionary<string, List<GameObject>> detectedObjects = new Dictionary<string, List<GameObject>>();
            //Loop though each object
            foreach (Collider collider in hitColliders)
            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(center, go.transform.position);
                float angle = Vector3.Angle(go.transform.position - center, transform.forward);
                if (angle <= lineOfSight / 2)
                {
                    CompareTags(go, request.detectionTags, ref detectedObjects);
                }
                
                

            }
            objectDetectedCallback(new CandiceDetectionResults(detectedObjects));
        }
        public void ScanForObjects2D(CandiceDetectionRequest request)
        {
            Vector3 center = transform.position;

            bool is3D = request.is3D;
            float radius = request.radius;
            float height = request.height;
            float lineOfSight = request.lineOfSight;
            SensorType type = request.type;

            //Array that will store all collided objects
            Collider2D[] hitColliders = null;
            Vector2 center2D = new Vector2(center.x, center.y);
            hitColliders = Physics2D.OverlapCircleAll(center, radius);
            Dictionary<string, List<GameObject>> detectedObjects = new Dictionary<string, List<GameObject>>();
            //Loop through each object
            foreach (Collider2D collider in hitColliders)
            {
                GameObject go = collider.gameObject;
                float distance = Vector3.Distance(center, go.transform.position);
                float angle = Vector2.Angle((new Vector2(go.transform.position.x, go.transform.position.y) - center2D), transform.forward);
                //Check if the object is in the enemy line of sight.
                if (angle <= lineOfSight / 2)
                {
                    CompareTags(go, request.detectionTags, ref detectedObjects);
                }

            }
            objectDetectedCallback(new CandiceDetectionResults(detectedObjects));
        }
        public void AvoidObstacles(Transform Target, Vector3 movePoint, Transform transform, float size, float movementSpeed, bool is3D, float maxDistance,int lines, LayerMask perceptionMask)
        {
            //
            //Method Name : void Move(Transform Target, Transform transform, float size)
            //Purpose     : This method moves the agent while avoiding immediate obstacles.
            //Re-use      : none
            //Input       : Transform Target, Transform transform, float size
            //Output      : void
            //
            if (!is3D)
            {
                AvoidObstacles2D(Target, movePoint, transform, size, movementSpeed, maxDistance, lines, perceptionMask);
                return;
            }
            bool obstacleHit = false;
            Vector3 dir = (transform.forward).normalized;
            RaycastHit hit;
            float distance = maxDistance;

            Vector3 center = transform.position;

            Vector3[] oaPoints = new Vector3[lines];
            float step = size / lines;
            float currentPos = transform.position.x - size;

            for(int i = 0; i < lines;i++)
            {
                oaPoints[i] = transform.position;

                oaPoints[i].x = currentPos;
                currentPos += step*2;
            }
            List<Vector3> lstNormals = new List<Vector3>();
            int countLeft = 0;
            int countRight = 0;

            for (int i = 0; i < oaPoints.Length;i++)
            {
                Vector3 point = oaPoints[i];
                if (i == 0 || i == oaPoints.Length - 1)
                {
                    distance = distance = maxDistance;
                }
                else
                {
                    distance = maxDistance / 2;
                }
                if (Physics.Raycast(point, transform.forward, out hit, maxDistance))
                {
                    if (hit.transform != transform && hit.transform != Target.transform)
                    {
                        if (HasLayer(perceptionMask, hit.transform.gameObject.layer))
                        {
                            Color color = Color.red;
                            if (i == 0)
                                color = Color.blue;
                            else if (i == oaPoints.Length - 1)
                                color = Color.green;
                            Debug.DrawLine(point, hit.point, color);
                            lstNormals.Add(hit.normal);
                            if(hit.normal.z < 0)
                            {
                                countRight = countRight + 1;
                                //direction = 1;
                            }
                            if(hit.normal.x < 0)
                            {
                                countLeft = countLeft + 1;
                                //direction = 0;
                            }
                            //dir += hit.normal * 90;
                            obstacleHit = true;
                        }
                        /*foreach (LayerMask region in walkableRegions)
                        {
                            int id = Convert.ToInt32(Mathf.Log(region.value, 2));

                            if(id == hit.transform.gameObject.layer)
                            {
                                Debug.DrawLine(point, hit.point, Color.red);
                                dir += hit.normal * 90;

                                obstacleHit = true;
                            }
                        }*/

                    }
                }
            }
            


            if(lstNormals.Count > 0)
            {
                if (countLeft > countRight)
                {
                    direction = 0;
                }
                else
                {
                    direction = 1;
                }
                bool isComplete = false;
                int index = 0;
                if (direction == 1)
                {
                    index = lstNormals.Count - 1;
                }

                while (!isComplete)
                {

                    dir += lstNormals[index] * 90;
                    if (direction == 1)
                    {
                        index = index - 1;
                        if (index < 0)
                        {
                            isComplete = true;
                        }
                    }
                    else
                    {
                        index = index + 1;
                        if (index > lstNormals.Count - 1)
                        {
                            isComplete = true;
                        }
                    }


                }
            }
            


            
            if (lstNormals.Count > 1)
            {
                //Debug.Log("First: " + lstNormals[0].ToString());
                //Debug.Log("Last: " + lstNormals[lstNormals.Count - 1].ToString());
                //Debug.Log("Direction: " + direction);
            }
            
            if (obstacleHit)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, movementSpeed * Time.deltaTime);
            }
            else
            {
                movePoint = new Vector3(movePoint.x, transform.position.y, movePoint.z);
                dir = (movePoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, movementSpeed * Time.deltaTime);
            }
            
        }

        public static bool HasLayer(LayerMask layerMask, int layer)
        {
            if (layerMask == (layerMask | (1 << layer)))
            {
                return true;
            }

            return false;
        }


        private bool compareObjToTags(GameObject obj, List<string> tags)
        {
            //Check if the object is in the tag list
            bool hasMatch = false;

            for (int i = 0; i < tags.Count; i++)
            {
                if(obj.tag.Equals(tags[i]))
                {
                    hasMatch = true;
                    i = tags.Count;
                }
            }

            return hasMatch;
        }

        public void AvoidObstacles2D(Transform Target, Vector3 movePoint, Transform transform, float size, float movementSpeed, float distance, int lines, LayerMask perceptionMask)
        {

            bool obstacleHit = false;
            Vector2 dir = (Target.position - transform.position).normalized;
            RaycastHit2D hit;

            Vector3 center = transform.position;

            Vector3[] oaPoints = new Vector3[lines];
            float step = size / lines;
            float currentPos = transform.position.x - size;

            for (int i = 0; i < lines; i++)
            {
                oaPoints[i] = transform.position;

                oaPoints[i].x = currentPos;
                currentPos += step * 2;
            }
            for (int i = 0; i < oaPoints.Length; i++)
            {
                Vector3 point = oaPoints[i];
                if (i >= oaPoints.Length / 2 && i <= oaPoints.Length / 2 + 1)
                {
                    distance = distance * 1.5f;
                }
                hit = Physics2D.Raycast(transform.position, transform.forward, distance);
                Debug.Log("OA 2D");
                if (hit.transform != transform && hit.transform != Target.transform)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                    dir += hit.normal * 50;
                    obstacleHit = true;
                }
            }
            if (obstacleHit)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, movementSpeed * Time.deltaTime);
            }
            else
            {
                movePoint = new Vector3(movePoint.x, transform.position.y, movePoint.z);
                dir = (movePoint - transform.position).normalized;
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, movementSpeed * Time.deltaTime);
            }







            //Quaternion rot = Quaternion.LookRotation(dir);
            //transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
            //transform.position += new Vector3(dir.x, dir.y) * movementSpeed * Time.deltaTime;


        }
        public void CompareTags(GameObject go, List<string> detectionTags, ref Dictionary<string, List<GameObject>> detectedObjects)
        {
            objects = new List<GameObject>();

            if (detectionTags.Contains(go.tag))
            {
                if(detectedObjects.ContainsKey(go.tag))
                {
                    detectedObjects[go.tag].Add(go);
                }
                else
                {
                    List<GameObject> objects = new List<GameObject>();
                    objects.Add(go);
                    detectedObjects.Add(go.tag, objects);
                }
            }
        }

    }
    
    public struct CandiceDetectionRequest
    {
        public SensorType type;
        public List<string> detectionTags;
        public float radius;
        public float height;
        public float lineOfSight;
        public bool is3D;

        public CandiceDetectionRequest(SensorType type, List<string> detectionTags, float radius, float height, float lineOfSight, bool is3D)
        {
            this.type = type;
            this.detectionTags = detectionTags;
            this.radius = radius;
            this.height = height;
            this.lineOfSight = lineOfSight;
            this.is3D = is3D;
        }
    }
    public struct CandiceDetectionResults
    {
        public Dictionary<string,List<GameObject>> objects;

        public CandiceDetectionResults(Dictionary<string, List<GameObject>> objects)
        {
            this.objects = objects;
        }
    }
}

