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

    private string inputString2 = ""; // 문자 -> 숫자
    private string resultString2 = ""; // 문자 -> 숫자


    private string numberA = ""; // 첫 번째 숫자 입력 필드
    private string numberB = ""; // 두 번째 숫자 입력 필드
    private string calculationResult = ""; // 계산 결과 필드

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
            // 입력값이 비어 있거나 null인 경우 검사
            if (string.IsNullOrEmpty(inputString2))
            {
                resultString2 = "0";
            }
            else
            {
                // 입력값에 유효하지 않은 문자가 포함되어 있는지 검사
                // 유효한 입력: 숫자, 소수점, 대문자 알파벳
                if (System.Text.RegularExpressions.Regex.IsMatch(inputString2, @"^[0-9A-Z\.]+$"))
                {
                    resultString2 = ConvertBackToNumber(inputString2).ToString();
                }
                else
                {
                    // 유효하지 않은 문자가 포함된 경우
                    resultString2 = "Invalid Input";
                }
            }

        }


        EditorGUILayout.LabelField("Converted Number:", resultString2, textAreaStyle);



        

        GUILayout.Space(10);
        DrawLine();

        GUILayout.Label("Big Number calculator", EditorStyles.boldLabel);

        // 사칙연산을 위한 추가 기능
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
        // 분리: 숫자 부분(소수점 포함)과 문자 부분
        var match = System.Text.RegularExpressions.Regex.Match(numberToString, @"(\d+(?:\.\d+)?)([A-Z]+)");

        if (!match.Success) return BigInteger.Parse(numberToString); 

        string numberPart = match.Groups[1].Value;
        string suffix = match.Groups[2].Value;

        // 소수점 처리
        int decimalPlaces = numberPart.Contains(".") ? numberPart.Length - numberPart.IndexOf(".") - 1 : 0;
        BigInteger baseNumber = BigInteger.Parse(numberPart.Replace(".", ""));

        // 문자 부분에 해당하는 지수 계산
        BigInteger exponent = 0;
        for (int i = 0; i < suffix.Length; i++)
        {
            int charPosition = suffix[i] - 'A' + 1;
            exponent = exponent * 26 + charPosition;
        }

        // 계산된 지수가 소수점 아래 자릿수보다 작은 경우, 오류 방지를 위해 조정
        int totalExponent = 3 * (int)exponent - decimalPlaces;
        if (totalExponent < 0)
        {
            // 음수 지수를 방지하기 위한 처리
            return BigInteger.Zero;
        }

        // 지수를 기반으로 하는 변환: 1A -> 1000, 1B -> 1000000, ...
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
            return; // 유효하지 않은 입력이 있으면 함수 종료
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
            return true; // 입력값이 비어있거나 null이면, 0으로 처리
        }
        else if (System.Text.RegularExpressions.Regex.IsMatch(numberStr, @"^[0-9A-Z\.]+$"))
        {
            result = ConvertBackToNumber(numberStr);
            return true;
        }
        else
        {
            calculationResult = "Invalid Input";
            return false; // 유효하지 않은 입력
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