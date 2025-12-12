if (typeof JS === "undefined") JS = { _namespace: true };
if (typeof JS.Ribbon === "undefined") JS.Ribbon = { _namespace: true };
if (typeof JS.Ribbon.Contact === "undefined") JS.Ribbon.Contact = { _namespace: true };

JS.Ribbon.Contact = {
    onClickAddParticipant: async (executionContext, selectedRecordIds) => {
        await ContactRibbon.addParticipants(executionContext, selectedRecordIds);
    },

    addParticipants: async (executionContext, selectedRecordIds) => {
        try {
            const workshopId = executionContext.entityReference.id.replace(/[{}]/g, "");
            // do not add contact if it is already participant
            const filteredRecordIds = await ContactRibbon.filterContacts(workshopId, selectedRecordIds);

            const failedCount = await ContactRibbon.addNewParticipants(workshopId, filteredRecordIds, selectedRecordIds);
            const skippedCount = selectedRecordIds.length - filteredRecordIds.length;
            const successfulCount = selectedRecordIds.length - failedCount - skippedCount;

            // may consider more detailed report in the dialog or reverting changes if any failure occurs
            let alertDialogText = "Added " + successfulCount + " participants, Skipped " + skippedCount + " contacts, " + "Errors: " + failedCount;
            ContactRibbon.showDialog(alertDialogText, "Confirmation");
            ContactRibbon.refreshSubgrid();
        }
        catch (error) {
            console.log(error.message);
            ContactRibbon.showDialog("Error", "An error occurred while adding participants.");
            return;
        }
    },

    filterContacts: async (workshopId, selectedRecordIds) => {
        const filteredContacts = selectedRecordIds.slice(); // Create a copy of the array to avoid modifying the original
        const existingParticipants = (await ContactRibbon.retrieveExistingParticipants(workshopId)).entities;
        if (!existingParticipants || existingParticipants.length === 0) {
            return filteredContacts;
        }
        for (var i = 0; i < existingParticipants.length; i++) {
            const entity = existingParticipants[i];
            const contactId = entity["_bp_contact_value"];
            const index = filteredContacts.indexOf(contactId);
            if (index > -1) {
                console.log("Contact with ID " + contactId + " is already a participant.");
                filteredContacts.splice(index, 1);
            }
        }
        return filteredContacts;
    },

    retrieveExistingParticipants: async (workshopId) => {
        // I assume that number of participants will not exceed 5000, otherwise paging would be needed
        return await Xrm.WebApi.retrieveMultipleRecords("bp_workshopparticipant", "?$select=_bp_contact_value&$filter=bp_WorkshopID/bp_workshopid eq " + workshopId);
    },

    addNewParticipants: async (workshopId, filteredRecordIds) => {
        let failedCount = 0;
        for (var j = 0; j < filteredRecordIds.length; j++) {
            const contactId = filteredRecordIds[j];
            console.log("Adding contact with ID " + contactId + " as a participant.");
            var data =
            {
                "bp_Contact@odata.bind": "/contacts(" + contactId + ")",
                "bp_WorkshopID@odata.bind": "/contacts(" + workshopId + ")"
            }

            try {
                //may consider promise.all for better performance or batch request
                await Xrm.WebApi.createRecord("bp_workshopparticipant", data);
            }
            catch (error) {
                console.log("Error adding contact with ID " + contactId + " as a participant: " + error.message);
                failedCount++;
            }
        }
        return failedCount;
    },

    createWorkshopParticipant: async (contactId, workshopId) => {
        var data =
        {
            "bp_Contact@odata.bind": "/contacts(" + contactId + ")",
            "bp_WorkshopID@odata.bind": "/contacts(" + executionContext.entityReference.id.replace(/[{}]/g, "") + ")"
        }

        // create account record
        await Xrm.WebApi.createRecord("bp_workshopparticipant", data);
    },

    showDialog: (dialogText, dialogTitle) => {
        var alertStrings = { confirmButtonLabel: "OK", text: dialogText, title: dialogTitle };
        var alertOptions = { height: 120, width: 260 };
        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
            function () {
                console.log("Alert dialog closed");
            },
            function (error) {
                console.log(error.message);
            }
        );
    },

    refreshSubgrid: () => {
        var subgrid = Xrm.Page.ui.controls.get("subgrid_participants", "Success");
        subgrid.refresh();
    }
}

const ContactRibbon = JS.Ribbon.Contact;