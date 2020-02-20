"use strict";
//Function to select all the checkbox
function allcheck(btnclicked, output) {

    $(btnclicked).click(function () {
        if (this.checked) {
            $(output).each(function () {
                this.checked = true;
            });
        } else {
            $(output).each(function () {
                this.checked = false;
            });
        }
    });
}

//Function for validation
function validationForm(inputValues, errorValues, validValue = /\w+/i) {
    
    var input = $(inputValues);
    var error = $(errorValues)
    input.removeClass("is-valid");
    input.removeClass("is-invalid");

    if (input.val() == null || input.val() == '') {
        input.addClass("is-invalid");
        //$(error).addClass("invalid-feedback");
        error.text("This is a Required Field");
        //console.log(input.value);
        //console.log("For null");
        return false;
    } else if (!input.val().match(validValue)) {
        $(input).addClass("is-invalid");
        //$(error).addClass("invalid-feedback");
        //console.log("For validation");
        error.text("Input values are not in valid format");    
        return false;
    } else {
       
        input.addClass("is-valid");
        console.log("success");
        //$(error).removeClass("invalid-feedback");
        error.text("");
        return true;
      
    }
}
