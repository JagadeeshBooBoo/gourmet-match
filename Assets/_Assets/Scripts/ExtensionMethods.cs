using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class ExtensionMethods
{
    //Even though they are used like normal methods, extension
    //methods must be declared static. Notice that the first
    //parameter has the 'this' keyword followed by a Transform
    //variable. This variable denotes which class the extension
    //method becomes a part of.
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
         float multiplier = 1;
         for (int i = 0; i < decimalPlaces; i++)
         {
             multiplier *= 10f;
         }
         return new Vector3(
             Mathf.Round(vector3.x * multiplier) / multiplier,
             Mathf.Round(vector3.y * multiplier) / multiplier,
             Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    public static int Zero(this int num,int places = 2){
        int multiplier = 1;
         for (int i = 0; i < places; i++)
         {
             multiplier *= 10;
         }
         return ( (num/multiplier) * multiplier);
    }

    public static float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }

    public static Vector3 Average(this Vector3[] arr){
        int len = arr.Length;
        Vector3 sum = Vector3.zero;
        for(int i=0;i<len;i++){
            sum+=arr[i];
        }
        return sum/len;
    }

    public static Vector3[] SimpleMovingAverage(this Vector3[] data, int size)
    {   
        if(size%2==0){size--;}
        if(size<=1)return data;
        if(data.Length<size)return data;
        Vector3[] filter = new Vector3[data.Length];
        data.CopyTo(filter,0);
        Vector3 sum = Vector3.zero;
        for(int i=size/2;i<data.Length - (size/2);i++){
            sum=Vector3.zero;
            for(int j=-size/2;j<=size/2;j++){
                sum+=data[i+j];
            }
            filter[i] = sum/size;
        }

        return filter;
    }

    public static Vector3 Min(this Vector3[] arr){
        int arrLen = arr.Length;

        if(arrLen==0)
            Debug.LogError("Vector is empty");

        Vector3 minVector=arr[0];

        for(int i=0;i<arrLen;i++){
            if(arr[i].y<minVector.y){
                minVector.y = arr[i].y;
            }
            if(arr[i].x<minVector.x){
                minVector.x = arr[i].x;
            }
        }

        return minVector;
    }

    public static LTDescr moveLeft(RectTransform rt,float leftOffset,float transitionTime){
        return LeanTween.moveX(rt,rt.anchoredPosition.x -leftOffset,transitionTime).setEaseInOutBack();
    }

    public static LTDescr moveRight(RectTransform rt,float rightOffset,float transitionTime){
        return LeanTween.moveX(rt,rt.anchoredPosition.x +rightOffset,transitionTime).setEaseInOutBack();
    }

    public static LTDescr moveUp(RectTransform rt,float upOffset,float transitionTime){
        return LeanTween.moveY(rt,rt.anchoredPosition.y +upOffset,transitionTime).setEaseInOutBack();
    }

    public static LTDescr moveDown(RectTransform rt,float downOffset,float transitionTime){
        return LeanTween.moveY(rt,rt.anchoredPosition.y -downOffset,transitionTime).setEaseInOutBack();
    }

    public static bool RectOverlaps(this RectTransform rectTrans1, RectTransform rectTrans2)
    {
        Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        return rect1.Overlaps(rect2);
    }

    public static void RecursiveFindRenderers(Transform t,ref List<Transform> transList){
        Renderer r = t.gameObject.GetComponent<Renderer>();
        if(r!=null){
            transList.Add(t);
        }
        foreach(Transform tr in t){
            RecursiveFindRenderers(tr,ref transList);
        }
    }

    public static Bounds GetRendererBounds(Transform t){
        Bounds b = new Bounds();
        b.center = t.position;
        Renderer r = t.GetComponent<Renderer>();
        if(r!=null)
            b.Encapsulate(r.bounds);
        foreach(Transform tr in t){
            b.Encapsulate(GetRendererBounds(tr));
        }
        return b;
    }

    public static Bounds GetColliderBounds(Transform t){
        Bounds b = new Bounds();
        b.center = t.position;
        Collider r = t.GetComponent<Collider>();
        if(r!=null)
            b.Encapsulate(r.bounds);
        foreach(Transform tr in t){
            b.Encapsulate(GetColliderBounds(tr));
        }
        return b;
    }

    public static void SetLayerRecursive(GameObject g, int layerid){
        g.layer = layerid;
        if(g.transform.childCount==0)return;
        foreach(Transform t in g.transform){
            SetLayerRecursive(t.gameObject,layerid);
        }
    }

    public static Vector3 ClampPosition(this Vector3 pos, Vector3 minPos, Vector3 maxPos){
        Vector3 tempPos = pos;
        tempPos.x = Mathf.Clamp(tempPos.x,minPos.x,maxPos.x);
        tempPos.y = Mathf.Clamp(tempPos.y,minPos.y,maxPos.y);
        tempPos.z = Mathf.Clamp(tempPos.z,minPos.z,maxPos.z);
        pos = tempPos;
        return pos;
    }

    public static LTDescr newMoveAbsolute(RectTransform rt,Vector2 absoluteBottomLeftPos,float _transitionTime,System.Action callback = null){
        Vector2 startAnchorMin = rt.anchorMin;
        Vector2 startAnchorMax = rt.anchorMax;
        Vector2 targetAnchorMin = absoluteBottomLeftPos;
        Vector2 targetAnchorMax = absoluteBottomLeftPos + Vector2.one;
        if(_transitionTime==0){
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            // rt.anchoredPosition = Vector2.zero;
            if(callback!=null)callback();
            return null;
        }
        return LeanTween.value(rt.gameObject,0,1f,_transitionTime).setOnUpdate((float val)=>{
            rt.anchorMin = Vector2.LerpUnclamped(startAnchorMin,targetAnchorMin,val);
            rt.anchorMax = Vector2.LerpUnclamped(startAnchorMax,targetAnchorMax,val);
            // rt.anchoredPosition = Vector2.zero;
        })
        .setOnComplete(()=>
        {
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            if(callback!=null)callback();
        });
    }

    public static LTDescr newMoveLeft(RectTransform rt,float offset,float _transitionTime,System.Action callback = null){
        Vector2 startAnchorMin = rt.anchorMin;
        Vector2 startAnchorMax = rt.anchorMax;
        Vector2 targetAnchorMin = rt.anchorMin + Vector2.left * offset;
        Vector2 targetAnchorMax = rt.anchorMax + Vector2.left * offset;
        if(_transitionTime==0){
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            // rt.anchoredPosition = Vector2.zero;
            if(callback!=null)callback();
            return null;
        }
        return LeanTween.value(rt.gameObject,0,1f,_transitionTime).setOnUpdate((float val)=>{
            rt.anchorMin = Vector2.LerpUnclamped(startAnchorMin,targetAnchorMin,val);
            rt.anchorMax = Vector2.LerpUnclamped(startAnchorMax,targetAnchorMax,val);
            // rt.anchoredPosition = Vector2.zero;
        })
        .setOnComplete(()=>
        {
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            if(callback!=null)callback();
        });
    }

    public static LTDescr newMoveRight(RectTransform rt,float offset,float _transitionTime,System.Action callback = null){
        Vector2 startAnchorMin = rt.anchorMin;
        Vector2 startAnchorMax = rt.anchorMax;
        Vector2 targetAnchorMin = rt.anchorMin + Vector2.right * offset;
        Vector2 targetAnchorMax = rt.anchorMax + Vector2.right * offset;
        if(_transitionTime==0){
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            // rt.anchoredPosition = Vector2.zero;
            if(callback!=null)callback();
            return null;
        }
        return LeanTween.value(rt.gameObject,0,1f,_transitionTime).setOnUpdate((float val)=>{
            rt.anchorMin = Vector2.LerpUnclamped(startAnchorMin,targetAnchorMin,val);
            rt.anchorMax = Vector2.LerpUnclamped(startAnchorMax,targetAnchorMax,val);
            // rt.anchoredPosition = Vector2.zero;
        })
        .setOnComplete(()=>
        {
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            if(callback!=null)callback();
        });
    }

    public static LTDescr newMoveUp(RectTransform rt,float offset,float _transitionTime,System.Action callback = null){
        Vector2 startAnchorMin = rt.anchorMin;
        Vector2 startAnchorMax = rt.anchorMax;
        Vector2 targetAnchorMin = rt.anchorMin + Vector2.up * offset;
        Vector2 targetAnchorMax = rt.anchorMax + Vector2.up * offset;
        if(_transitionTime==0){
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            // rt.anchoredPosition = Vector2.zero;
            if(callback!=null)callback();
            return null;
        }
        return LeanTween.value(rt.gameObject,0,1f,_transitionTime).setOnUpdate((float val)=>{
            rt.anchorMin = Vector2.LerpUnclamped(startAnchorMin,targetAnchorMin,val);
            rt.anchorMax = Vector2.LerpUnclamped(startAnchorMax,targetAnchorMax,val);
            // rt.anchoredPosition = Vector2.zero;
        })
        .setOnComplete(()=>
        {
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            if(callback!=null)callback();
        });
    }

    public static LTDescr newMoveDown(RectTransform rt,float offset,float _transitionTime,System.Action callback = null){
        Vector2 startAnchorMin = rt.anchorMin;
        Vector2 startAnchorMax = rt.anchorMax;
        Vector2 targetAnchorMin = rt.anchorMin + Vector2.down * offset;
        Vector2 targetAnchorMax = rt.anchorMax + Vector2.down * offset;
        if(_transitionTime==0){
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            // rt.anchoredPosition = Vector2.zero;
            if(callback!=null)callback();
            return null;
        }
        return LeanTween.value(rt.gameObject,0,1f,_transitionTime).setOnUpdate((float val)=>{
            rt.anchorMin = Vector2.LerpUnclamped(startAnchorMin,targetAnchorMin,val);
            rt.anchorMax = Vector2.LerpUnclamped(startAnchorMax,targetAnchorMax,val);
            // rt.anchoredPosition = Vector2.zero;
        })
        .setOnComplete(()=>
        {
            rt.anchorMin = targetAnchorMin;
            rt.anchorMax = targetAnchorMax;
            if(callback!=null)callback();
        });
    }

    public static void RecursiveFind<T>(Transform transform,ref List<T> transList){
        T tt = transform.gameObject.GetComponent<T>();
        if(!tt.Equals(null)){
            transList.Add(tt);
        }
        foreach(Transform tr in transform){
            RecursiveFind(tr,ref transList);
        }
    }

 public static float GetClipTime(string clipName,Animator animator){
        foreach(AnimationClip clip in animator.runtimeAnimatorController.animationClips){
            if(clip.name==clipName){
                return clip.length;
            }
        }
        return 0;
    }

    public static Vector3 GetInterpolatedNormal(RaycastHit hit){
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return hit.normal;
        }

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = hit.barycentricCoordinate;

        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;

        // Transform local space normals to world space
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);
        // Debug.DrawRay(hit.point, interpolatedNormal);
        return interpolatedNormal;
    }

    public static void TweenToTransform(this Transform t, Transform target)
    {
    }

    public static LTDescr QuaternionTweenLocal(GameObject g,Quaternion targetRot,float transitionTime){
        Quaternion startRotation = g.transform.localRotation;
        return LeanTween.value(g,0,1,transitionTime).setOnUpdate((float t)=>{
            g.transform.localRotation = Quaternion.SlerpUnclamped(startRotation,targetRot,t);
        });
    }

    // public static LTDescr ParabolaTween(Transform t,Vector3 startPos,Vector3 endPos,float height,float transitionTime){
    //     return LeanTween.value(t.gameObject,0,1,transitionTime).setOnUpdate((float val)=>{
    //         t.position = MathParabola.Parabola(startPos,endPos,height,val);
    //     });
    // }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static Vector3 GetLaunchVelocity(Vector3 startPos, Vector3 endPos,float angle, out float flightTime)
    {
        float g = Physics.gravity.y;
        Vector3 offset = endPos - startPos;
        float deltaY = offset.y;
        offset.y = 0;
        float deltaX = offset.magnitude;

        //if angle == 45 vx=vy
        //and 
        // float vx = vy = deltaX * Mathf.Sqrt(g/(2*(deltaY-deltaX)));

        float angDegrees = angle*Mathf.Deg2Rad;

        float cos = Mathf.Cos(angDegrees); 
        float sin = Mathf.Sin(angDegrees); 
        float tan = Mathf.Tan(angDegrees); 
        float v = deltaX * Mathf.Sqrt(g/(2*cos*cos*(deltaY-deltaX*tan)));
        float vx = v*cos;
        float vy = v*sin;

        Vector3 launchVelocity = new Vector3(vx, vy, 0);
        launchVelocity = Quaternion.Euler(0,Vector3.SignedAngle(Vector3.right, offset,Vector3.up),0) * launchVelocity;
        flightTime = deltaX/vx;

        return launchVelocity;
    }
}
   