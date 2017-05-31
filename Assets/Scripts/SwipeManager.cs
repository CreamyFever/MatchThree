using UnityEngine;
using SingletonPattern;

public enum SwipeDirection
{
    None = 0,       // 0000
    Left = 1,       // 0001
    Right = 2,      // 0010
    Up = 4,         // 0100
    Down = 8,       // 1000

    LeftDown = 9,   // 1001
    LeftUp = 5,     // 0101
    RightDown = 10, // 1010
    RightUp = 6     // 0110
}


public class SwipeManager : SingletonPattern<SwipeManager>
{
    public GameObject targetObj = null;

    public SwipeDirection m_Direction { set; get; }

    private Vector3 touchPos;
    private float swipeResistanceX = 50.0f;     // スワイプの抵抗値。大きいほどもっと大きく振らないと作動しない。
    private float swipeResistanceY = 50.0f;
    
    void Update()
    {
        m_Direction = SwipeDirection.None;

        #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        if (Input.GetMouseButtonDown(0))
        {
            touchPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            DecideDirectionBySwiping();
            targetObj = null;
        }

        #elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.touchCount < 2)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
            {
                touchPos = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                DecideDirectionBySwiping();
            }
        }
        #endif
    }

    void DecideDirectionBySwiping()
    {
        Vector2 deltaSwipe = Input.mousePosition - touchPos;

        if (Mathf.Abs(deltaSwipe.x) > swipeResistanceX)
        {
            //Swipe on the X axis
            m_Direction |= (deltaSwipe.x > 0) ? SwipeDirection.Right : SwipeDirection.Left;
        }

        if (Mathf.Abs(deltaSwipe.y) > swipeResistanceY)
        {
            //Swipe on the Y axis
            m_Direction |= (deltaSwipe.y > 0) ? SwipeDirection.Up : SwipeDirection.Down;
        }
    }

    public void SetTargetObject(GameObject obj)
    {
        targetObj = obj;
    }

    public void ReleaseTargetObject()
    {
        targetObj = null;
    }

    public bool IsSwiping(SwipeDirection dir)
    {
        return (m_Direction & dir) == dir;
    }
}
