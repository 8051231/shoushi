using Hi5_Interaction_Core;
using UnityEngine;
using Ladder.Scripts;

public class Rotate : MonoBehaviour
{
    public enum RotateType
    {
        EDefault = 0,
        ELeftFistLeftRotate,
        ERightFistRightRotate,
        ELeftOkUp,
        ERightOkDown
    };

    public static RotateType rotateType = RotateType.EDefault;
    public float RunSpeed = 3;
    public static int enableRotate = 0;
    protected Hi5_Hand_Visible_Hand mHand = null;
    //摄像机
    private Camera m_Camera;
    private bool m_isClimbing = false; // Are we currently climbing?
                                       //当前触发的梯子
                                       //视角控制脚本
    [SerializeField]
    private MouseLook m_MouseLook;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        m_Camera = Camera.main;
        m_MouseLook.Init(player.transform, m_Camera.transform);
    }

    // Update is called once per frame
    void Update() 
    {
        if (rotateType == RotateType.ELeftFistLeftRotate) //left fist left-rotate
        {
            RotateView();
        }
        else if (rotateType == RotateType.ERightFistRightRotate) //right fist right-rotate
        {
            RotateView();
        }
        else if (rotateType == RotateType.ELeftOkUp) //left ok up
        {
            float ver2 = 0.2f;
            if (ver2 > 0 && m_Camera.transform.localPosition.y > 0.2f)
            {
                m_Camera.transform.position += -transform.up * ver2 * Time.deltaTime;
            }
        }
        else if (rotateType == RotateType.ERightOkDown)  //right ok down
        {
            float ver2 = -0.2f;
            if (ver2 < 0 && m_Camera.transform.localPosition.y < 4.4f)
            {
                m_Camera.transform.position += -transform.up * ver2 * Time.deltaTime;
            }
        }
        rotateType = RotateType.EDefault;
    }

    //选择视角到正常角度
    private void RotateView()
    {
        if (!m_isClimbing)
        {
        //    m_MouseLook.LookRotation(player.transform, m_Camera.transform, null, rotateType);
        }
        else 
        {
            // If we are climbing let's look to the direction of the ladder
            // 如果我们在爬梯，朝着梯子的方向
            //       m_MouseLook.LookRotation(transform, m_Camera.transform, m_ladderTrigger);
        //    m_MouseLook.LookRotation(player.transform, m_Camera.transform, null, rotateType);
        }
    }
}
