using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalSprite : MonoBehaviour
{
    [Header("Look propertys")]
    public bool followCam = true;
    public bool yAxisOnly = true;

    public Vector3 lookOffset = Vector3.zero;
    [HideInInspector] public Vector3 orginalRot = Vector3.zero;

    [Header("Sprite propertys")]
    public Sprite[] directionalSprites;
    public int currentSpriteIndex;
    public int spriteCount = 8;

    public float sortAngle; //how many sprites /360
    private float lookAngle;

    public float test;

    [HideInInspector] public SpriteRenderer render;

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();

        //SetUp();
    }


    void Update()
    {
        if (Camera.main == null)
            return;

        if (followCam)
        {
            faceCamera(yAxisOnly);
        }

        if (directionalSprites.Length < 2) //optermisation 
            return;
        

        Vector3 toOther = (transform.position - Camera.main.transform.position).normalized;

        lookAngle = Mathf.Atan2(toOther.z, toOther.x) * Mathf.Rad2Deg + 180; //mathmoment
        lookAngle = (lookAngle - (sortAngle / 2)) + transform.parent.eulerAngles.y;




        int spriteNum = (int)(lookAngle / sortAngle);

        if (spriteNum >= spriteCount)
            spriteNum = (spriteNum - spriteCount);

        else if (spriteNum < 0)
            spriteNum = (spriteNum + spriteCount);


        if (currentSpriteIndex != spriteNum)
        {
            currentSpriteIndex = spriteNum;
            render.sprite = directionalSprites[currentSpriteIndex];
        }
    }

    void faceCamera(bool yOnly) //rotates the mesh to face the main.camera
    {
        transform.LookAt(Camera.main.transform.position, -Vector3.up);

        transform.eulerAngles += lookOffset;

        if (yOnly)
            transform.eulerAngles = new Vector3(lookOffset.x, transform.eulerAngles.y, lookOffset.z);
    }
}
