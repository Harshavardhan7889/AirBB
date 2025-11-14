$(document).ready(function () {
    // Handle Edit button click
    $('.edit-btn').on('click', function () {
        var clientId = $(this).data('client-id');
        
        // Fetch user data
        $.get('/Admin/Manage/GetUser/' + clientId, function (data) {
            // Fill form fields
            $('#ClientID').val(data.clientID);
            $('#Name').val(data.name);
            $('#PhoneNumber').val(data.phoneNumber);
            $('#Email').val(data.email);
            $('#SSN').val(data.ssn);
            $('#UserType').val(data.userType);
            $('#DOB').val(data.dob);
            $('#IsEdit').val('true');
            
            // Update form appearance
            $('#form-title').text('Edit User');
            $('#submit-btn').text('Update User');
            $('#cancel-btn').show();
            
            // Switch to Add User tab
            $('#add-user-tab').tab('show');
        }).fail(function () {
            alert('Error loading user data.');
        });
    });

    // Handle Cancel button click
    $('#cancel-btn').on('click', function () {
        resetForm();
    });

    // Handle Delete button click
    $('.delete-btn').on('click', function () {
        var clientId = $(this).data('client-id');
        var userName = $(this).data('user-name');
        var userType = $(this).data('user-type');
        
        if (confirm('Are you sure you want to delete user "' + userName + '"?')) {
            $.post('/Admin/Manage/DeleteUser', { id: clientId }, function (response) {
                if (response.success) {
                    // Remove row from table
                    $('#user-row-' + clientId).fadeOut(function () {
                        $(this).remove();
                    });
                    
                    // Show success message
                    showAlert('success', response.message);
                } else {
                    // Show error message
                    showAlert('danger', response.message);
                }
            }).fail(function () {
                showAlert('danger', 'Error occurred while deleting user.');
            });
        }
    });

    // Handle form submission
    $('#user-form').on('submit', function(e) {
        if ($(this).valid()) {
            // Form is valid, it will submit normally
            // After successful submission (handled by server redirect), 
            // the success message will be shown and tab will be switched
            
            // Store in session storage that we want to switch tabs after redirect
            sessionStorage.setItem('switchToListTab', 'true');
        }
    });

    // Check if we should switch to list tab after form submission
    if (sessionStorage.getItem('switchToListTab') === 'true') {
        sessionStorage.removeItem('switchToListTab');
        $('#list-users-tab').tab('show');
    }

    // Handle form submission success
    if ($('#user-form').length && window.location.search.includes('success')) {
        showAlert('success', 'User saved successfully!');
    }

    function resetForm() {
        $('#user-form')[0].reset();
        $('#ClientID').val('0');
        $('#IsEdit').val('false');
        $('#form-title').text('Add New User');
        $('#submit-btn').text('Add User');
        $('#cancel-btn').hide();
        
        // Clear validation
        $('.is-invalid').removeClass('is-invalid');
        $('.is-valid').removeClass('is-valid');
        $('.field-validation-error').text('');
    }

    function showAlert(type, message) {
        var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
                       message +
                       '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
                       '</div>';
        
        $('#list-users .card-body').prepend(alertHtml);
        
        setTimeout(function () {
            $('.alert').fadeOut();
        }, 5000);
    }

    // Handle tab switching
    $('#userTabs button').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
});