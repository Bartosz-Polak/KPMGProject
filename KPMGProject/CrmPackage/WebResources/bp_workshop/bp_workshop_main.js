if (typeof JS === "undefined") JS = { _namespace: true };
if (typeof JS.Entities === "undefined") JS.Entities = { _namespace: true };
if (typeof JS.Entities.bp_workshop === "undefined") JS.Entities.bp_workshop = { _namespace: true };

const oneDay = 86400000;
const dateControl = "bp_date";


JS.Entities.bp_workshop = {
    formContext: null,
    formType: null,

    onLoad: (executionContext) => {
        const formContext = executionContext.getFormContext();

        bp_workshopForm.clearValidationNotifications(formContext);
    },

    onChange_bp_workshop_date: (executionContext) => {
        const formContext = executionContext.getFormContext();

        bp_workshopForm.validateDate(formContext);

    },

    clearValidationNotifications: (formContext) => {
        formContext.getControl("bp_date").clearNotification();
    },

    validateDate: (formContext) => {
        const dateAttribute = formContext.getAttribute("bp_date");

        if (!dateAttribute) {
            console.error("Attribute 'bp_date' not found on the form. Please add attribute to the form");
            return;
        }

        let chosenDate = dateAttribute.getValue();
        chosenDate.setHours(0, 0, 0, 0);
        let currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        const tomorrowDate = new Date(currentDate.getTime() + 86400000);

        if (chosenDate >= tomorrowDate) {
            formContext.getControl("bp_date").clearNotification();
            return;
        }

        formContext.getControl("bp_date").setNotification("Date must be in future");
    }

}

const bp_workshopForm = JS.Entities.bp_workshop;