document.addEventListener('DOMContentLoaded', function () {
    const toaster = new ToasterUi();



    function handleButtonClick(event) {
        var method = event.target.getAttribute('data-method');

        var selectedIds = [];
        var checkboxes = document.querySelectorAll('input[type=checkbox]:checked');
        checkboxes.forEach(function (checkbox) {
            selectedIds.push(checkbox.value);
        });

        if (selectedIds.length === 0) {
            toaster.addToast("Please select at least one user", "warning");
            return;
        }
        
        var data = {
            ids: selectedIds.join(','),
            method: method
        };

        fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: new URLSearchParams(data).toString()
            })
            .then(function (response) {
                if (!response.ok) {
                    toaster.addToast("Please select at least one user.", "error");
                }
                return response.json();
            })
            .then(function (users) {
                updateUsersTable(users.result);
                toaster.addToast("Users updated successfully.", "success", { styles: { background:'#4bbf73' }});
            })
            .catch(function (error) {
                toaster.addToast("An error ocurred.", "error");
            });
    }

    function updateUsersTable(users) {
        var usersContainer = document.querySelector('.usersContainer');

        usersContainer.innerHTML = '';

        users.forEach(function (user) {
            var row = `
                <div class="row justify-content-center border-top">
            <div class="col-12 col-md-1 d-flex justify-content-center align-items-center">
                <input value="${user.id}" type="checkbox" class="form-check-input border-primary" />
            </div>
            <div class="col-12 col-md-2 d-flex justify-content-center align-items-center py-0 py-md-3">
                ${user.userName}
            </div>
            <div class="col-12 col-md-3 d-flex justify-content-center align-items-center py-0 py-md-3">
                ${user.email}
            </div>
            <div class="col-12 col-md-2 d-flex justify-content-center align-items-center py-0 py-md-3">
                ${user.lastLogin}
            </div>
            <div class="col-12 col-md-2 d-flex justify-content-center align-items-center py-0 py-md-3">
                ${user.registration}
            </div>
            <div class="col-12 col-md-2 d-flex justify-content-center align-items-center py-0 py-md-3">
                ${user.status}
            </div>
        </div>
            `;
            usersContainer.innerHTML += row;
        });
    }

    var buttons = document.querySelectorAll('.actionButton');
    buttons.forEach(function (button) {
        button.addEventListener('click', handleButtonClick);
    });
});