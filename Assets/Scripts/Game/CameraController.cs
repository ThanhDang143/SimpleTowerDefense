using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Space]
    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float moveSpeed = 100f;

    public void Setup()
    {
        if (cam == null) GetComponent<Camera>();
        cam.orthographicSize = GameManager.Instance.GetCellSize() * 5f;
    }

    public void ManualUpdate()
    {
        ZoomCamera();
        MoveCamera();
    }

    private void MoveCamera()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            cam.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            cam.transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            cam.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            cam.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }
    }

    private void ZoomCamera()
    {
        float camSize = cam.orthographicSize;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            camSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize - zoomSpeed, Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            camSize = Mathf.Lerp(cam.orthographicSize, cam.orthographicSize + zoomSpeed, Time.deltaTime);
        }

        cam.orthographicSize = Mathf.Clamp(camSize, 10f, 100f);
    }
}
