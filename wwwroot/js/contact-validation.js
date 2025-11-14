$(document).ready(function () {
    // Initialize form validation first
    var validator = $("#user-form").validate({
        // Validate on blur, keyup, and click events for real-time validation
        onkeyup: function(element) {
            this.element(element);
        },
        onfocusout: function(element) {
            this.element(element);
        },
        onclick: function(element) {
            this.element(element);
        },
        
        // Custom error placement
        errorPlacement: function (error, element) {
            // Find the validation span for this element
            var validationSpan = element.closest('.row').find('span[data-valmsg-for="' + element.attr('name') + '"]');
            if (validationSpan.length > 0) {
                error.appendTo(validationSpan);
            } else {
                // Fallback to default placement
                error.insertAfter(element);
            }
        },
        
        // Custom highlighting
        highlight: function(element, errorClass, validClass) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        
        unhighlight: function(element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass('is-invalid');
            
            // Only show green checkmark if field has content and is valid
            if ($element.val().trim().length > 0) {
                $element.addClass('is-valid');
            } else {
                $element.removeClass('is-valid');
            }
        }
    });

    // Custom validation method for contact requirement
    $.validator.addMethod("contactrequired", function (value, element, params) {
        var phoneValue = $("#PhoneNumber").val().trim();
        var emailValue = $("#Email").val().trim();
        return phoneValue.length > 0 || emailValue.length > 0;
    }, "Either Phone Number or Email must be provided.");

    // Custom validation method for SSN format
    $.validator.addMethod("ssnformat", function (value, element) {
        if (value === "") return true; // Let required handle empty values
        return /^\d{3}-?\d{2}-?\d{4}$/.test(value);
    }, "Please enter a valid SSN format (XXX-XX-XXXX)");

    // Custom validation method for phone format
    $.validator.addMethod("phoneformat", function (value, element) {
        if (value === "") return true; // Let contactrequired handle empty values
        return /^[\+]?[1-9]?[\s\-\(\)]?[\d\s\-\(\)]{10,14}$/.test(value);
    }, "Please enter a valid phone number format");

    // Custom validation method for DOB - not future date
    $.validator.addMethod("notfuturedate", function (value, element) {
        if (value === "" || value === null) return true; // Allow empty values
        var today = new Date();
        var inputDate = new Date(value);
        return inputDate <= today;
    }, "Date of Birth cannot be a future date");

    // Custom validation method for DOB - reasonable date (not too old)
    $.validator.addMethod("reasonabledate", function (value, element) {
        if (value === "" || value === null) return true; // Allow empty values
        var maxDate = new Date();
        maxDate.setFullYear(maxDate.getFullYear() - 150);
        var inputDate = new Date(value);
        return inputDate >= maxDate;
    }, "Please enter a valid Date of Birth");

    // Add validation rules for all fields
    $("#Name").rules("add", {
        required: true,
        maxlength: 100,
        messages: {
            required: "Name is required",
            maxlength: "Name cannot be longer than 100 characters"
        }
    });

    $("#SSN").rules("add", {
        required: true,
        ssnformat: true,
        messages: {
            required: "SSN is required",
            ssnformat: "Please enter a valid SSN format (XXX-XX-XXXX)"
        }
    });

    $("#UserType").rules("add", {
        required: true,
        messages: {
            required: "User Type is required"
        }
    });

    $("#DOB").rules("add", {
        notfuturedate: true,
        reasonabledate: true,
        messages: {
            notfuturedate: "Date of Birth cannot be a future date",
            reasonabledate: "Please enter a valid Date of Birth"
        }
    });

    // Phone and Email validation (existing code)
    $("#PhoneNumber").rules("remove", "required");
    $("#PhoneNumber").rules("add", {
        contactrequired: true,
        phoneformat: true,
        messages: {
            contactrequired: "Either Phone Number or Email must be provided",
            phoneformat: "Please enter a valid phone number format"
        }
    });
    
    $("#Email").rules("remove", "required");
    $("#Email").rules("add", {
        contactrequired: true,
        email: true,
        messages: {
            contactrequired: "Either Phone Number or Email must be provided",
            email: "Please enter a valid email address format"
        }
    });

    // Real-time validation on input change
    $("#PhoneNumber, #Email").on('input blur', function() {
        validator.element("#PhoneNumber");
        validator.element("#Email");
    });
    
    // Validate other fields on blur (including DOB)
    $("#Name, #SSN, #UserType, #DOB").on('blur', function() {
        validator.element(this);
    });
    
    // Validate dropdown and date input on change
    $("#UserType, #DOB").on('change', function() {
        validator.element(this);
    });
    
    // Clear validation styling on focus for empty fields
    $("#user-form input, #user-form select").on('focus', function() {
        var $this = $(this);
        if ($this.val().trim().length === 0) {
            $this.removeClass('is-valid is-invalid');
        }
    });
    
    // Set max date for DOB input to today
    var today = new Date().toISOString().split('T')[0];
    $("#DOB").attr('max', today);
});