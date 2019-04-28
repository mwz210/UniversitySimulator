﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    //UI text
    [SerializeField] TextMeshProUGUI eventLog;
    [SerializeField] TextMeshProUGUI studentsText;
    [SerializeField] TextMeshProUGUI facultyText;
    [SerializeField] TextMeshProUGUI alumniText;
    [SerializeField] TextMeshProUGUI buildingsText;
	[SerializeField] TextMeshProUGUI wealthText;

    //Other UI Elements
    public Button playButton;
    public Text playText;

	//resources
	private int students;
	private int faculty;
	private int alumni;
	private int buildings;
	private float wealth;

	//other hidden resources
	private int buildingBonus = 0;
	private float growthBonus = 1.0f;
	private float difficulty = 1.0f;
	private int studentGrowth;
	private int popInitial; //initial student population

    //other variables
    private bool playing = true; //check if paused or not

    // Start is called before the first frame update
    void Start()
    {
    	//Resource list
        studentsText.text = "Students: " + students.ToString();
        facultyText.text = "Faculty: " + faculty.ToString();
        alumniText.text = "Alumni: " + alumni.ToString();
        buildingsText.text = "Buildings: " + buildings.ToString();
		wealthText.text = "Wealth: "+ wealth.ToString();

        //Button Setup //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        playButton.onClick.AddListener(PauseOnClick);

        //set up random ranges (possibly based on difficulty later)
        students = Mathf.FloorToInt(Random.Range(2.0f, 15.0f));
		faculty = Mathf.FloorToInt(Random.Range(1.0f, 5.0f));
		alumni = 1;
		buildings = 1;
		wealth = 0;
		popInitial = students;

		eventLog.text += "\n BREAKING: SADU's only alum has taken over for the school!";

        //A turn is done every second, with a 0.5 second delay upon resuming
        InvokeRepeating("Turns", 0.5f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
    	//check for game over
    	if (students <= 0) {
    		eventLog.text = "\n You've run out of students and this University has failed.";
    	}
    	else if (alumni >= 500000) {
    		eventLog.text = "\n Congrats! You have as much alumni as NYU! \n \n what else do you want. a cookie?";
    	}
        

        //Resource List to be updated
        studentsText.text = "Students: " + students.ToString();
        facultyText.text = "Faculty: " + faculty.ToString();
        alumniText.text = "Alumni: " + alumni.ToString();
        buildingsText.text = "Buildings: " + buildings.ToString();
		wealthText.text = "Wealth: "+wealth.ToString();
    }

    //take into account all policy changes and changes in resources, then update said resources
    void Turns() {
        //Check whether game is paused or not
        if (playing) {
            //calculate wealth and apply wealth difficulty
            wealth = students + faculty + alumni + buildings * 200;

            //calculate difficulty
            difficulty = ((students + alumni * 2 + faculty + buildings * 10) / wealth);

            wealth += Mathf.FloorToInt(alumni / 5) + students + buildingBonus;

            //student changes
            if ((students / 3 < faculty) && (students < 350 * buildings)) {
                //increase by growth rate
                int K = 350 * buildings;
                students += Mathf.FloorToInt(growthBonus * students * (1 - (students / K)));
            }

            //faculty changes
            if (wealth <= faculty) {
                faculty -= Mathf.FloorToInt(((faculty - wealth) / 2));
                wealth = 0;
            }
            else if (wealth <= 0) {
                students -= 2;
            }
            else {
                wealth -= faculty;
            }

            //alumni resource changes
            if (students <= 5) {
                alumni += students;
                students = 0;
            }
            else {
                int mod = Mathf.FloorToInt(students / 5);
                students -= mod;
                alumni += mod;
            }

            //events
            DoEvent();
        }
    }

    //Not currently working, view design doc
    void DoEvent() {
    	float rand = Random.Range(0.0f, difficulty);

    	//Random, dynamic difficulty events
    	if (rand <= 0.1) {
    		eventLog.text += "\n EVENT: A faculty member has published a great paper! \n New faculty have joined!";
    		faculty++;
    	}
    	else if (rand <= 0.4) {
    		if (rand <= 0.3) {
    			eventLog.text += "\n EVENT: Students have transferred from SADDERU \n Students have increased!";
    			students++;
    		}
    		else if (buildings > 1) {
    			eventLog.text += "\n EVENT: Two buildings bumped into each other, went on a date, and fell in love! \n A new building is born?";
    			buildings++;
    		}
    		else {
    			eventLog.text += "\n EVENT: A pipe has burst creating super athletes. \n Student growth has increased!";
    			growthBonus += 0.1f;
    		}
    	}
    	else if (rand <= 0.47) {
    		eventLog.text += "\n EVENT: A pipe has burst injuring and killing students!";
    		students -= 3;
    	}
    	else if (rand <= 0.69) {
    		eventLog.text += "\n EVENT: Students have used materials to buy equipment \n Materials lost!";
    		wealth -= Mathf.FloorToInt(wealth / 2.7f);
    	}
    	else if (rand <= 0.75) {
    		eventLog.text += "\n EVENT: Due to your great works, more students are joining! \n Student growth has increased!";
    		growthBonus += 0.1f;
    	}
    	else if (rand <= 0.85) {
    		eventLog.text += "\n EVENT: Due to rampant [REDACTED] bad things are happening! \n Faculty and Students lost!";
    		students -= 5;
    		faculty--;
    	}
    	else if (rand <= 1.0) {
    		eventLog.text += "\n EVENT: Ya just got taxed boi \n Materials lost!";
    		wealth -= Mathf.FloorToInt(wealth / 2.0f);
    	}
    	else if (rand <= 1.2) {
    		eventLog.text += "\n EVENT: Due to a bad Mathlete loss students grow disinterested \n Student growth decreased!";
    		growthBonus -= 0.2f;
    	}
    	else if (rand <= 1.4) {
    		eventLog.text += "\n EVENT: Increasing wealth of the school makes people mad \n Materials lost! \n Students lost!";
    		wealth -= 200;
    		students -= 10;
    	}
    	else if (rand <= 1.5) {
    		eventLog.text += "\n EVENT: Alumni have died on a trip to mars \n Alumni lost!";
    		alumni -= 50;
    	}
    	else {
    		eventLog.text += "\n BIG EVENT: 'Damn this school wealthy bruh lemme cop some of that' - President \n Materials halved!";
    		wealth = Mathf.FloorToInt(wealth / 2);
    	}
    }

    //Pause and Play button click function
    void PauseOnClick() {
        //change text when paused or playing
        if (playing) {
            playText.text = "Play";
            playing = !playing;
        }
        else {
            playText.text = "Pause";
            playing = !playing;
        }
    }
}
