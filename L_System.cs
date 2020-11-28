using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UI;

//Reference link:
//https://www.youtube.com/watch?v=tUbTGWl-qus || https://github.com/pejoph/CT5009
//https://www.youtube.com/watch?v=AWDEfN0eeWI
//https://www.youtube.com/watch?v=fCskfurruS8 || https://github.com/davilopez10/LSystem_Unity
//https://www.youtube.com/watch?v=wo3VGyF5z8Q&t=67s
//https://github.com/SamMurphy/L-System-Trees-in-Unity

public class TransformInfo
{
    public Vector3 position;
    public Quaternion rotation;
}



public class L_System : MonoBehaviour
{
    //The times of iterations.
    private int iterations;
    private int lastTree = -1;
    int ChooseTree = 0;
    private float length;
    private float width;
    //Angle to rotate the tree's growth by.
    private float angle;
    public float fieldofView;

    private string axiom;
    private string currentString = string.Empty;

    // Store the tree's position and rotation.
    private Vector3 posInit;
    private Vector3 rotInit;

    [SerializeField] private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;

    [SerializeField] private GameObject treeStem;
    [SerializeField] private GameObject branch;
    private GameObject Tree = null;
    private GameObject treeSegment;

    //InputFields to assign in the Inspector. 
    [SerializeField] private InputField inputFieldAngle;
    [SerializeField] private InputField inputFieldLength;
    [SerializeField] private InputField inputFieldWidth; //consider about whether add width or not
    [SerializeField] private InputField inputFieldIterations;
    [SerializeField] private InputField inputFieldAxiom;
    [SerializeField] private InputField inputFieldAlphabet;
    [SerializeField] private InputField inputFieldRules;
    [SerializeField] private InputField inputFieldMer;

    //UI Button
    [SerializeField] private Button resetButton;
    //[SerializeField] private Button resetAxiomButton;
    //[SerializeField] private Button resetRuleButton;
    //[SerializeField] private Button changeAxiomButton;
    //[SerializeField] private Button addtoRuleButton;
    //[SerializeField] private Button addValuesButton;
    //[SerializeField] private Button SelectTreeOneButton;
    //[SerializeField] private Button SelectTreeTwoButton;
    [SerializeField] private Slider CameraSlider;
    //[SerializeField] private Button changeIterationButton;

    [SerializeField] private Button Tree1Button;
    [SerializeField] private Button Tree2Button;
    [SerializeField] private Button Tree3Button;
    [SerializeField] private Button Tree4Button;
    [SerializeField] private Button Tree5Button;
    [SerializeField] private Button Tree6Button;
    [SerializeField] private Button Tree7Button;
    [SerializeField] private Button Tree8Button;


    public Slider angleSlider;
    public Slider lengthSlider;
    public Slider widthSlider;


    //public Button growTreeOnebyStep;
    //public Button growTreeTwobyStep;


    //public Button growTreeThreebyStep;
    //public Button growTreeFourbyStep;
    //public Button growTreeFivebyStep;
    //public Button growTreeSixbyStep;
    //public Button growTreeSevenbyStep;
    //public Button growTreeEightbyStep;

    //Color change?
    private Color startColour = new Color(55, 89, 10, 255);
    private Color endColour = Color.green;

    //private string inputFieldAlphabetValue;
    //private string inputFieldRuleSet;


    //public Text textComponent;

    // Use this for initialization
    void Start()
    {
        fieldofView = 60.0f;

        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {   //Example
            //{'X',"F[+X]F[-X]+X" },
            //{'F',"FF" }
        };

        currentString = axiom;

        //Generate();

    }


