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
        },
        
        // IMPORTANT: Override submitHandler to prevent auto tab switching
        submitHandler: function(form) {
            // Let the form submit normally - don't prevent it
            form.submit();
        }
    });

    // SIMPLIFIED: Custom validation method for contact requirement
    $.validator.addMethod("contactrequired", function (value, element, params) {
        var phoneValue = $("#PhoneNumber").val().trim();
        var emailValue = $("#Email").val().trim();
        
        // At least one must have content
        return phoneValue.length > 0 || emailValue.length > 0;
    }, "Either Phone Number or Email must be provided.");

    // Custom validation method for SSN format
    $.validator.addMethod("ssnformat", function (value, element) {
        if (value === "") return false; // SSN is required, so empty is not valid
        return /^\d{3}-?\d{2}-?\d{4}$/.test(value);
    }, "Please enter a valid SSN format (XXX-XX-XXXX)");

    // Custom validation method for phone format - more lenient
    $.validator.addMethod("phoneformat", function (value, element) {
        if (value === "") return true; // Empty is OK, contactrequired handles the requirement
        // More flexible phone validation
        return /^[\+]?[1-9]?[\s\-\(\)]?[\d\s\-\(\)]{9,}$/.test(value.replace(/\s/g, ''));
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

    // SIMPLIFIED: Phone validation - only format validation, contact validation handled separately
    $("#PhoneNumber").rules("add", {
        phoneformat: true,
        messages: {
            phoneformat: "Please enter a valid phone number format"
        }
    });
    
    // SIMPLIFIED: Email validation - only format validation, contact validation handled separately
    $("#Email").rules("add", {
        email: true,
        messages: {
            email: "Please enter a valid email address format"
        }
    });

    // FIXED: Contact validation - only validate when both are empty
    function validateContact() {
        var phoneValue = $("#PhoneNumber").val().trim();
        var emailValue = $("#Email").val().trim();
        var phoneSpan = $("#PhoneNumber").closest('.row').find('span[data-valmsg-for="PhoneNumber"]');
        var emailSpan = $("#Email").closest('.row').find('span[data-valmsg-for="Email"]');
        
        if (phoneValue.length === 0 && emailValue.length === 0) {
            // Both are empty - show error on both
            if (!phoneSpan.text().includes("Either Phone Number or Email")) {
                phoneSpan.append('<div class="contact-error">Either Phone Number or Email must be provided.</div>');
            }
            if (!emailSpan.text().includes("Either Phone Number or Email")) {
                emailSpan.append('<div class="contact-error">Either Phone Number or Email must be provided.</div>');
            }
            $("#PhoneNumber, #Email").addClass('is-invalid');
            return false;
        } else {
            // At least one has content - clear contact errors
            phoneSpan.find('.contact-error').remove();
            emailSpan.find('.contact-error').remove();
            
            // Only remove invalid styling if no other errors exist
            if (phoneValue.length > 0 && phoneSpan.text().trim().length === 0) {
                $("#PhoneNumber").removeClass('is-invalid').addClass('is-valid');
            }
            if (emailValue.length > 0 && emailSpan.text().trim().length === 0) {
                $("#Email").removeClass('is-invalid').addClass('is-valid');
            }
            return true;
        }
    }

    // Contact validation on change
    $("#PhoneNumber, #Email").on('input blur keyup', function() {
        // First validate the individual field format
        validator.element(this);
        
        // Then validate contact requirement
        setTimeout(validateContact, 100);
    });
    
    // Validate other fields on blur
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
    
    // Validate contact requirement on form submission
    $("#user-form").on('submit', function(e) {
        if (!validateContact()) {
            e.preventDefault();
            return false;
        }
    });
    
    // Set max date for DOB input to today
    var today = new Date().toISOString().split('T')[0];
    $("#DOB").attr('max', today);
});