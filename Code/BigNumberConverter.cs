using UnityEditor;
using UnityEngine;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine.Windows;

public class BigNumberConverter : EditorWindow
{
    private string inputString = "";
    private string resultString = "";

    private string inputString2 = ""; // ���� -> ����
    private string resultString2 = ""; // ���� -> ����


    private string numberA = ""; // ù ��° ���� �Է� �ʵ�
    private string numberB = ""; // �� ��° ���� �Է� �ʵ�
    private string calculationResult = ""; // ��� ��� �ʵ�

    private UnityEngine.Vector2 scrollPosition;

    private List<string> suffixes = new List<string>(new[] { "" });

    [MenuItem("Tools/BigNumberConverter")]
    public static void ShowWindow()
    {
        GetWindow<BigNumberConverter>("Big Number Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("Ver. 1.0.1", EditorStyles.boldLabel);
        DrawLine();

        GUILayout.Label("Big Number Converter", EditorStyles.boldLabel);


        GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea)
        {
            wordWrap = true
        };

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
        inputString = EditorGUILayout.TextArea(inputString, textAreaStyle, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Convert"))
        {
            if (string.IsNullOrEmpty(inputString))
            {
                resultString = "0";
            }
            else resultString = ConvertToBigNumberFormat(inputString);
        }

        EditorGUILayout.LabelField("Converted Number:", resultString, textAreaStyle);
        GUILayout.Space(10);
        DrawLine();

        

        GUILayout.Label("Back to Big Number Converter", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
        inputString2 = EditorGUILayout.TextArea(inputString2, textAreaStyle, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();


        if (GUILayout.Button("Convert Back To Number"))
        {
            // �Է°��� ��� �ְų� null�� ��� �˻�
            if (string.IsNullOrEmpty(inputString2))
            {
                resultString2 = "0";
            }
            else
            {
                // �Է°��� ��ȿ���� ���� ���ڰ� ���ԵǾ� �ִ��� �˻�
                // ��ȿ�� �Է�: ����, �Ҽ���, �빮�� ���ĺ�
                if (System.Text.RegularExpressions.Regex.IsMatch(inputString2, @"^[0-9A-Z\.]+$"))
                {
                    resultString2 = ConvertBackToNumber(inputString2).ToString();
                }
                else
                {
                    // ��ȿ���� ���� ���ڰ� ���Ե� ���
                    resultString2 = "Invalid Input";
                }
            }

        }


        EditorGUILayout.LabelField("Converted Number:", resultString2, textAreaStyle);



        

        GUILayout.Space(10);
        DrawLine();

        GUILayout.Label("Big Number calculator", EditorStyles.boldLabel);

        // ��Ģ������ ���� �߰� ���
        GUILayout.Label("Number A:");
        numberA = EditorGUILayout.TextArea(numberA, textAreaStyle);
        GUILayout.Label("Number B:");
        numberB = EditorGUILayout.TextArea(numberB, textAreaStyle);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+")) PerformCalculation("+");
        if (GUILayout.Button("-")) PerformCalculation("-");
        if (GUILayout.Button("*")) PerformCalculation("*");
        if (GUILayout.Button("/")) PerformCalculation("/");
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Calculation Result:", calculationResult,textAreaStyle);
    }

    private BigInteger ConvertBackToNumber(string numberToString)
    {
        // �и�: ���� �κ�(�Ҽ��� ����)�� ���� �κ�
        var match = System.Text.RegularExpressions.Regex.Match(numberToString, @"(\d+(?:\.\d+)?)([A-Z]+)");

        if (!match.Success) return BigInteger.Parse(numberToString); 

        string numberPart = match.Groups[1].Value;
        string suffix = match.Groups[2].Value;

        // �Ҽ��� ó��
        int decimalPlaces = numberPart.Contains(".") ? numberPart.Length - numberPart.IndexOf(".") - 1 : 0;
        BigInteger baseNumber = BigInteger.Parse(numberPart.Replace(".", ""));

        // ���� �κп� �ش��ϴ� ���� ���
        BigInteger exponent = 0;
        for (int i = 0; i < suffix.Length; i++)
        {
            int charPosition = suffix[i] - 'A' + 1;
            exponent = exponent * 26 + charPosition;
        }

        // ���� ������ �Ҽ��� �Ʒ� �ڸ������� ���� ���, ���� ������ ���� ����
        int totalExponent = 3 * (int)exponent - decimalPlaces;
        if (totalExponent < 0)
        {
            // ���� ������ �����ϱ� ���� ó��
            return BigInteger.Zero;
        }

        // ������ ������� �ϴ� ��ȯ: 1A -> 1000, 1B -> 1000000, ...
        BigInteger result = baseNumber * BigInteger.Pow(10, totalExponent);
        return result;
    }



    private string ConvertToBigNumberFormat(string input)
    {
        BigInteger number;
        if (BigInteger.TryParse(input, out number))
        {
            return NumberToBigNumberFormat(number);
        }
        else
        {
            return "Invalid Input";
        }
    }

    private string NumberToBigNumberFormat(BigInteger number)
    {
        if (number == 0) return "0";

        int unitIndex = 0;
        double value = (double)number;


        while (value >= 1000)
        {
            value /= 1000;
            unitIndex++;
        }


        while (unitIndex >= suffixes.Count)
        {
            ExtendSuffixes();
        }

        string suffix = unitIndex > 0 ? suffixes[unitIndex - 1] : "";


        string format = "0.##";
        string result = value.ToString(format) + suffix;

        return result;
    }
    private void PerformCalculation(string operation)
    {
        BigInteger a, b;


        if (!TryConvertNumberOrInvalidInput(numberA, out a) || !TryConvertNumberOrInvalidInput(numberB, out b))
        {
            return; // ��ȿ���� ���� �Է��� ������ �Լ� ����
        }

        switch (operation)
        {
            case "+":
                calculationResult = NumberToBigNumberFormat(a + b).ToString();
                break;
            case "-":
                calculationResult = NumberToBigNumberFormat(a - b).ToString();
                break;
            case "*":
                calculationResult = NumberToBigNumberFormat(a * b).ToString();
                break;
            case "/":
                if (b == 0)
                    calculationResult = "Cannot divide by zero";
                else
                    calculationResult = NumberToBigNumberFormat(a / b).ToString();
                break;
            default:
                calculationResult = "Unknown operation";
                break;
        }
    }

    private bool TryConvertNumberOrInvalidInput(string numberStr, out BigInteger result)
    {
        result = 0;
        if (string.IsNullOrEmpty(numberStr))
        {
            return true; // �Է°��� ����ְų� null�̸�, 0���� ó��
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(numberStr, @"^[0-9A-Z\.]+$"))
        {
            result = ConvertBackToNumber(numberStr);
            return true;
        }
        else
        {
            calculationResult = "Invalid Input";
            return false; // ��ȿ���� ���� �Է�
        }
    }

    private void ExtendSuffixes()
    {
        if (suffixes.LastOrDefault() == "")
        {
            suffixes[0] = "A";
        }
        else
        {
            string lastSuffix = suffixes.Last();
            char lastChar = lastSuffix.Last();
            if (lastChar < 'Z')
            {
                suffixes.Add(lastSuffix.Substring(0, lastSuffix.Length - 1) + (char)(lastChar + 1));
            }
            else
            {
                if (lastSuffix.All(c => c == 'Z'))
                {
                    suffixes.Add(new string('A', lastSuffix.Length + 1));
                }
                else
                {
                    int index = lastSuffix.Length - 1;
                    while (lastSuffix[index] == 'Z')
                    {
                        index--;
                    }
                    string nextSuffix = lastSuffix.Substring(0, index) + (char)(lastSuffix[index] + 1);
                    nextSuffix = nextSuffix.PadRight(lastSuffix.Length, 'A');
                    suffixes.Add(nextSuffix);
                }
            }
        }
    }
    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }
}