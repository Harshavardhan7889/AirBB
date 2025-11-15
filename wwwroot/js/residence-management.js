$(document).ready(function() {
    // Edit button click handler
    $('.edit-btn').click(function() {
        var residenceId = $(this).data('residence-id');
        loadResidenceForEdit(residenceId);
    });

    // Delete button click handler
    $('.delete-btn').click(function() {
        var residenceId = $(this).data('residence-id');
        var residenceName = $(this).data('residence-name');
        
        if (confirm('Are you sure you want to delete "' + residenceName + '"?')) {
            deleteResidence(residenceId);
        }
    });

    // Cancel button click handler
    $('#cancel-btn').click(function() {
        resetForm();
        switchToAddMode();
    });

    // Form submission handler
    $('#residence-form').submit(function(e) {
        // Clear any previous deletion alerts
        $('#deletion-alert').hide();
    });
});

function loadResidenceForEdit(residenceId) {
    $.get('/Admin/Manage/GetResidence/' + residenceId, function(data) {
        // Populate form fields
        $('#ResidenceID').val(data.residenceID);
        $('#Name').val(data.name);
        $('#ResidencePicture').val(data.residencePicture);
        $('#LocationID').val(data.locationID);  // This should now select the correct option
        $('#ClientID').val(data.clientID);      // This should now select the correct option
        $('#GuestNumber').val(data.guestNumber);
        $('#BedroomNumber').val(data.bedroomNumber);
        $('#BathroomNumber').val(data.bathroomNumber);
        $('#BuildYear').val(data.buildYear);
        $('#PricePerNight').val(data.pricePerNight);
        $('#IsEdit').val('true');
        
        // Switch to edit mode
        switchToEditMode();
        
        // Switch to Add Residence tab
        $('#add-residence-tab').tab('show');
        
        // Clear any validation errors
        $('.text-danger').empty();
        $('.input-validation-error').removeClass('input-validation-error');
        
    }).fail(function() {
        alert('Error loading residence data.');
    });
}

function deleteResidence(residenceId) {
    $.post('/Admin/Manage/DeleteResidence', { id: residenceId }, function(response) {
        if (response.success) {
            // Remove the row from table
            $('#residence-row-' + residenceId).fadeOut(300, function() {
                $(this).remove();
            });
            showSuccessMessage(response.message);
        } else {
            // Show error message
            $('#deletion-message').text(response.message);
            $('#deletion-alert').show();
            
            // Auto-hide alert after 8 seconds
            setTimeout(function() {
                $('#deletion-alert').fadeOut();
            }, 8000);
        }
    }).fail(function() {
        alert('Error occurred while deleting residence.');
    });
}

function switchToEditMode() {
    $('#submit-btn').text('Update Residence').removeClass('btn-primary').addClass('btn-success');
    $('#cancel-btn').show();
    
    // Update form heading if exists
    $('.card-title').text('Edit Residence');
}

function switchToAddMode() {
    $('#submit-btn').text('Add Residence').removeClass('btn-success').addClass('btn-primary');
    $('#cancel-btn').hide();
    
    // Update form heading if exists
    $('.card-title').text('Add New Residence');
}

function resetForm() {
    $('#residence-form')[0].reset();
    $('#ResidenceID').val('0');
    $('#IsEdit').val('false');
    
    // Clear validation messages
    $('.text-danger').empty();
    $('.input-validation-error').removeClass('input-validation-error');
    $('.field-validation-error').removeClass('field-validation-error');
    
    // Reset dropdowns to default
    $('#LocationID').val('');
    $('#ClientID').val('');
}

function showSuccessMessage(message) {
    if (message && message !== '') {
        // Create a temporary success alert
        var alertHtml = '<div class="alert alert-success alert-dismissible fade show" role="alert">' +
                       '<strong>Success!</strong> ' + message +
                       '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                       '</div>';
        
        // Insert at the top of the container
        $('.container-fluid').prepend(alertHtml);
        
        // Auto-hide after 5 seconds
        setTimeout(function() {
            $('.alert-success').fadeOut(300, function() {
                $(this).remove();
            });
        }, 5000);
    }
}