$(document).ready(function() {
    //Name for the id of the divs we want to change on
    var settingsName = "settings";
    var calenderName = "calendar";

    //Settings button 
    $("#Setting").click(function(){
        //If settings is not visible
        if (document.getElementById(settingsName).style.display === "none") {
            document.getElementById(settingsName).style.display = "block";
            document.getElementById(calenderName).style.width = "78%";
        }
        //If setting is visible
        else {
            document.getElementById(settingsName).style.display = "none";
            document.getElementById(calenderName).style.width = "90%";
        }
    });
});

