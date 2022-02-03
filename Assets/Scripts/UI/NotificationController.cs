using UnityEngine;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    private float _timer;
    private bool _active;
    
    [Space]
    public GameObject panel;
    public GameObject text;
    [Space] 
    public float time;
    
    void Start()
    {
        panel.SetActive(false);
    }

    public void ShowNotification(string message)
    {
        text.GetComponent<Text>().text = message;
        panel.SetActive(true);
        _timer = time;
        _active = true;
    }


    public void Hide()
    {
        panel.SetActive(false);
        _active = false;
    }
    
    private void FixedUpdate()
    {
        if (!_active)
        {
            return;
        }
        
        if (_timer > 0)
        {
            _timer -= Time.fixedDeltaTime;
        }
        else
        {
            Hide();
        }
    }
}
