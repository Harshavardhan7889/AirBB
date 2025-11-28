$(document).ready(function () {
    // Initialize form validation first
    var validator = $("#location-form").validate({
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

    // Add validation rules for location name
    $("#Name").rules("add", {
        required: true,
        maxlength: 100,
        messages: {
            required: "Location name is required",
            maxlength: "Location name must be less than 100 characters"
        }
    });

    // Real-time validation on input change
    $("#location-form input").on('input blur change', function() {
        validator.element(this);
    });
    
    // Clear validation styling on focus for empty fields
    $("#location-form input").on('focus', function() {
        var $this = $(this);
        if ($this.val().trim().length === 0) {
            $this.removeClass('is-valid is-invalid');
        }
    });

    // Force validation on page load for fields with values
    $("#location-form input").each(function() {
        if ($(this).val().trim().length > 0) {
            validator.element(this);
        }
    });
});