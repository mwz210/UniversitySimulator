# University Simulator: Build and grow a University!

### Game Phases
## Early Game: 1k students required, 5k wealth recommended to proceed

**Premise:**

	The university is brand new and doesn't have enough students to be recognized as a university yet! The school relies on local highschools to funnel students into SADU. Starting with generated high schools every couple of turns, SADU picks new agreements that can be upgrades or downgrades.

**Focus:**
	
	* Tuition Rate Slider - Affects Happiness and Wealth. Higher Tuition rate decreases happiness and increases wealth per turn.
	* Acceptance Rate Slider - Determines how many of the applied students will enter the University. Higher acceptance rate increases students but decreases renown per turn.
	* Highschool Agreements - New agreements appear each turn and must be purchased. Agreements affect the max pool of students who can apply, as well as renown.
	* Alumni Donation Rate: Affect wealth gained by Alumni. Setting it too high when happiness is low will cause Alumni to denounce the university and alumni number decreases
	
**Details:**

- Student Body: Main Goal
- Wealth: Soft Goal
- Happiness: Affects number of students who apply to the university
- Graduation Rate: Static in this phase, set very low
- Buildings: Given 3 at the start, you cannot buy new buildings in this stage
- Renown: Affects growth rate of faculty
- Students to Faculty Ratio: The number of students each faculty can take care of. Static in this stage, set to 10.
- Available Upgrades: Hire Administrators (Unlocks visibility of hidden resources), Official Campus (Needed to unlock next phase), Official University Certificate (needed to unlock next phase)
	
**Lose Condition:**
- No wealth or students

**Win Condition:**
- 1k Students unlocks two required purchases.
- To move on, you need to purchase **Official Campus** costing 3K wealth and **Official University Certificate** costing 2K wealth

## Mid Game:

**Premise:**
	
	Now that the University is official, it can grow by buying buildings, hiring specialists, and no longer needs high school agreements. The university needs to rise in the rankings, so management of the renown of the University becomes vital.

**Focus:**

	* Building Management - Introduces clickable tilemap mechanic, building management tab, and StudentPool and Faculty is now capped by the buildings you have
		- Buildings have different types, that come with different bonuses
	* Faculty Pay Slider - Affects 

**Details:**

**Lose Condition:**
- No wealth or students
- Fall out of national ranking (only a lose condition once you make it halfway through the ranking)

**Win Condition:**
- Become number 1 ranked national university

## End Game: *TO BE DESIGNED*

### Events

	Done using the EventController script. Uses an event ticker that counts down the time since the last event.
	Picks a random value in a range. When the ticker hits that number an event happens.
	Events can be helpful or damaging. Currently based on a random value

### UI/UX

- Tabs on the bottom of the screen for **Event Log** and one for each resource to manage them.
- The **Event Log** tab, will open or close a transparent Event Log Object in the top middle of the screen.
- **Event Log** will be a "simple messaging system which will allow items in our projects to subscribe to events, and have events trigger actions in our games. This will reduce dependencies and allow easier maintenance of our projects."
- **Play / Pause** button will be on the bottom right above the tabs
- **Resources** will be in the top left
- Tabs include a **Buy Menu**, an **Advanced Stats** tab, and an **Interactables** tab.

### Balance and Difficulty:

Rules for resources (every turn):
	Look at Google Slides Chart

Types of Resources:

1. Students - Determining growth counter. Grows depending on certain resources (algorithm below). Provides materials in terms of yearly tuition.

2. Alumni - A counter for how many students have graduated. Each alumni grants a small amount materials as donations

3. Buildings - Determines how much Faculty you can accumulate. Costs donations to purchase more. Buildings can provide a building bonus, whether positive or negative

4. Faculty - Determines how many Students you can have at one time. Can be purchased.

5. Wealth - Resource used to spend used for actions

Hidden Resources:

1. Student Growth (r) - Determines how many students you get per turn

2. Happiness - Factors into student growth

3. Renown

4. Student Pool - Determines max number of students you can get per turn