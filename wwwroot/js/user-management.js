$(document).ready(function () {
    // Edit button click handler
    $('.edit-btn').click(function () {
        var clientId = $(this).data('client-id');
        loadUserForEdit(clientId);
    });

    // Delete button click handler
    $('.delete-btn').click(function () {
        var clientId = $(this).data('client-id');
        var userName = $(this).data('user-name');
        var userType = $(this).data('user-type');
        
        if (confirm('Are you sure you want to delete "' + userName + '"?')) {
            deleteUser(clientId);
        }
    });

    // Cancel button click handler
    $('#cancel-btn').click(function () {
        resetForm();
        switchToAddMode();
    });

    // REMOVED: Form submission handler that was causing issues
    // Let the form submit naturally without interference
});

function loadUserForEdit(clientId) {
    $.get('/Admin/Manage/GetUser/' + clientId, function (data) {
        // Populate form fields
        $('#ClientID').val(data.clientID);
        $('#Name').val(data.name);
        $('#PhoneNumber').val(data.phoneNumber);
        $('#Email').val(data.email);
        $('#SSN').val(data.ssn);
        $('#UserType').val(data.userType);
        $('#DOB').val(data.dob);
        $('#IsEdit').val('true');
        
        // Switch to edit mode
        switchToEditMode();
        
        // ONLY switch to Add User tab if currently on a different tab
        if (!$('#add-user').hasClass('show')) {
            $('#add-user-tab').tab('show');
        }
        
        // Clear any validation errors
        $('.text-danger').empty();
        $('.input-validation-error').removeClass('input-validation-error');
        
        // Trigger validation for populated fields
        $('#Name, #PhoneNumber, #Email, #SSN, #UserType').each(function () {
            if ($(this).val().trim().length > 0) {
                $(this).trigger('blur');
            }
        });
        
    }).fail(function () {
        alert('Error loading user data.');
    });
}

function deleteUser(clientId) {
    $.post('/Admin/Manage/DeleteUser', { id: clientId }, function (response) {
        if (response.success) {
            // Remove the row from table
            $('#user-row-' + clientId).fadeOut(300, function () {
                $(this).remove();
            });
            showSuccessMessage(response.message);
        } else {
            // Show error message
            $('#deletion-message').text(response.message);
            $('#deletion-alert').show();
            
            // Auto-hide alert after 8 seconds
            setTimeout(function () {
                $('#deletion-alert').fadeOut();
            }, 8000);
        }
    }).fail(function () {
        alert('Error occurred while deleting user.');
    });
}

function switchToEditMode() {
    $('#submit-btn').text('Update User').removeClass('btn-primary').addClass('btn-success');
    $('#cancel-btn').show();
}

function switchToAddMode() {
    $('#submit-btn').text('Add User').removeClass('btn-success').addClass('btn-primary');
    $('#cancel-btn').hide();
}

function resetForm() {
    $('#user-form')[0].reset();
    $('#ClientID').val('0');
    $('#IsEdit').val('false');
    
    // Clear validation messages
    $('.text-danger').empty();
    $('.input-validation-error').removeClass('input-validation-error');
    $('.field-validation-error').removeClass('field-validation-error');
    
    // Remove validation styling
    $('#user-form input, #user-form select').removeClass('is-valid is-invalid');
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
        setTimeout(function () {
            $('.alert-success').fadeOut(300, function () {
                $(this).remove();
            });
        }, 5000);
    }
}