﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManagerScript : MonoBehaviour {
    public static GameManagerScript instance;

    // For testing purposes
    //public TextMeshProUGUI r_rate;
    //public TextMeshProUGUI k_rate;

	//resources
    public Resources resources;
    
    //Per Turn Display Stats
    public Resources resourcesDelta;

    //sliders
    public Slider tuitionSlider;
    public Slider donationSlider;
    public Slider acceptanceRateSlider;
    public Transform sliderContentPanel; //These sliders need to be added dynamically once the right game phase is active
    public GameObject salarySliderPrefab;
    private Slider salarySlider; 
    public GameObject facultyRatioSliderPrefab;
    private Slider facultyRatioSlider;

    //Ticker/Time variabless
    public int ticker = 0;
    private int eventTicker = 0; //time between events, resets after every event
    private int agreementTicker = 0; //time between new purchasable HS agreements
    private int eventThreshold; //time until events, changes after every event
    private int agreementThreshold; //time until new purchasable HS agreements
    private int negativeWealthTicker = 5;

    //other variables
    public bool playing = true; //check if paused or not
    public enum GameState {EarlyGame, MidGame, EndGame};
    public GameState state;
    [HideInInspector] // prevent this from being selectable in the inspector
    public EventController eventController; //script for events

    //buyables
    public Transform contentPanel; //The content object that we're attaching upgrade buttons to
    public GameObject upgradeButton; //Button prefab for the upgrade object
    public List<UpgradeBase> upgradeList;

    //EarlyGame Resources
    public HighSchoolAgreement[] agreements; //purchasable agreements
    public BuyAgreementScript BuyHSA1;
    public BuyAgreementScript BuyHSA2;
    public BuyAgreementScript BuyHSA3;
    public bool enableStatistics = false;
    public int earlyGameRequirements = 0;

    private void Awake() {
        if (GameManagerScript.instance == null) {
            GameManagerScript.instance = this;
        } else {
            Destroy(this);
        }

        eventController = GetComponent<EventController> ();
    }

    // Start is called before the first frame update
    void Start() {
        /*
            To add events, use eventController.DoEvent(new Event("Sample String"));
            Max lines can be changed in the editor
        */

        resources = new Resources();
        state = GameState.EarlyGame; //Set early game state, now disable all features not available in the early game

        //Initial upgrades that are available
        upgradeList = new List<UpgradeBase> ();
        UpgradeAdministrator upgradeAdmin = new UpgradeAdministrator();
        AddUpgradable(upgradeAdmin); //Add Hire Administrators upgrade


        //set up  ranges (possibly based on difficulty later)
        this.resources.students = 45;
		this.resources.faculty = 10;
		this.resources.alumni = 1;
		this.resources.wealth = 50;

        //initial purchasable agreements
        agreements = new HighSchoolAgreement[3];

        //Start timer thresholds
        eventThreshold = Random.Range(2, 10);
        agreementThreshold = Random.Range(15, 30);

        //run generation function for initial agreements
        string[] name = RandomAgreements.instance.ChooseName(3);
        for (int i = 0; i < 3; i++) {
            agreements[i] = RandomAgreements.instance.generateAgreement(name[i]);
        }

        //starting dialogue
        this.eventController.DoEvent(new Event("BREAKING: Crazy person declares themselves alumnus for non-existent University!"));

        //A turn is done every second, with a 0.5 second delay upon resuming
        InvokeRepeating("Turns", 0.5f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Debugging
        // Debug.Log("R Value: " + this.resources.r);
        // Debug.Log("K Value: " + this.resources.K);
        //r_rate.text = "R: " + this.resources.r_rate.ToString();
        //k_rate.text = "K: " + this.resources.k_rate.ToString();

        //pause control by pressing key
        if (Input.GetKeyDown(KeyCode.P)) {
            this.playing = !this.playing;
        }
    }

    //take into account all policy changes and changes in resources, then update said resources
    void Turns() {
        //Check whether game is paused or not
        if (playing) {
            //ticker values
            //eventController.DoEvent();
            ticker++;
            eventTicker++;
            agreementTicker++;

            //acceptance rate
            this.resources.calcAcceptanceRate(acceptanceRateSlider.value);

            //happiness. Optimal value is currently set to half the max value
            this.resources.calcHappiness(tuitionSlider.value, tuitionSlider.maxValue, donationSlider.value, donationSlider.maxValue);

            //Calculate wealth
            this.resourcesDelta.wealth = this.resources.calcWealth(donationSlider.value, tuitionSlider.value);

            //Calculate Faculty
            this.resourcesDelta.faculty = this.resources.calcFaculty();

            //Calculate Students
            this.resourcesDelta.students = this.resources.calcStudents(tuitionSlider.maxValue + donationSlider.maxValue);

            //Calculate Alumni
            this.resourcesDelta.alumni = this.resources.calcAlumni();

            //calculate HS Agreements, only done in early game
            if (state == GameState.EarlyGame) {
                this.resources.calcHSAgreements();
            }            

            //CODE FOR UPGRADES
            //Unlocking Early Game Upgrades, make sure they aren't already added
            if (this.resources.students > 1000 && upgradeList.Count < 2) {
                //Add the Buy Campus and Buy License Upgrades
                UpgradeCampus campusUpgrade = new UpgradeCampus();
                AddUpgradable(campusUpgrade);

                UpgradeLicense licenseUpgrade = new UpgradeLicense();
                AddUpgradable(licenseUpgrade);
            }

        }
        // ALL CODE BELOW IS OUTSIDE OF THE TICKER AND WILL BE RUN EVERY SECOND

        //Events
        if (eventTicker == eventThreshold) {
            //regenerate event threshold, reset time ot next event, and then do an event
            eventThreshold = Random.Range(5, 20); //use this to change time between events
            eventTicker = 0;
            //eventController.DoEvent();
        }

        //Future Event Code Here: Checks for bad stats (if happiness is too low do an event letting you know that people are unhappy)

        //randomized agreements, made sure it's only for the early game
        if (state == GameState.EarlyGame) {
            if (agreementTicker == agreementThreshold) {
                this.eventController.DoEvent(new Event("!!!: New HS Agreements are available!"));
                Debug.Log("New HS Agreements");

                //run generation function
                string[] name = RandomAgreements.instance.ChooseName(3);
                for (int i = 0; i < 3; i++) {
                    agreements[i] = RandomAgreements.instance.generateAgreement(name[i]);
                }

                agreementThreshold = Random.Range(4, 18); //use this to change time between new agreements
                agreementTicker = 0;

                //enable every window if they were purchased before
                BuyHSA1.gameObject.SetActive(true);
                BuyHSA2.gameObject.SetActive(true);
                BuyHSA3.gameObject.SetActive(true);
            }
        }

        //check for game over, or game win
        if (resources.students <= 0 && state == GameState.EarlyGame) {
            this.eventController.DoEvent(new Event("You've run out of students and this University has failed. \n Don't be sad it happened be happy it's over"));
            CancelInvoke();
        }
        //check if early game is finished
        if (earlyGameRequirements == 2) {
            state = GameState.MidGame;
            MoveToEarlyGame();
            //Unlock buildings, code required below

        }

        // when wealth is negative increase ticker.
        if (resources.wealth < 0) {
            negativeWealthTicker -= 1;
            this.eventController.DoEvent(new Event("!!! You are currently in debt. Recover your debt before the collectors shutdown the University. \nYou have " + negativeWealthTicker + "left."));
        }
        if (negativeWealthTicker < 0) {
            this.eventController.DoEvent(new Event("You have been in debt for more than 5 turns and the collectors are at your door. \n This university has failed."));
            CancelInvoke();
        }

    }

    //Add A Purchasable Upgrades
    void AddUpgradable(UpgradeBase item) {
        //Create button prefab and attach it to the content panel
        GameObject buttonCreation = Instantiate(upgradeButton, contentPanel);
        UpgradeBuyButton buttonScript = buttonCreation.GetComponent<UpgradeBuyButton> ();
        buttonScript.Setup(item); //pass upgrade object into the button

        upgradeList.Add(item);
    }

    //Code when moving to the MIDGAME
    void MoveToEarlyGame() {
        //Create sliders and attach them to their content panel (IDK if this works cuz i need to pass the early game to test it)
        GameObject sliderCreation = Instantiate(salarySliderPrefab, sliderContentPanel);
        salarySlider = sliderCreation.GetComponent<Slider> ();
        this.eventController.DoEvent(new Event("NEW POLICIES: Faculty Salary determines how much you pay faculty.\n A higher amount decreases wealth, but increases renown and happiness."));
        GameObject sliderCreation2 = Instantiate(facultyRatioSliderPrefab, sliderContentPanel);
        facultyRatioSlider = sliderCreation2.GetComponent<Slider> ();
        this.eventController.DoEvent(new Event("NEW POLICIES: Student-Faculty decides how many students a faculty can handle.\n Higher amount increases graduation rate but decreases happiness."));

        
    }
}
