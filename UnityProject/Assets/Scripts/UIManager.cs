using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public SimController simController;

    [Header("Menu")]
    public GameObject UIMenu;
    public Dropdown DShape;
    public InputField IFPopulation;
    public InputField IFVelocity;
    public Text TParam1;
    public Text TParam2;
    public Text TParam3;
    public InputField IFParam1;
    public InputField IFParam2;
    public InputField IFParam3;
    public Toggle toggleAStiCo;
    public InputField IFAStiCo;
    public Toggle toggleIdStiCo;
    public Text TError;

    private int population = -1;
    private float velocity = -1f;
    private float param1 = -1f;
    private float param2 = -1f;
    private float param3 = -1f;
    private float timeIncrease = -1f;
    private SimController.shape simShape;

    // Starts simulation if all values are correct
    public void StartSimulation()
    {
        bool valid = CheckValues();
        if (valid)
        {
            UIMenu.SetActive(false);
            simController.Init(simShape, population, velocity, param1, param2, param3, toggleAStiCo.isOn, timeIncrease, toggleIdStiCo.isOn);
        }
        else
            TError.text = "Error : Invalid values";
    } 

    // Changes parameters displayed
    public void ChangeParameters()
    {
        string s = DShape.captionText.text;

        switch(s)
        {
            case "Circle" :
                TParam1.text = "Angular Velocity";
                TParam2.text = "Max Curve";
                TParam3.text = "Min Curve";
                break;
            default :
                TParam1.text = "Distance";
                TParam2.text = "Max Distance";
                TParam3.text = "Min Distance";
                break;
                
        }
    }

    // Checks values validity
    private bool CheckValues()
    {
        bool pBool = int.TryParse(IFPopulation.text, out population);
        if (!pBool) return false;
        bool vBool = float.TryParse(IFVelocity.text, NumberStyles.Any, CultureInfo.InvariantCulture, out velocity);
        if(!vBool) return false;
        bool p1Bool = float.TryParse(IFParam1.text, NumberStyles.Any, CultureInfo.InvariantCulture, out param1);
        if(!p1Bool) return false;
        bool p2Bool = float.TryParse(IFParam2.text, NumberStyles.Any, CultureInfo.InvariantCulture, out param2);
        if(!p2Bool) return false;
        bool p3Bool = float.TryParse(IFParam3.text, NumberStyles.Any, CultureInfo.InvariantCulture, out param3);
        if(!p3Bool) return false;

        if(toggleAStiCo.isOn)
        {
            bool aBool = float.TryParse(IFAStiCo.text, NumberStyles.Any, CultureInfo.InvariantCulture, out timeIncrease);
            if (!aBool) return false;
        }

        if((population<=0) || (velocity<=0f) || (param1<=0f)
             || (param2<=0f) || (param3<=0f) || (toggleAStiCo.isOn && timeIncrease<=0f))
            return false;
        
        string s = DShape.captionText.text;
        switch(s)
        {
            case "Circle" :
                simShape = SimController.shape.Circle;
                break;
            case "Square" :
                simShape = SimController.shape.Square;
                break;
            case "Triangle" :
                simShape = SimController.shape.Triangle;
                break;
        }
        return true;
    }
}
