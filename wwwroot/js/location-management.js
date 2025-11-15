$(document).ready(function() {
    // Edit button click handler
    $('.edit-btn').click(function() {
        var locationId = $(this).data('location-id');
        loadLocationForEdit(locationId);
    });

    // Delete button click handler
    $('.delete-btn').click(function() {
        var locationId = $(this).data('location-id');
        var locationName = $(this).data('location-name');
        
        if (confirm('Are you sure you want to delete "' + locationName + '"?')) {
            deleteLocation(locationId);
        }
    });

    // Cancel button click handler
    $('#cancel-btn').click(function() {
        resetForm();
        switchToAddMode();
    });

    // Form submission handler
    $('#location-form').submit(function(e) {
        // Clear any previous deletion alerts
        $('#deletion-alert').hide();
    });
});

function loadLocationForEdit(locationId) {
    $.get('/Admin/Manage/GetLocation/' + locationId, function(data) {
        // Populate form fields
        $('#LocationID').val(data.locationID);
        $('#Name').val(data.name);
        $('#IsEdit').val('true');
        
        // Switch to edit mode
        switchToEditMode();
        
        // Switch to Add Location tab
        $('#add-location-tab').tab('show');
        
        // Clear any validation errors
        $('.text-danger').empty();
        $('.input-validation-error').removeClass('input-validation-error');
        
        // Trigger validation for the populated field
        $('#Name').trigger('blur');
        
    }).fail(function() {
        alert('Error loading location data.');
    });
}

function deleteLocation(locationId) {
    $.post('/Admin/Manage/DeleteLocation', { id: locationId }, function(response) {
        if (response.success) {
            // Remove the row from table
            $('#location-row-' + locationId).fadeOut(300, function() {
                $(this).remove();
                
                // Check if table is empty and show message
                if ($('#list-locations tbody tr').length === 0) {
                    $('#list-locations tbody').append(
                        '<tr><td colspan="4" class="text-center text-muted">No locations found. Add your first location using the form above.</td></tr>'
                    );
                }
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
        alert('Error occurred while deleting location.');
    });
}

function switchToEditMode() {
    $('#submit-btn').text('Update Location').removeClass('btn-primary').addClass('btn-success');
    $('#cancel-btn').show();
    
    // Update form heading if exists
    $('.card-title').text('Edit Location');
}

function switchToAddMode() {
    $('#submit-btn').text('Add Location').removeClass('btn-success').addClass('btn-primary');
    $('#cancel-btn').hide();
    
    // Update form heading if exists
    $('.card-title').text('Add New Location');
}

function resetForm() {
    $('#location-form')[0].reset();
    $('#LocationID').val('0');
    $('#IsEdit').val('false');
    
    // Clear validation messages
    $('.text-danger').empty();
    $('.input-validation-error').removeClass('input-validation-error');
    $('.field-validation-error').removeClass('field-validation-error');
    
    // Remove validation styling
    $('#Name').removeClass('is-valid is-invalid');
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