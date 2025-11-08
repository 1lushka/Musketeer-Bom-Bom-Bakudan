using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{

    public LayerMask planeLayerMask;
    public string planeTag = "GamePlane";

    public UnityEvent<Vector3> OnPlaneClick; //здесь будет проще через юнитевский ивент сделать 

    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        // лкм + если не паузы - тогда обрабатываем клик
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Paused &&
            Input.GetMouseButtonDown(0))
        {
            HandlePlaneClick();
        }
    }

    void HandlePlaneClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, planeLayerMask))
        {
            
            if (hit.collider.CompareTag(planeTag))
            {
                Vector3 worldHitPoint = hit.point;

                OnPlaneClick?.Invoke(worldHitPoint); // отсылаю позицию к стрельбе
            }
        }
    }
}