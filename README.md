Unfortunately I had limited time to work on the project because of other personal assignments. I will describe my reasoning regarding various aspects of the case study and indicate when I had not enough time to make a proper implementation along with describing what should have been done and how.

Task 2: 
- I have decided to implement N:N relationship via custom table. Reasoning is that workshop participant is likely to be extended - invitation status, approvals etc. Out of the box N:N relationship would introduce unnecessary issues once such extension would be needed
- I have implemented date as date only. I realized later that it probably should be date and time. In normal world I would clarify it first before creating field. Due to time constraints I have not changed the column to date time
- There are a few inconsistencies with entities and fields naming conventions. Normally I would fix them and recreate fields but again, I had no time to do it, however I do acknowledge it
Task 3:
- I assume that date needs to be in future ie. not today. I do not consider user timezone - whatever user selects in calendar, should be matched with server time. Timezone of the server should have no impact as well since I am using only date component

Task 4: 
- I assumed possibility for multiple people to be selected at the same time which was an overkill, it would be better to limit to single participant or plan to display results accordingly, display errors in a reasonable way etc. Again normally I would have agreed on details first
- Button is visible only if at least one record is selected and if contact subgrid is rendered on workshop form. In case contact subgrid would be displayed on workshop form in multiple places, button display rules may need to be changed to make sure correct subgrid is targeted
- Additionally, contacts should be filtered to not show ones that already has been added. In reality the best solution for it would probably be PCF, alternatively retrieve multiple plugin but using retrieve multiple plugins is in my opinion something to avoid
- In my implementation participants will not be created if contact was already used to create participant in the past - linking multiple times is not possible, at least on frontend level as it is not safeguarded by plugins (and should be)

Additionally for javascripts, Typescript implementation would probably be better but I had no time to set it up

Task 5

Plugins in general:

- I had no time to setup early bounds correctly as well as to build well refined project skeleton with utility functions and correct base classes
- Ideally structure would be as follows:
	- Better custom PluginBase class that would encapsulate all the boilerplate code like checking for correct plugin registration, checking and extracting Target, Image, building context and other properties
	- Ideally, Plugin implementation should be generic associated with Target early bound class and contain only Execute method.
	- Execute method would contain set of registrations for each separate business logic and those business logics would be separated to another class
	- Each business logic would contain CanExecute and Execute methods that would handle checking for conditions to run the piece of logic and ultimately running it
	- Down the line of course, there would be Services, then Repositories and then model similarly to how its done already but perhaps a bit better polished
	- Of course using inline strings for entity names and fields is a mess and if I had more time I would rather use early bounds or alternatively generate typings and at least refer to those typings instead of static strings.

Plugin 1:

- There should be an additional plugin to make sure that if user changed MaxParticipants, it did not make it lower than CurrentParticipants
- If we allow to remove participants, it should also be reflected via plugin code to recalculate current participants
- There is no block in code to stop adding the same person as a participant again - should be considered
- Either we must assume that participants won't change their contact and workshop field values and ideally guard that in code or allow for such change but make sure that counts are recalculated properly

Plugin 2:

- Missing implementation of update/delete plugins completely. Logic is currently executed only for create
- Currently, nearby workshops are calculated only for target entity. In reality, it should also be calculated for all such nearby workshops for data to matched
- Also on update/delete, old nearby workshops should be updated accordingly
- There is no check for empty workshop name
- Recalculation of nearby workshops for all the workshops other than target should be done without relying on current value of current workshops to avoid race condition or locking/unlocking multiple records and reading 2 times

Task 6:

- There should be flag "EmailNotificationSent" and all records with flag set to yes should be excluded. Flag should be set to yes if notification was sent for given participant
- Right now flow is triggered once per day and checks for all workshops in range 2-3 days. Ideal implementation would include EmailNotificationSent flag and cloud flow can be executed more often than once per day and give timing more close to 2 days. Current implementation is leaky and may result in 2 notifications being sent or perhaps none at all
- Used some test gmail account as connector, normally would use something like shared mailbox
- Since there is no guard for single contact per workshop, it may happen that the same person will receive email more than once. Moreover, there is no check for empty email, empty contact on workshop participant and possibly more. Additionally there should be some error handling, perhaps using scopes.
