//This checks if the site is ready
$(document).ready(function() {

        //New vue object   
        var settingsVue = new Vue({

            //Which html element it is going to keep track at
            el: '#settings',
            //Data it stores
            data: {
                //Data here, maybe one for every value in the settings model?
                setting:{
                    id:0,
                    userId: '',
                    darkMode: false,
                    startTime: '',
                    endTime:'',
                    visibleTimeTo:'',
                    visibleTimeFrom:'',
                }
            },
            methods: {
                // Functions here probably one for each get and set value?
                setDarkMode: function () {
                    var self = this;

                    //get the darkMode value from checkbox
                    var darkmode = document.getElementById("darkmode").checked;
                    
                    //Set the correct value for darkMode
                    self.setting.darkMode = darkmode;
                    
                    axios.put('/api/calendar/settings/'+ self.setting.id, self.setting).then(function(res) {
                        //Set the value it is changed to
                        self.setting = res.data;
                        
                        var dm = "darkmode";
                        
                        if (self.setting.darkMode){
                            $('body').addClass(dm);
                            $('nav').removeClass("bg-light").addClass( "bg-dark");
                            $("#settings").removeClass("settingLight").addClass("settingDark");
                            $('button').removeClass("btn-primary").addClass("btn-secondary");
                            $('#calendar').addClass("calDark");
                            $('.fc-today').addClass("darkCalendar");
                            $('.modal-content').addClass("darkCalendar");
                        }  
                        else{
                            $('body').removeClass(dm);
                            $('nav').removeClass( "bg-dark").addClass( "bg-light");
                            $("#settings").removeClass("settingDark").addClass("settingLight");
                            $('button').removeClass("btn-secondary").addClass("btn-primary");
                            $("#calendar").removeClass("calDark");
                            $('.fc-today').removeClass("darkCalendar");
                            $('.modal-content').removeClass("darkCalendar");
                        }
                    });
                },

                setStartTime: function () {
                    var self = this;

                    //Get the start time from the input field
                    var time = $("#starttime").val();

                    //Set the correct value for starttime
                    self.setting.startTime = "1970-01-01T" + time + ":00";
                    
                    axios.put('/api/calendar/settings/' + self.setting.id, self.setting).then(function(res) {
                        self.setting = res.data;
                    });
                },
                setEndTime: function () {
                    var self = this;

                    //Get the start time from the input field
                    var time = $("#endtime").val();

                    //Set the correct value for starttime
                    self.setting.endTime = "1970-01-01T" + time + ":00";

                    axios.put('/api/calendar/settings/' + self.setting.id, self.setting).then(function(res) {
                        self.setting = res.data;
                    });
                },
                setVisibleTimeTo: function () {
                    var self = this;

                    //Get the start time from the input field
                    var time = $("#visibletimeto").val();

                    //Set the correct value for starttime
                    self.setting.visibleTimeTo = "1970-01-01T" + time + ":00";

                    axios.put('/api/calendar/settings/' + self.setting.id, self.setting).then(function(res) {
                        self.setting = res.data;
                        globalConnectionToCal.setOption('maxTime',  self.setting.visibleTimeTo.slice(11, -3));
                    });
                },
                setVisibleTimeFrom: function () {
                    var self = this;

                    //Get the start time from the input field
                    var time = $("#visibletimefrom").val();

                    //Set the correct value for starttime
                    self.setting.visibleTimeFrom = "1970-01-01T" + time + ":00";

                    axios.put('/api/calendar/settings/' + self.setting.id, self.setting).then(function(res) {
                        self.setting = res.data;
                        globalConnectionToCal.setOption('minTime',  self.setting.visibleTimeFrom.slice(11, -3));
                    });
                }
            },
            //When creating this object do this, aka constructor
            //Before is like a constructor. So maybe load in data from the settings Api here? 
            created: function() {
                var self = this;

                //Get all settings
                axios.get('/api/calendar/settings').then(async function (res) {

                    //Set all settings
                    self.setting = res.data;

                    //Set the buttons to the correct values

                    //Set darkMode check to the right value 
                    $('#darkmode').prop('checked', self.setting.darkMode);

                    //Add darkmode to view
                    if (self.setting.darkMode) {
                        $('body').addClass("darkmode");
                        $('.modal-content').addClass("darkCalendar");
                    }

                    //Set starttime 
                    var st = self.setting.startTime.slice(11, -3);

                    $('#starttime').val(st);

                    //Set endtime
                    var st = self.setting.endTime.slice(11, -3);

                    $('#endtime').val(st);

                    //Set visibleTimeTo
                    var st = self.setting.visibleTimeTo.slice(11, -3);

                    $('#visibletimeto').val(st);

                    //Set visibleTimeTo
                    var st = self.setting.visibleTimeFrom.slice(11, -3);

                    $('#visibletimefrom').val(st);

                    //Add event listner for done with calendar setup when that is done run this function
                    //This work since this run before the calendar setup is done
                    document.addEventListener("doneWithCalSetup", function() {
                        globalConnectionToCal.setOption('minTime',  self.setting.visibleTimeFrom.slice(11, -3));
                        globalConnectionToCal.setOption('maxTime',  self.setting.visibleTimeTo.slice(11, -3));
                        if (self.setting.darkMode) {
                            $('button').removeClass("btn-primary").addClass("btn-secondary");
                            $('nav').addClass("bg-dark", "text-light");
                            $("#settings").removeClass("settingLight").addClass("settingDark");
                            $('.fc-today').addClass("darkCalendar");
                        }
                    });
                });
                
            }
        });
});
