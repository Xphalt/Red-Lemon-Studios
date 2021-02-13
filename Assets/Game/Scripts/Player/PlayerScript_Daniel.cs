/*
 * DANIEL BIBBY
 */


using UnityEngine;

public class PlayerScript_Daniel : MonoBehaviour
{
    internal GameObject tool;
    public GameObject weapon;

    internal ToolBase toolActivate;
    internal ElementShooting shooter;
    internal ElementsScript_MattNDaniel elementChanger;

    private float toolTimer = 0;
    public float toolDuration;
    internal bool isToolAvailable;

    private void Start()
    {
        isToolAvailable = false;

        shooter = weapon.GetComponent<ElementShooting>();
        elementChanger = weapon.GetComponent<ElementsScript_MattNDaniel>();
    }

    void Update()
    {
        Inputs();

        ToolCooldown();
    }

    private void Inputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            shooter.Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseTool();
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            elementChanger.ChangeElement(Mathf.FloorToInt(Input.mouseScrollDelta.y));
        }
    }



    #region Tool
    private void UseTool()
    {
        if (isToolAvailable)
        {
            toolActivate.Activate();
            isToolAvailable = false;
        }
    }

    private void ToolCooldown()
    {
        if (!isToolAvailable)
        {
            toolTimer += Time.deltaTime;

            if (toolTimer > toolDuration)
            {
                isToolAvailable = true;
                toolTimer = 0;
            }
        }
    }
    #endregion
}