    // Update is called once per frame
    void Update()
    {

        Camera.main.fieldOfView = fieldofView;

        OnGUI();//control view

        int moveSpeed = 20; // speed of camera movement

        if (Input.GetKey(KeyCode.W))
        {
            Camera.main.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Camera.main.transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Camera.main.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Camera.main.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
        }



        #region
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    HotKeyTreeOne();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    HotKeyTreeTwo();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    HotKeyTreeThree();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    HotKeyTreeFour();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    HotKeyTreeFive();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    HotKeyTreeSix();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    HotKeyTreeSeven();
        //}

        //else if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    HotKeyTreeEight();
        //}

        #endregion

        switch (ChooseTree)
        {
            case 1:
                HotKeyTreeOne();
                break;
            case 2:
                HotKeyTreeTwo();
                break;
            case 3:
                HotKeyTreeThree();
                break;
            case 4:
                HotKeyTreeFour();
                break;
            case 5:
                HotKeyTreeFive();
                break;
            case 6:
                HotKeyTreeSix();
                break;
            case 7:
                HotKeyTreeSeven();
                break;
            case 8:
                HotKeyTreeEight();                
                break;
            default:
                break;
        }


    }

    // This is the core part of the script. It generates the L-System.
    private void Generate(bool cr = false)
    {
        //reset the position where to add the next segment
        transform.position = posInit;
        transform.eulerAngles = rotInit;

        //transformStack.Clear();
        //destroy the tree if already generate other trees
        if (treeStem != null)
            Destroy(Tree);

        Tree = Instantiate(treeStem);

        StringBuilder sb = new StringBuilder();

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Debug.Log("GeneratecurrentString");

        //Apply rules
        // Loop through characters in the input string

        for (int i = 0; i < iterations; i++) //count iteraction
        {
            //check every character in the current string, which has been replaced to new string by the rule.
            foreach (char c in currentString)
            {
                //?substitutes the replacement string in the relevant production rules with the current character
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        StopAllCoroutines();

        if (cr == false)
            ShowTree();
        else
            StartCoroutine(ShowTreeCr());

    }

    public Material mat;
    public void ShowTree()
    {
        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F': //// Draw line of length lastBranchLength, in direction of lastAngle
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);

                    treeSegment = Instantiate(branch);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    treeSegment.GetComponent<LineRenderer>().startWidth = width;
                    treeSegment.GetComponent<LineRenderer>().endWidth = width;
                    treeSegment.GetComponent<LineRenderer>().material = mat;//material
                    treeSegment.GetComponent<LineRenderer>().startColor = startColour;
                    treeSegment.GetComponent<LineRenderer>().endColor = endColour;
                    treeSegment.transform.SetParent(Tree.transform);

                    break;

                case 'X': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'V': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'W': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'Y': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'Z': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case '+':// If there is a plus in the rule, rotate the branch angle
                    transform.Rotate(Vector3.forward * angle);
                    break;

                case '-':// If there is a minus in the rule, rotate the branch angle to another side
                    transform.Rotate(Vector3.back * angle);
                    break;

                case '*':// rotate the branch angle +120
                    transform.Rotate(Vector3.up * 120);
                    break;

                case '/':// rotate the branch angle -120
                    transform.Rotate(Vector3.down * 120);
                    break;

                case '['://save state of the position and rotation
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']'://load the saved state
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-Tree operation"); //if error?
            }
        }

        length /= 2;
        width /= 2;

