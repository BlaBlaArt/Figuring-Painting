using UnityEngine;

public class CoinMove : MonoBehaviour
{

    ///////////////  VARS FOR 3D  MOVEMENT
    
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private string _targetTag;

    private Transform _targetTransform = null;

    
    ///////////////  VARS FOR UI MOVEMENT 
  
    private Transform _coinUITransform;
    private Vector3 _coinPoint;
    private Vector3 _targetPosition;

    
    
    
    private void Awake()
    {
        _targetTransform = GameObject.FindGameObjectWithTag(_targetTag).transform;   // Can be another tag or find with T component 
        SetUICoordTo3D();
    }

    private void Update()
    {
        if (_targetTransform != null)
        {

            ///////////////  USE THIS  FOR 3D  MOVEMENT 
            if ((_targetTransform.position - transform.position).sqrMagnitude < (_distance * _distance)) // Distance == 5 , you can change this value from inspector
            {
                transform.position = Vector3.Lerp(transform.position, _targetTransform.position,
                    Time.deltaTime * _moveSpeed);
            }
            
            
            
            ///////////////  USE THIS  FOR   MOVEMENT FROM 3D TO UI 
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 5);
            if ((_targetPosition - transform.position).sqrMagnitude < (_distance * _distance))
            {
                gameObject.SetActive(false);
                // YOU LOGIC 
            }
        }
    }

    public void SetUICoordTo3D()
    {
        _coinUITransform = GameObject.FindWithTag("CoinUI").transform;     // USE YOUR TAG 
        _coinPoint = _coinUITransform.position + new Vector3(0f, 0f, 5f);
        _targetPosition = Camera.main.ScreenToWorldPoint(_coinPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag)) {
            Destroy(gameObject);
        }
    }

}
