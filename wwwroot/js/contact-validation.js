$(document).ready(function () {
    var hasUserInteracted = false; // Track if user has interacted with the form
    
    // Initialize form validation
    var validator = $("#user-form").validate({
        onkeyup: function(element) {
            if (hasUserInteracted) {
                this.element(element);
            }
        },
        onfocusout: function(element) {
            if (hasUserInteracted) {
                this.element(element);
            }
        },
        
        errorPlacement: function (error, element) {
            var validationSpan = element.closest('.row').find('span[data-valmsg-for="' + element.attr('name') + '"]');
            if (validationSpan.length > 0) {
                error.appendTo(validationSpan);
            } else {
                error.insertAfter(element);
            }
        },
        
        highlight: function(element, errorClass, validClass) {
            $(element).addClass('is-invalid').removeClass('is-valid');
        },
        
        unhighlight: function(element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass('is-invalid');
            
            if ($element.val().trim().length > 0) {
                $element.addClass('is-valid');
            } else {
                $element.removeClass('is-valid');
            }
        },
        
        submitHandler: function(form) {
            form.submit();
        }
    });

    // Add custom validation method for contact requirement
    $.validator.addMethod("contactrequired", function (value, element, params) {
        var currentValue = $(element).val().trim();
        var otherPropertyName = params.otherproperty;
        var otherElement = $("#" + otherPropertyName.charAt(0).toUpperCase() + otherPropertyName.slice(1));
        var otherValue = otherElement.val().trim();
        
        // At least one must have content
        return currentValue.length > 0 || otherValue.length > 0;
    }, "Either Phone Number or Email must be provided.");

    // Add validation methods
    $.validator.addMethod("ssnformat", function (value, element) {
        if (value === "") return false;
        return /^\d{3}-?\d{2}-?\d{4}$/.test(value);
    }, "Please enter a valid SSN format (XXX-XX-XXXX)");

    $.validator.addMethod("notfuturedate", function (value, element) {
        if (value === "" || value === null) return true;
        var today = new Date();
        var inputDate = new Date(value);
        return inputDate <= today;
    }, "Date of Birth cannot be a future date");

    // Add validation rules
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
        messages: {
            notfuturedate: "Date of Birth cannot be a future date"
        }
    });

    
    function validateContactFields() {
        if (!hasUserInteracted) return;
        
        $("#PhoneNumber, #Email").each(function() {
            var $field = $(this);
            var $span = $field.closest('.row').find('span[data-valmsg-for="' + $field.attr('name') + '"]');
            $span.find('.contact-error').remove();
            $field.removeClass('is-invalid');
        });
        
        var phoneValue = $("#PhoneNumber").val().trim();
        var emailValue = $("#Email").val().trim();
        
        if (phoneValue.length === 0 && emailValue.length === 0) {
          
            $("#PhoneNumber, #Email").each(function() {
                var $field = $(this);
                var $span = $field.closest('.row').find('span[data-valmsg-for="' + $field.attr('name') + '"]');
                $span.append('<div class="contact-error">Either Phone Number or Email must be provided.</div>');
                $field.addClass('is-invalid').removeClass('is-valid');
            });
        } else {
            
            if (phoneValue.length > 0) {
                
                if (!/^[\+]?[1-9]?[\s\-\(\)]?[\d\s\-\(\)]{9,}$/.test(phoneValue.replace(/\s/g, ''))) {
                    var $phoneSpan = $("#PhoneNumber").closest('.row').find('span[data-valmsg-for="PhoneNumber"]');
                    $phoneSpan.append('<div class="contact-error">Please enter a valid phone number.</div>');
                    $("#PhoneNumber").addClass('is-invalid').removeClass('is-valid');
                } else {
                    $("#PhoneNumber").addClass('is-valid').removeClass('is-invalid');
                }
            }
            
            if (emailValue.length > 0) {

                if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(emailValue)) {
                    var $emailSpan = $("#Email").closest('.row').find('span[data-valmsg-for="Email"]');
                    $emailSpan.append('<div class="contact-error">Please enter a valid email address.</div>');
                    $("#Email").addClass('is-invalid').removeClass('is-valid');
                } else {
                    $("#Email").addClass('is-valid').removeClass('is-invalid');
                }
            }
        }
    }

    $("#PhoneNumber, #Email").on('input blur keyup', function() {
        hasUserInteracted = true; 
        validateContactFields();
    });

    $("#PhoneNumber, #Email").on('focus', function() {
        hasUserInteracted = true;
    });

    $("#Name, #SSN, #UserType, #DOB").on('blur change', function() {
        hasUserInteracted = true;
        validator.element(this);
    });

    $("#user-form input, #user-form select").on('focus', function() {
        hasUserInteracted = true; 
        var $this = $(this);
        if ($this.val().trim().length === 0) {
            $this.removeClass('is-valid is-invalid');
        }
    });
    
    var today = new Date().toISOString().split('T')[0];
    $("#DOB").attr('max', today);

    $("#user-form").on('submit', function() {
        hasUserInteracted = true;
        validateContactFields();
    });
});