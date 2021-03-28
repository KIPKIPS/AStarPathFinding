using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {
    public Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    public PathRequest curPathRequest;
    static PathRequestManager instance;

    PathFinding pathFinding;
    private bool isProcessingPath;

    void Awake() {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //Debug.Log(instance == null);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    public void TryProcessNext() {
        if (!isProcessingPath && pathRequestQueue.Count > 0) {
            curPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(curPathRequest.pathStart, curPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        curPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    public struct PathRequest {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
