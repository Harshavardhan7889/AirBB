$(document).ready(function () {
    // Initialize form validation first
    var validator = $("#residence-form").validate({
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

    // Custom validation method for alphanumeric with spaces
    $.validator.addMethod("alphanumericspaces", function (value, element) {
        if (value === "") return true; // Let required handle empty values
        return /^[a-zA-Z0-9\s]+$/.test(value);
    }, "Name can only contain letters, numbers, and spaces");

    // Custom validation method for bathroom format
    $.validator.addMethod("bathroomformat", function (value, element) {
        if (value === "" || value === null) return true;
        var num = parseFloat(value);
        if (isNaN(num)) return false;
        var fractionalPart = num % 1;
        return fractionalPart === 0 || fractionalPart === 0.5;
    }, "Bathroom number must be a whole number or end with .5 (e.g., 1, 1.5, 2)");

    // Custom validation method for build year (dynamic range based on current year)
    $.validator.addMethod("buildyear", function (value, element, params) {
        if (value === "" || value === null) return true;
        var year = parseInt(value);
        if (isNaN(year)) return false;
        var maxYear = parseInt(params.max);
        var minYear = parseInt(params.min);
        return year >= minYear && year <= maxYear;
    }, "");

    // Unobtrusive adapter for buildyear validation
    $.validator.unobtrusive.adapters.add("buildyear", ["max", "min"], function (options) {
        options.rules["buildyear"] = {
            max: options.params.max,
            min: options.params.min
        };
        options.messages["buildyear"] = options.message;
    });

    // Add validation rules for all fields
    $("#Name").rules("add", {
        required: true,
        maxlength: 50,
        alphanumericspaces: true,
        messages: {
            required: "Name is required",
            maxlength: "Name must be less than 50 characters",
            alphanumericspaces: "Name can only contain letters, numbers, and spaces"
        }
    });

    $("#LocationID").rules("add", {
        required: true,
        min: 1,
        messages: {
            required: "Please select a location",
            min: "Please select a location"
        }
    });

    $("#ClientID").rules("add", {
        required: true,
        min: 1,
        messages: {
            required: "Please select an owner",
            min: "Please select an owner"
        }
    });

    $("#GuestNumber").rules("add", {
        required: true,
        range: [1, 50],
        messages: {
            required: "Guest number is required",
            range: "Guest number must be between 1 and 50"
        }
    });

    $("#BedroomNumber").rules("add", {
        required: true,
        range: [0, 20],
        messages: {
            required: "Bedroom number is required",
            range: "Bedroom number must be between 0 and 20"
        }
    });

    $("#BathroomNumber").rules("add", {
        required: true,
        range: [0.5, 20],
        bathroomformat: true,
        messages: {
            required: "Bathroom number is required",
            range: "Bathroom number must be between 0.5 and 20",
            bathroomformat: "Bathroom number must be a whole number or end with .5"
        }
    });

    // BuildYear validation is handled by data attributes from the custom validation attribute
    // No need to add rules here as it will dynamically calculate the range

    $("#PricePerNight").rules("add", {
        required: true,
        range: [0.01, 10000],
        messages: {
            required: "Price per night is required",
            range: "Price must be between $0.01 and $10,000"
        }
    });

    // Real-time validation on input change
    $("#residence-form input, #residence-form select").on('input blur change', function() {
        validator.element(this);
    });
    
    // Clear validation styling on focus for empty fields
    $("#residence-form input, #residence-form select").on('focus', function() {
        var $this = $(this);
        if ($this.val().trim().length === 0) {
            $this.removeClass('is-valid is-invalid');
        }
    });
});