        Debug.Log(" ShowTreeGenerate");
    }

    public IEnumerator ShowTreeCr()
    {

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':
                    Vector3 initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);

                    treeSegment = Instantiate(branch);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    treeSegment.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    treeSegment.GetComponent<LineRenderer>().startWidth = width;
                    treeSegment.GetComponent<LineRenderer>().endWidth = width;
                    treeSegment.GetComponent<LineRenderer>().material = mat;
                    treeSegment.GetComponent<LineRenderer>().startColor = startColour;
                    treeSegment.GetComponent<LineRenderer>().endColor = endColour;
                    treeSegment.transform.SetParent(Tree.transform);

                    yield return new WaitForEndOfFrame();

                    break;

                case 'X': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'V': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'W': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'Y': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case 'Z': // X is a “dummy” commands into the rule set, which we can use to control how the directions string evolves. 
                    break;

                case '+':// rotate the branch angle
                    transform.Rotate(Vector3.forward * angle);
                    break;

                case '-':// rotate the branch angle to another side
                    transform.Rotate(Vector3.back * angle);
                    break;

                case '*':// rotate the branch angle to another side
                    transform.Rotate(Vector3.up * 120);
                    break;

                case '/':// rotate the branch angle to another side
                    transform.Rotate(Vector3.down * 120);
                    break;

                case '['://save the position and rotation
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']'://go back to the saved place
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-Tree operation"); //if error?
            }
        }

        length /= 2;
        width /= 2;

        Debug.Log("ShowTreeCrGenerate");
    }

    public void OnEnable() //Button control,call generate function when clik the button
    {
        Tree1Button.onClick.AddListener(TreeOne);
        Tree2Button.onClick.AddListener(TreeTwo);
        Tree3Button.onClick.AddListener(TreeThree);
        Tree4Button.onClick.AddListener(TreeFour);
        Tree5Button.onClick.AddListener(TreeFive);
        Tree6Button.onClick.AddListener(TreeSix);
        Tree7Button.onClick.AddListener(TreeSeven);
        Tree8Button.onClick.AddListener(TreeEight);

        resetButton.onClick.AddListener(ResetValues);

        //generateButton.onClick.AddListener(Generate);

        //addtoRuleButton.onClick.AddListener(AddToRule);
        //changeAxiomButton.onClick.AddListener(ChangeAxiom);
        //resetAxiomButton.onClick.AddListener(ResetAxiom);
        //resetRuleButton.onClick.AddListener(ResetRule);
        //addValuesButton.onClick.AddListener(AddValues);
        //onemoreIterationButton.onClick.AddListener(OneMoreIteration);
        //changeIterationButton.onClick.AddListener(ChangeIteration);



        //growTreeOnebyStep.onClick.AddListener(GrowTreeOnebyStep);
        //growTreeTwobyStep.onClick.AddListener(GrowTreeTwobyStep);


        //growTreeOnebyStep.onClick.AddListener(GrowTreeThreebyStep);
        //growTreeOnebyStep.onClick.AddListener(GrowTreeFourbyStep);
        //growTreeOnebyStep.onClick.AddListener(GrowTreeFivebyStep);
        //growTreeOnebyStep.onClick.AddListener(GrowTreebyStep);
        //growTreeOnebyStep.onClick.AddListener(GrowTreeOnebyStep);
        //growTreeOnebyStep.onClick.AddListener(GrowTreeOnebyStep);
        //SelectTreeOneButton.onClick.AddListener(SelectTreeOne);
    }


    //void GrowTreeOnebyStep()
    //{
    //    iterations += 1;
    //    SelectTreeOne();
    //    Debug.Log("Grow tree 1");
    //}

    //void GrowTreeTwobyStep()
    //{
    //    iterations += 1;
    //    SelectTreeTwo();
    //    Debug.Log("Grow tree 2");
    //}



    //private void AddValues()
    //{
    //    angle = float.Parse(inputFieldAngle.text);
    //    length = float.Parse(inputFieldLength.text);
    //    width = float.Parse(inputFieldWidth.text);
    //    iterations = int.Parse(inputFieldIterations.text);
    //    Debug.Log("Add Values Iterations:" + iterations);

    //    //Text textAngleComponent = inputFieldAngle.textComponent;
    //    //StextAngle = textAngleComponent.text;

    //    Generate();
    //}

    //private void ChangeIteration()
    //{

    //    axiom = inputFieldAxiom.text;
    //    currentString = axiom;
    //    inputFieldAxiom.text = "";
    //    Debug.Log("InterChangeAxiom");

    //    inputFieldAlphabetValue = inputFieldAlphabet.text;
    //    inputFieldRuleSet = inputFieldRules.text;
    //    inputFieldAlphabet.text = "";
    //    inputFieldRules.text = "";

    //    Debug.Log("InterinputFieldkeyandrule");

    //    rules.Clear();
    //    rules.Add(inputFieldAlphabetValue[0], inputFieldRuleSet);
    //    foreach (KeyValuePair<char, string> kvp in rules)
    //    {
    //        //texBox3.???
    //        Debug.Log("interKey = {0}, Value = {1}" + " " + kvp.Key + " " + kvp.Value);
    //    }


    //    Generate();
    //    //Text textAngleComponent = inputFieldAngle.textComponent;
    //    //StextAngle = textAngleComponent.text;

    //    axiom = "F";
    //    inputFieldAlphabet.text = "F";
    //    inputFieldAxiom.text = "F";
    //    inputFieldRules.text = "F[+F]F[-F]F";
    //    Debug.Log("Inter2ChangeAxiom");

    //    //rules.Clear();
    //    rules = new Dictionary<char, string>
    //    {
    //        { 'F', "F[+F]F[-F]F" }
    //    };

    //    currentString = axiom;
    //}


    
    private void ResetValues()//reset screen input field values
    {
        inputFieldAngle.text = "";
        inputFieldLength.text = "";
        inputFieldWidth.text = "";
        inputFieldIterations.text = "";
        inputFieldAlphabet.text = "";
        inputFieldAxiom.text = "";
        inputFieldRules.text = "";

    }

    //private void AddToRule()
    //{
    //    inputFieldAlphabetValue = inputFieldAlphabet.text;
    //    inputFieldRuleSet = inputFieldRules.text;
    //    inputFieldAlphabet.text = "";
    //    inputFieldRules.text = "";

    //    Debug.Log("inputFieldAxiom");

    //    rules.Clear();
    //    rules.Add(inputFieldAlphabetValue[0], inputFieldRuleSet);
    //    foreach (KeyValuePair<char, string> kvp in rules)
    //    {
    //        //texBox3.???
    //        Debug.Log("Key = {0}, Value = {1}" + " " + kvp.Key + " " + kvp.Value);
    //    }



    //}



    //private void ResetRule()
    //{
    //    rules.Clear();
    //}

    //private void ChangeAxiom()
    //{
    //    axiom = inputFieldAxiom.text;
    //    currentString = axiom;
    //    inputFieldAxiom.text = "";
    //    Debug.Log("ChangeAxiom");

    //}

    //private void ResetAxiom()
    //{
    //    axiom = null;
    //    Debug.Log("ResetAxiom");
    //}

    private void OnGUI()
    {
        float min = 20.0f;
        float max = 179.0f;
        CameraSlider.minValue = min;
        CameraSlider.maxValue = max;
        Camera.main.fieldOfView = CameraSlider.value;
    }

    void TreeOne()
    {
        ChooseTree = 1;
        if (iterations < 5)
        {   // growing step by step, next iteration
            iterations += 1;
            //SelectTreeOne();
        }

        SelectTreeOne();

    }

    private void SelectTreeOne()
    {
        if (lastTree != 1)
        {
            iterations = 0;
            lastTree = 1;
        }

        angle = 25.7f;
        length = 0.4f;
        width = 0.7f;

        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "F";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "F";
        inputFieldAxiom.text = "F";
        inputFieldRules.text = "F[+F]F[-F]F";

        //rules.Clear();
        rules = new Dictionary<char, string>
        {
            { 'F', "F[+F]F[-F]F" }
        };

        currentString = axiom;

        Generate();
        inputFieldMer.text = " L-system Tree 1";

    }

    private void HotKeyTreeOne()
    {
        if (lastTree != 1)
        {
            iterations = 0;
            lastTree = 1;
        }


        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 5;
        axiom = "F";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "F";
        inputFieldAxiom.text = "F";
        inputFieldRules.text = "F[+F]F[-F]F";

        //rules.Clear();
        rules = new Dictionary<char, string>
        {
            { 'F', "F[+F]F[-F]F" }
        };

        currentString = axiom;

        Generate();


    }

    void TreeTwo()
    {
        ChooseTree = 2;
        if (iterations < 5)
        {
            iterations += 1;
        }

        SelectTreeTwo();

    }

    private void SelectTreeTwo()
    {
        if (lastTree != 2)
        {
            iterations = 0;
            lastTree = 2;
        }

        angle = 20.0f;
        length = 1.5f;
        width = 0.4f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "F";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "F";
        inputFieldAxiom.text = "F";
        inputFieldRules.text = "F[+F]F[-F][F]";

        //rules.Clear();
        rules = new Dictionary<char, string>
        {
            {'F', "F[+F]F[-F][F]" }
        };

        currentString = axiom;

        Generate();

        inputFieldMer.text = " L-system Tree 2";
    }

    private void HotKeyTreeTwo()
    {
        if (lastTree != 2)
        {
            iterations = 0;
            lastTree = 2;
        }

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 5;
        axiom = "F";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "F";
        inputFieldAxiom.text = "F";
        inputFieldRules.text = "F[+F]F[-F][F]";

        //rules.Clear();
        rules = new Dictionary<char, string>
        {
            {'F', "F[+F]F[-F][F]" }
        };

        currentString = axiom;

        Generate();
    }


    void TreeThree()
    {
        ChooseTree = 3;
        if (iterations < 4)
        {
            iterations += 1;
        }

        SelectTreeThree();

    }

    private void SelectTreeThree()
    {
        if (lastTree != 3)
        {
            iterations = 0;
        }
        lastTree = 3;

        angle = 22.5f;
        length = 1.5f;
        width = 0.5f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "F";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "F";
        inputFieldAxiom.text = "F";
        inputFieldRules.text = "F[+F]F[-F][F]";

        rules = new Dictionary<char, string>
        {
            { 'F', "FF-[-F+F+F]+[+F-F-F]" }
        };

        currentString = axiom;

        Generate();

        inputFieldMer.text = " L-system Tree 3";
    }

    private void HotKeyTreeThree()
    {
        if (lastTree != 3)
        {
            iterations = 0;
        }
        lastTree = 3;

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 7;
        axiom = "F";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "F";
        inputFieldAxiom.text = "F";
        inputFieldRules.text = "F[+F]F[-F][F]";

        rules = new Dictionary<char, string>
        {
            { 'F', "FF-[-F+F+F]+[+F-F-F]" }
        };

        currentString = axiom;

        Generate();
    }

    void TreeFour()
    {
        ChooseTree = 4;
        if (iterations < 7)
        {
            iterations += 1;
        }

        SelectTreeFour();

    }

    private void SelectTreeFour()
    {

        if (lastTree != 4)
        {
            iterations = 0;
        }
        lastTree = 4;

        angle = 20.0f;
        length = 0.4f;
        width = 0.5f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "F[+X]F[-X]+X,FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "F[+X]F[-X]+X" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
        inputFieldMer.text = " L-system Tree 4";
    }
    private void HotKeyTreeFour()
    {

        if (lastTree != 4)
        {
            iterations = 0;
        }
        lastTree = 4;

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 7;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "F[+X]F[-X]+X,FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "F[+X]F[-X]+X" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
    }

    void TreeFive()
    {
        ChooseTree = 5;
        if (iterations < 7)
        {
            iterations += 1;
        }

        SelectTreeFive();

    }

    private void SelectTreeFive()
    {
        if (lastTree != 5)
        {
            iterations = 0;
        }
        lastTree = 5;

        angle = 25.7f;
        length = 0.4f;
        width = 0.5f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "F[+X][-X]FX,FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "F[+X][-X]FX" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
        inputFieldMer.text = " L-system Tree 5";
    }

    private void HotKeyTreeFive()
    {
        if (lastTree != 5)
        {
            iterations = 0;
        }
        lastTree = 5;

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 7;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "F[+X][-X]FX,FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "F[+X][-X]FX" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
    }

    void TreeSix()
    {
        ChooseTree = 6;
        if (iterations < 5)
        {
            iterations += 1;
        }

        SelectTreeSix();

    }

    private void SelectTreeSix()
    {

        if (lastTree != 6)
        {
            iterations = 0;
        }
        lastTree = 6;

        angle = 22.5f;
        length = 1.2f;
        width = 0.4f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "F-[[X]+X]+F[+FX]-X,FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "F-[[X]+X]+F[+FX]-X" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
        inputFieldMer.text = " L-system Tree 6";
    }

    private void HotKeyTreeSix()
    {

        if (lastTree != 6)
        {
            iterations = 0;
        }
        lastTree = 6;

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 5;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "F-[[X]+X]+F[+FX]-X,FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "F-[[X]+X]+F[+FX]-X" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
    }

    //Personal Tree

    void TreeSeven()
    {
        ChooseTree = 7;
        if (iterations < 5)
        {
            iterations += 1;
        }

        SelectTreeSeven();

    }

    #region
    private void SelectTreeSeven()
    {
        if (lastTree != 7)
        {
            iterations = 0;
        }
        lastTree = 7;

        angle = 20.0f;
        length = 1.5f;
        width = 0.3f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "[*+FX]X[+FX][/+F-FX],FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();

        inputFieldMer.text = " L-system Tree 7";
    }

    private void HotKeyTreeSeven()
    {
        if (lastTree != 7)
        {
            iterations = 0;
        }
        lastTree = 7;

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 6;
        axiom = "X";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X,F";
        inputFieldAxiom.text = "X";
        inputFieldRules.text = "[*+FX]X[+FX][/+F-FX],FF";

        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        currentString = axiom;

        Generate();
    }

    void TreeEight()
    {
        ChooseTree = 8;
        if (iterations < 9)
        {
            iterations += 1;
        }


        SelectTreeEight();
    }

    private void SelectTreeEight()
    {
        if (lastTree != 8)
        {
            iterations = 0;
        }
        lastTree = 8;

        angle = 20.0f;
        length = 3.0f;
        width = 0.4f;
        angleSlider.value = angle;
        lengthSlider.value = length;
        widthSlider.value = width;
        //iterations += 1;
        axiom = "VZFFF";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X";
        inputFieldAxiom.text = "VZFFF";
        inputFieldRules.text = "[+++W][---W]YV,+X[-W]Z,-W[+X]Z,YZ,[-FFF][+FFF]F";

        rules = new Dictionary<char, string>
        {
            { 'V', "[+++W][---W]YV" },
            { 'W', "+X[-W]Z" },
            { 'X', "-W[+X]Z" },
            { 'Y', "YZ" },
            { 'Z', "[-FFF][+FFF]F" }
        };

        currentString = axiom;

        Generate();
        inputFieldMer.text = " :) Merry Christmas";


    }

    private void HotKeyTreeEight()
    {
        
        if (lastTree != 8)
        {
            iterations = 0;
        }
        lastTree = 8;

        angle = angleSlider.value;
        length = lengthSlider.value;
        width = widthSlider.value;
        //iterations = 10;
        axiom = "VZFFF";

        inputFieldAngle.text = angle.ToString();
        inputFieldLength.text = length.ToString();
        inputFieldWidth.text = width.ToString();
        inputFieldIterations.text = iterations.ToString();
        inputFieldAlphabet.text = "X";
        inputFieldAxiom.text = "VZFFF";
        inputFieldRules.text = "[+++W][---W]YV,+X[-W]Z,-W[+X]Z,YZ,[-FFF][+FFF]F";

        rules = new Dictionary<char, string>
        {
            { 'V', "[+++W][---W]YV" },
            { 'W', "+X[-W]Z" },
            { 'X', "-W[+X]Z" },
            { 'Y', "YZ" },
            { 'Z', "[-FFF][+FFF]F" }
        };

        currentString = axiom;

        Generate();


    }
    #endregion

    //public void Awake()
    //{
    //    posInit = transform.position;
    //    rotInit = transform.eulerAngles;
    //    //GenerateAxiom(true);
    //}
}
