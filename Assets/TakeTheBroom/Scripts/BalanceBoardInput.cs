﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/*
 * This script parses a textfile containing sensor data which was generated by an external python script.
 * The textfile is formatted "int int int int " and the data is ordered top, bottom, left, right.
 * The unit of each int in the textfile is kg on the specific side of the balance board.
 * Then the two directions of each axis are combined into one float and normalized by dividing by the total weight,
 * so the result values are floats between -1 and 1.
 */


public class BalanceBoardInput : MonoBehaviour
{
    public float x;
    public float y;

    public float accY;

    private string serverUrl = "http://192.168.0.105:5000/besen";
    //private string serverUrl = "http://132.199.196.245:5000/besen";

    private string dataString = "";

    //private const String pattern = @"(\d+)(\s+)(\d+)(\s+)(\d+)(\s+)(\d+)";

    // Use this for initialization
    void Start()
    {
        //Debug.Log("start bbi...");
        //StartCoroutine(ReadDataStringFromUrl(serverUrl));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("update bbi...");
        //StartCoroutine(ReadDataStringFromUrl(serverUrl));
        // read data file generated by python script
        //string balanceBoardDataString = System.IO.File.ReadAllText("balanceBoardData");
        StartCoroutine(ReadDataStringFromUrl(serverUrl));
        //string balanceBoardDataString = dataString;
        string wiiMoteDataString = dataString;

        /*
		try
		{
			// perform string parsing magic
			string[] subStrings = balanceBoardDataString.Split(' ');
			float[] balanceBoardData = {0, 0, 0, 0};
			float weightSum = 0;	// total weight of the player, used to normalize data
			for(int i = 0; i < 4; i++)
			{
				balanceBoardData[i] = (float)int.Parse(subStrings[i]);
				weightSum += balanceBoardData[i];
			}
			// the string is now parsed into a float[4] array
			// now combine the two directions into one axis and divide it by the total weight to get values between -1 and 1
			// and square them for reasons
			x = (balanceBoardData[2] - balanceBoardData[3]) / weightSum;
			y = (balanceBoardData[0] - balanceBoardData[1]) / weightSum;
			x *= Mathf.Abs(x);
			y *= Mathf.Abs(y);
			//Debug.Log(x);
		}
		catch(System.FormatException e)
		{
			// the file was not readable
		}
		*/

        try
        {
            //Debug.Log(wiiMoteDataString);
            // perform string parsing magic
            string[] lines = wiiMoteDataString.Split(' ');
            //string[] subStringsIR = lines[0].Split(' ');
            //string[] subStringsAcc = lines[1].Split(' ');
            //float[] irData = {0, 0};
            float[] accData = { 0, 0, 0 };
            //float weightSum = 0;	// total weight of the player, used to normalize data

            /*for(int i = 0; i < 2; i++)
			{
				irData[i] = (float)int.Parse(subStringsIR[i]);
			}*/

            for (int i = 0; i < 3; i++)
            {
                accData[i] = (float)int.Parse(lines[i]);
            }

            accY = accData[1];

            //Debug.Log(x);
        }
        catch (System.FormatException e)
        {
            // the file was not readable
        }

    }

    IEnumerator ReadDataStringFromUrl(string url)
    {
        //Debug.Log("reading...");
        WWW www = new WWW(url);
        yield return www;

        dataString = www.text;
        //Debug.Log(www.text);
    }
}